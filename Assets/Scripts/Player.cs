using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class Player : MonoBehaviour {

    [System.Serializable]
    public class PlayerStats
    {
        public float maxHealth = 5f;
        [Header("Note: 1.0 = 100%")] [Range(0.1f, 10.0f)] public float startHealthPercent;

        [HideInInspector]
        public float actualMaxHealth;

        public float accuracyFactor = 0.4f; // 0 equals 100 percent accurate.

        [SerializeField] private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, actualMaxHealth); }
        }

        public void Init()
        {
            currentHealth = actualMaxHealth;
        }
    }

    public PlayerStats playerStats = new PlayerStats();

    public int fallBoundary = -20;

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    public bool isDead = false;

    private AudioManager audioManager;

    public static Player instance;

    [HideInInspector]
    public Vector3 playerLocation;

    // [SerializeField] private StatusIndicator statusIndicator;

    Animator playerAnim;

    BoxCollider2D boxCollider;
    Rigidbody2D rb;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        playerStats.actualMaxHealth = playerStats.maxHealth * playerStats.startHealthPercent;
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

        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

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
        {
            _weapon.enabled = !active;
        }
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

            boxCollider.offset = new Vector2(-0.13f, -23.6f); // So that if the enemy shoots the player when they're dead, it doesn't produce a blood splatter from empty space
            boxCollider.size = new Vector2(52.5f, 4.8f);
        }
        else if (playerStats.currentHealth > 0 && isDead == false)
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        // statusIndicator.SetHealth(playerStats.currentHealth, playerStats.maxHealth);
    }
}
