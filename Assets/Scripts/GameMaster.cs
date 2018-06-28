using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    public static GameMaster gm;

    [SerializeField] private int startingMoney = 100;
    public static int Money;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 3;

    public CameraShake cameraShake;

    [SerializeField] private GameObject upgradeMenu;

    [SerializeField] private WaveSpawner waveSpawner;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;

    Player player;
    Enemy enemy;

    Scene currentScene;

    // Cache
    private AudioManager audioManager;

    void Awake()
    {
        if (gm == null)
        {
            gm = this;
        }
    }

    void Start()
    {
        player = Player.instance;
        enemy = Enemy.instance;

        if (cameraShake == null)
        {
            Debug.LogError("No camera shake referenced in GameMaster.");
        }

        Money = startingMoney;

        // Caching
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }

        currentScene = SceneManager.GetActiveScene();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUpgradeMenu();
        }

        Debug.Log("Player Health: " + player.playerStats.currentHealth);
    }

    private void ToggleUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        waveSpawner.enabled = !upgradeMenu.activeSelf;
        onToggleUpgradeMenu.Invoke(upgradeMenu.activeSelf);
    }

    public IEnumerator _RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        SceneManager.LoadScene(currentScene.buildIndex);

        //Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public static void KillPlayer(Player player)
    {
        gm.StartCoroutine(gm._RespawnPlayer());
    }

    public static void KillEnemy(Enemy enemy)
    {
        gm._KillEnemy(enemy);
    }

    public void _KillEnemy(Enemy _enemy)
    {
        // Death sound
        audioManager.PlaySound(_enemy.deathSoundName);

        // Drop money on death
        Money += _enemy.moneyDrop;
        audioManager.PlaySound("Money");

        // Camera Shake
        // cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
    }
}
