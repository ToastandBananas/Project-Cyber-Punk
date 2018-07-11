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

    [SerializeField] private GameObject upgradeMenu;

    // [SerializeField] private WaveSpawner waveSpawner;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;

    Player player;

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

        Money = startingMoney;

        // Sound
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

        debugPause();

        // Debug.Log("Player Health: " + player.playerStats.currentHealth);
    }

    void debugPause()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Break();
        }
    }

    private void ToggleUpgradeMenu()
    {
        upgradeMenu.SetActive(!upgradeMenu.activeSelf);
        // waveSpawner.enabled = !upgradeMenu.activeSelf;
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
}
