using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public static int money = 0;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public int spawnDelay = 3;

    public delegate void UpgradeMenuCallback(bool active);
    public UpgradeMenuCallback onToggleUpgradeMenu;
    [SerializeField] private GameObject upgradeMenu;

    public delegate void PauseMenuCallback(bool active);
    public PauseMenuCallback onTogglePauseMenu;
    [SerializeField] private GameObject pauseMenu;
    PauseMenu pauseMenuScript;

    [Header("Game Stats")]
    public int totalVictimsSaved = 0;
    
    [Header("Gadget Stats")]
    public int teleportationDeviceMaxCharge = 1;
    public int teleportationDeviceRange = 3;
    public int AIHackingDeviceMaxCharge = 1;
    public int AIHackingDeviceRange = 3;
    public int AIScanningDeviceMaxCharge = 1;
    public int AIScanningDeviceRange = 3;

    // [SerializeField] private WaveSpawner waveSpawner;

    Player player;
    GameObject[] enemies;

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
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        pauseMenuScript = pauseMenu.GetComponent<PauseMenu>();

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf == true || (pauseMenu.activeSelf == false && pauseMenuScript.quitConfirmation.activeSelf == false && pauseMenuScript.optionsMenu.activeSelf == false))
                TogglePauseMenu();
            else if (pauseMenuScript.quitConfirmation.activeSelf == true)
            {
                pauseMenuScript.quitConfirmation.SetActive(false);
                player.SetScriptsActive();
                foreach (GameObject enemy in enemies)
                    enemy.GetComponent<Enemy>().SetScriptsActive();
            }
            else if (pauseMenuScript.optionsMenu.activeSelf == true)
            {
                pauseMenuScript.optionsMenu.SetActive(false);
                player.SetScriptsActive();
                foreach (GameObject enemy in enemies)
                    enemy.GetComponent<Enemy>().SetScriptsActive();
            }
        }

        debugPause();

        // Debug.Log("Player Health: " + player.playerStats.currentHealth);
    }

    private void debugPause()
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

        if (upgradeMenu.activeSelf == true)
        {
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            Cursor.visible = true;
        }
        else
        {
            player.GetComponent<Rigidbody2D>().gravityScale = 1;
            Cursor.visible = false;
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        // waveSpawner.enabled = !pauseMenu.activeSelf;
        onTogglePauseMenu.Invoke(pauseMenu.activeSelf);

        if (pauseMenu.activeSelf == true)
        {
            //Time.timeScale = 0;
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            foreach (GameObject enemy in enemies)
                enemy.GetComponent<Rigidbody2D>().gravityScale = 0;

            Cursor.visible = true;
        }
        else
        {
            //Time.timeScale = 1;
            if (pauseMenu.GetComponent<PauseMenu>().quitConfirmation.activeSelf == true)
                pauseMenu.GetComponent<PauseMenu>().quitConfirmation.SetActive(false);

            player.GetComponent<Rigidbody2D>().gravityScale = 1;
            foreach (GameObject enemy in enemies)
                enemy.GetComponent<Rigidbody2D>().gravityScale = 1;

            Cursor.visible = false;
        }
    }

    public IEnumerator _RespawnPlayer()
    {
        yield return new WaitForSeconds(spawnDelay);

        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public static void KillPlayer(Player player)
    {
        gm.StartCoroutine(gm._RespawnPlayer());
    }
}
