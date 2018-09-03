using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth = 5f;
        [Header("Note: 1.0 = 100%")] [Range(0.1f, 1.0f)] public float startHealthPercent = 1f;

        public float inaccuracyFactor = 0.15f; // 0 equals 100 percent accurate.

        [SerializeField] private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, maxHealth); }
        }

        public void Init()
        {
            currentHealth = maxHealth * startHealthPercent;
        }
    }

    public PlayerStats playerStats = new PlayerStats();

    public int fallBoundary = -20;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    public bool isDead = false;

    public bool hasEquippedStartingWeapon = false;
    public GameObject itemToPickup;

    GameObject[] doors;

    private AudioManager audioManager;

    public static Player instance;

    [HideInInspector]
    public Vector3 playerLocation;

    // [SerializeField] private StatusIndicator statusIndicator;

    Animator playerAnim;

    CapsuleCollider2D capsuleCollider;
    Rigidbody2D rb;

    // Timer for weapon pick ups
    public float timeSinceQPressed;

    [Header("LightRaycast Script Variables")]
    public float lightTimer = 0.0f; // For the LightRaycast script
    public bool isVisibleByLight = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        doors = GameObject.FindGameObjectsWithTag("Door");

        playerStats.Init();

        playerAnim = GetComponent<Animator>();

        playerAnim.SetFloat("health", playerStats.currentHealth);

        /*if (statusIndicator == null)
        {
            Debug.LogError("No status indicator referenced on Player.");
        }
        else
        {
            statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
        }*/

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;
        GameMaster.gm.onTogglePauseMenu += OnPauseMenuToggle;

        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    void Update()
    {
        playerLocation = transform.localScale;

        if (transform.position.y <= fallBoundary)
        {
            DamagePlayer(1000000000);
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // Handle what happens when the upgrade menu is toggled
        GetComponent<PlayerController>().enabled = !active;

        Weapon _weapon = GetComponentInChildren<Weapon>();
        if(_weapon != null)
            _weapon.enabled = !active;

        ArmRotation playerArmRotationScript = GetComponentInChildren<ArmRotation>();
        if (playerArmRotationScript != null)
            playerArmRotationScript.enabled = !active;

        foreach (GameObject door in doors)
            door.GetComponentInChildren<Door>().enabled = !active;
    }

    void OnPauseMenuToggle(bool active)
    {
        // Handle what happens when the pause menu is toggled
        GetComponent<PlayerController>().enabled = !active;

        Weapon _weapon = GetComponentInChildren<Weapon>();
        if (_weapon != null)
            _weapon.enabled = !active;

        ArmRotation playerArmRotationScript = GetComponentInChildren<ArmRotation>();
        if (playerArmRotationScript != null)
            playerArmRotationScript.enabled = !active;

        foreach (GameObject door in doors)
            door.GetComponentInChildren<Door>().enabled = !active;
    }

    public void DamagePlayer(float damage)
    {
        if (playerStats.currentHealth > 0)
        {
            playerStats.currentHealth -= damage;
            playerAnim.SetFloat("health", playerStats.currentHealth);
        }

        if (playerStats.currentHealth <= 0 && isDead == false )
        {
            // Play death sound
            audioManager.PlaySound(deathSoundName);

            // Kill player
            GameMaster.KillPlayer(this);
            isDead = true;

            rb.drag = 10; // So that the player doesn't slide if they're moving when they die

            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.offset = new Vector2(3.43f, -23.89f);
            capsuleCollider.size = new Vector2(46.55f, 9.1f);
        }
        else if (playerStats.currentHealth > 0 && isDead == false)
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        // statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }
}
