﻿using UnityEngine;

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

    int startingWeaponID;

    // public float shakeAmt = 0.1f;
    // public float shakeLength = 0.3f;

    [Header("Optional: ")][SerializeField] private StatusIndicator statusIndicator;

    void Start()
    {
        enemyArm = transform.Find("EnemyArm").gameObject;

        RandomizeStartingWeapon();

        player = Player.instance;
        enemyMovementScript = gameObject.GetComponent<EnemyMovement>();
        lootDropScript = GetComponent<LootDrop>();

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

        if (enemyStats.currentHealth <= 0 && isDead == false) // If enemy health falls to 0 or less they will die
        {
            // Death sound
            audioManager.PlaySound(deathSoundName);

            // Camera Shake
            // cameraShake.Shake(_enemy.shakeAmt, _enemy.shakeLength);
            
            isDead = true;
            anim.SetBool("isDead", isDead);
            enemyMovementScript.currentState = EnemyMovement.State.Dead;

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
            lootDropScript.DropWeapon(enemyWeaponScript.currentAmmoAmount, enemyWeaponScript.clipSize, enemyWeaponScript.ammoType, enemyWeaponScript.damage, enemyWeaponScript.playerFireRate, enemyWeaponScript.isSilenced, 
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
