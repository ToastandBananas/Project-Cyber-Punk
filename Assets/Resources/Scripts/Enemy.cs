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

    public int minMoneyDrop = 2;
    public int maxMoneyDrop = 10;
    public int actualMoney;

    public float distanceToPlayer;

    public bool isDead = false;

    public Material highlightMaterial;
    public Material defaultMaterial;

    private AudioManager audioManager;

    CapsuleCollider2D playerCapsuleCollider;
    CapsuleCollider2D capsuleCollider;
    CircleCollider2D hearingCollider;
    CircleCollider2D sightCollider;
    BoxCollider2D lootBoxCollider;

    SpriteRenderer spriteRenderer;

    Animator anim;

    Player player;
    EnemyMovement enemyMovementScript;
    LootDrop lootDropScript;

    GameObject enemyWeapon;
    GameObject enemyArm;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        player = Player.instance;
        enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        lootDropScript = GetComponent<LootDrop>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        enemyArm = transform.Find("EnemyArm").gameObject;
        enemyWeapon = enemyArm.transform.GetChild(0).gameObject;

        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        hearingCollider = transform.Find("Hearing").GetComponent<CircleCollider2D>();
        sightCollider = transform.Find("Sight").GetComponent<CircleCollider2D>();
        lootBoxCollider = GetComponent<BoxCollider2D>();
        lootBoxCollider.enabled = false;

        Physics2D.IgnoreCollision(capsuleCollider, playerCapsuleCollider);

        enemyStats.actualMaxHealth = enemyStats.maxHealth * enemyStats.startHealthPercent;
        enemyStats.Init();
        actualMoney = Mathf.RoundToInt(Random.Range(minMoneyDrop, maxMoneyDrop));

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

        if (enemyStats.currentHealth <= 0) // If enemy is dead
        {
            // Drop held weapon with current amount of ammo and specify the ammo type (so that it can be accessed when the player tries to pick up ammo in the WeaponPickup script)
            lootDropScript.DropWeapon(enemyWeapon.GetComponent<EnemyWeapon>().currentAmmoAmount, enemyWeapon.GetComponent<EnemyWeapon>().ammoType, enemyWeapon.GetComponent<EnemyWeapon>().damage, enemyWeapon.GetComponent<EnemyWeapon>().playerFireRate);
            lootBoxCollider.enabled = true;
            sightCollider.enabled = false;
            hearingCollider.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision) // Damage player on collision with enemy
    {
        Player _player = collision.collider.GetComponent<Player>();

        if(_player != null)
        {
            _player.DamagePlayer(enemyStats.onTouchDamage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isDead == true)
        {
            if (actualMoney > 0)
            {
                spriteRenderer.material = highlightMaterial;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameMaster.Money += actualMoney; // Loot money from enemy
                    actualMoney = 0;
                    audioManager.PlaySound("Money");
                    spriteRenderer.material = defaultMaterial;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isDead == true)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
