using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour {

    [System.Serializable]
    public class EnemyStats
    {
        public float maxHealth = 1;
        [Header("Note: 1.0 = 100%")]
        [Range(0.1f, 10.0f)] public float startHealthPercent = 1f;

        [HideInInspector]
        public float actualMaxHealth;

        private float _currentHealth;
        public float currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0f, actualMaxHealth); }
        }

        public int hearingRadius = 200;

        [Header("Note: 0 equals 100% accurate")]
        public float inaccuracyFactor = 0.4f;

        public void Init()
        {
            currentHealth = actualMaxHealth;
        }
    }

    public EnemyStats enemyStats = new EnemyStats();
    
    [Header("Sounds")]
    public string deathSoundName = "DeathVoice";
    public string damageSoundName = "DamageVoice";

    [Header("Money")]
    public int minMoneyDrop = 2;
    public int maxMoneyDrop = 10;
    public int actualMoney;

    [Header("Materials")]
    public Material highlightMaterial;
    public Material defaultMaterial;

    [Header("Other Variables")]
    public float distanceToPlayer;
    public bool isDead = false;
    public bool isAssassinationTarget = false;
    public bool isHackable = true;

    private AudioManager audioManager;

    CapsuleCollider2D playerCapsuleCollider;
    CapsuleCollider2D thisEnemyCollider;
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
    EnemyWeapon enemyWeaponScript;
    LevelExit levelExitScript;
    MouseCursor cursor;
    Gadget gadgetScript;

    GameObject scannerInfoTooltip;

    int startingWeaponID;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        enemyArm = transform.Find("EnemyArm").gameObject;

        RandomizeStartingWeapon();
        
        if (enemyArm.transform.childCount > 0)
            enemyWeapon = enemyArm.transform.GetChild(0).gameObject;

        player = Player.instance;
        enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        enemyWeaponScript = enemyArm.GetComponentInChildren<EnemyWeapon>();
        lootDropScript = GetComponent<LootDrop>();
        levelExitScript = GameObject.Find("LevelExitTrigger").GetComponent<LevelExit>();
        cursor = MouseCursor.instance;
        gadgetScript = GameObject.Find("GadgetSlotPanel").GetComponent<Gadget>();

        scannerInfoTooltip = gadgetScript.scannerInfoTooltip;

        spriteRenderer = GetComponent<SpriteRenderer>();

        playerCapsuleCollider = player.GetComponent<CapsuleCollider2D>();
        thisEnemyCollider = GetComponent<CapsuleCollider2D>();
        hearingCollider = transform.Find("Hearing").GetComponent<CircleCollider2D>();
        sightCollider = transform.Find("Sight").GetComponent<CircleCollider2D>();
        lootBoxCollider = GetComponent<BoxCollider2D>();
        lootBoxCollider.enabled = false;

        Physics2D.IgnoreCollision(thisEnemyCollider, playerCapsuleCollider);

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
        GameMaster.gm.onTogglePauseMenu += OnPauseMenuToggle;

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

        DisplayScannerTooltip();
    }

    void RandomizeStartingWeapon()
    {
        if (enemyArm.transform.childCount == 0) // If no weapon already equipped
        {
            var possibleWeapons = Resources.LoadAll<GameObject>("Prefabs/Items/EnemyWeapons");
            startingWeaponID = Random.Range(0, possibleWeapons.Length);
            foreach (GameObject weapon in possibleWeapons)
            {
                if (startingWeaponID == weapon.GetComponent<EnemyWeapon>().weaponID)
                {
                    Instantiate(weapon, enemyArm.transform);
                }
            }
        }
    }

    void DisplayScannerTooltip()
    {
        if (enemyMovementScript.isScanned && isDead == false && (gadgetScript.enemyBeingScanned == null || gadgetScript.enemyBeingScanned == transform))
        {
            if (Vector2.Distance(transform.position, cursor.transform.position) <= 0.25f)
            {
                gadgetScript.enemyBeingScanned = transform;

                string silencedText;
                if (enemyWeaponScript.isSilenced)
                    silencedText = "Silenced ";
                else
                    silencedText = "";

                string fireRateText;
                if (enemyWeaponScript.fireRate > 1)
                    fireRateText = "Fire Rate: " + enemyWeaponScript.fireRate + " rounds/second" + "\n";
                else
                    fireRateText = "";

                scannerInfoTooltip.SetActive(true);
                scannerInfoTooltip.GetComponentInChildren<Text>().text = "Health: " + enemyStats.currentHealth + "/" + enemyStats.actualMaxHealth + "\n\n"
                                                                            + "<size=22px>" + silencedText + enemyWeaponScript.gameObject.name + "</size>\n"
                                                                            + "Action Type: " + enemyWeaponScript.actionType + "\n"
                                                                            + "Ammo: " + enemyWeaponScript.currentAmmoAmount + "/" + enemyWeaponScript.clipSize + "\n"
                                                                            + "Damage: " + enemyWeaponScript.damage + "\n"
                                                                            + fireRateText
                                                                            + "Durability: " + enemyWeaponScript.durability + "%" + "\n"
                                                                            + "Ammo Type: " + enemyWeaponScript.ammoType;
            }
            else
            {
                scannerInfoTooltip.SetActive(false);
                gadgetScript.enemyBeingScanned = null;
            }
        }
        else if (isDead)
        {
            scannerInfoTooltip.SetActive(false);
            gadgetScript.enemyBeingScanned = null;
        }
    }

    void OnUpgradeMenuToggle(bool active)
    {
        // Handle what happens when the upgrade menu is toggled
        if (this != null)
        {
            enemyMovementScript.enabled = !active;
            enemyWeaponScript.enabled = !active;
        }
    }

    void OnPauseMenuToggle(bool active)
    {
        // Handle what happens when the pause menu is toggled
        if (this != null)
        {
            enemyMovementScript.enabled = !active;
            enemyWeaponScript.enabled = !active;
        }
    }

    public void DamageEnemy(float damage)
    {
        if (enemyStats.currentHealth > 0)
        {
            enemyStats.currentHealth -= damage;
            anim.SetFloat("health", enemyStats.currentHealth);
        }

        if (enemyStats.currentHealth <= 0 && isDead == false) // If enemy health falls to 0 or less they will die
        {
            // Death sound
            audioManager.PlaySound(deathSoundName);

            // Camera Shake
            // cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
            
            isDead = true;
            anim.SetBool("isDead", isDead);
            enemyMovementScript.currentState = EnemyMovement.State.Dead;

            levelExitScript.enemiesKilled++;
            if (levelExitScript.enemiesKilled == levelExitScript.enemyCount)
                levelExitScript.KillAllEnemiesMissionComplete();

            if (isAssassinationTarget)
                levelExitScript.targetsAssassinated++;

            if (levelExitScript.targetsAssassinated == levelExitScript.assassinationTargetCount)
                levelExitScript.AssassinationMissionComplete();

            thisEnemyCollider.direction = CapsuleDirection2D.Horizontal;
            thisEnemyCollider.offset = new Vector2(3.43f, -19.05f);
            thisEnemyCollider.size = new Vector2(46.55f, 10.41f);
        }
        else if (enemyStats.currentHealth > 0)
        {
            // Play damage sound
            audioManager.PlaySound(damageSoundName);
        }

        if (enemyStats.currentHealth <= 0) // If enemy is dead
        {
            if (enemyWeapon == null)
            {
                enemyWeapon = enemyArm.transform.GetChild(0).gameObject;
                enemyWeaponScript = enemyWeapon.transform.GetComponent<EnemyWeapon>();
            }
            // Drop held weapon with current amount of ammo and specify the ammo type (so that it can be accessed when the player tries to pick up ammo in the WeaponPickup script's PickUpAmmo() method)
            lootDropScript.DropWeapon(enemyWeaponScript.currentAmmoAmount, enemyWeaponScript.clipSize, enemyWeaponScript.ammoType, enemyWeaponScript.damage, enemyWeaponScript.fireRate, enemyWeaponScript.isSilenced, 
                                        enemyWeaponScript.hasIncreasedClipSize, enemyWeaponScript.clipSizeMultiplier, enemyWeaponScript.inaccuracyFactor, enemyWeaponScript.durability, enemyWeaponScript.hasAlteredDurability,
                                        enemyWeaponScript.durabilityMultiplier);

            lootBoxCollider.enabled = true;
            sightCollider.enabled = false;
            hearingCollider.enabled = false;
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
                    GameMaster.money += actualMoney; // Loot money from enemy
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
