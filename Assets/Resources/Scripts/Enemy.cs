using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats
    {
        public float maxHealth = 1;
        [Header("Note: 1.0 = 100%")] [Range(0.1f, 10.0f)] public float startHealthPercent = 1f;

        [HideInInspector]
        public float actualMaxHealth;

        private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, actualMaxHealth); }
        }

        public int hearingRadius = 200;

        [Header("Note: 0 equals 100% accurate")] public float accuracyFactor = 0.4f;

        public float onTouchDamage = 0;

        public void Init()
        {
            currentHealth = actualMaxHealth;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();

    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    public int moneyDrop = 10;

    public float distanceToPlayer;

    public bool isDead = false;

    private AudioManager audioManager;

    CapsuleCollider2D capsuleCollider;
    CapsuleCollider2D playerCapsuleCollider;

    Animator anim;

    Player player;
    EnemyMovement enemyMovementScript;
    LootDrop lootDropScript;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        player = Player.instance;
        enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        lootDropScript = GetComponent<LootDrop>();

        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();

        Physics2D.IgnoreCollision(capsuleCollider, playerCapsuleCollider);

        enemyStats.actualMaxHealth = enemyStats.maxHealth * enemyStats.startHealthPercent;
        enemyStats.Init();

        anim = GetComponent<Animator>();

        anim.SetFloat("health", enemyStats.currentHealth);

        /*if (statusIndicator != null)
        {
            statusIndicator.SetHealth(enemyStats.currentHealth, enemyStats.maxHealth); // Set health to max health at start
        }*/

        GameMaster.gm.onToggleUpgradeMenu += OnUpgradeMenuToggle;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audio manager in scene.");
        }
    }

    void FixedUpdate()
    {
        if (enemyMovementScript.currentState != EnemyMovement.State.Dead)
        {
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            // print("Distance to Player: " + distanceToPlayer + " units");
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // Handle what happens when the upgrade menu is toggled
        if (this != null)
        {
            GetComponent<EnemyMovement>().enabled = !active;
        }
    }

    public void DamageEnemy(float damage)
    {
        if (enemyStats.currentHealth > 0)
        {
            enemyStats.currentHealth -= damage;
            anim.SetFloat("health", enemyStats.currentHealth);
        }

        if (enemyStats.currentHealth <= 0 && isDead == false)
        {
            // Death sound
            audioManager.PlaySound(deathSoundName);

            // Drop money on death
            GameMaster.Money += moneyDrop;
            audioManager.PlaySound("Money");

            // Camera Shake
            // cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
            
            isDead = true;
            enemyMovementScript.currentState = EnemyMovement.State.Dead;

            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
            capsuleCollider.offset = new Vector2(3.43f, -23.89f);
            capsuleCollider.size = new Vector2(46.55f, 9.1f);
        }
        else if (enemyStats.currentHealth > 0)
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        if (enemyStats.currentHealth <= 0)
        {
            lootDropScript.DropWeapon();
        }
    }

    void OnCollisionEnter2D(Collision2D _colliderInfo) // Damage player on collision with enemy
    {
        Player _player = _colliderInfo.collider.GetComponent<Player>();

        if(_player != null)
        {
            _player.DamagePlayer(enemyStats.onTouchDamage);
        }
    }
}
