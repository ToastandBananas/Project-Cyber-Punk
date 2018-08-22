using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyWeapon : MonoBehaviour {

    Enemy enemyScript;
    EnemySight enemySightScript;
    EnemyMovement enemyMovementScript;
    Player player;
    Transform enemy;
    Transform enemySight;

    public LayerMask whatToHit;
    public LayerMask whatToHitWhileHacked;
    public int weaponID;

    [Header("Stats:")]
    public float inaccuracyFactor = 0;
    public float finalAccuracyFactor;
    public float damage;
    public string ammoType;
    public int clipSize;
    public int currentAmmoAmount;
    public float durability;
    public float durabilityUse;
    public string actionType;
    public bool isTwoHanded = false;
    public bool isBoltAction = false;
    public bool isSniper = false;

    [Header("Actual size will depend on parent object size:")]
    public int soundRadius;

    [Header("Perk Variables:")] // Mostly for tooltip use
    public bool isSilenced = false;
    public bool hasIncreasedClipSize = false;
    public float clipSizeMultiplier;
    public bool hasAlteredInaccuracyFactor = false;
    public bool hasAlteredDurability = false;
    public float durabilityMultiplier;

    private float initialWaitToShootTime;

    [Header("Time between each shot for semi-auto or single shot guns:")]
    public bool isShotgun = false;
    public float semiAutoCoolDownTime = 0.75f;

    [Header("Time between each shot for automatic or burst-fire guns:")]
    public bool isAutomatic = false;
    public float autoCoolDownTime = .2f;
    private float coolDownTime;

    [Header("Effects:")]
    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitEffectPrefab;
    public int muzzleFlashMinSize = 7;
    public int muzzleFlashMaxSize = 10;
    
    float timeToFire = 0;
    float timeToSpawnEffect = 0;
    float effectSpawnRate = 10;
    Transform firePoint;
    public float playerFireRate;
    
    // Sound
    private AudioManager audioManager;
    string gunfireSoundName;

    [Header("Max distance that the enemy will shoot the player from:")]
    public float maxShootDistance = 5f;
    
    bool isAbleToShoot = false;
    bool isCoolingDown = false;

    Rigidbody2D rb;

    RaycastHit2D hit;
    Vector2 bulletMoveDirection;

    ProduceSoundTrigger produceSoundTriggerScript;
    ItemDatabase itemDatabase;
    WeaponPerks weaponPerksScript;

    void Awake()
    {
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();
        produceSoundTriggerScript = gameObject.GetComponent<ProduceSoundTrigger>();

        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No fire point...Please make an empty object named FirePoint and attach to end of gun. :)");
        }
    }

    // Use this for initialization
    void Start () {
        enemy = transform.root;
        enemySight = enemy.Find("Sight");

        enemySightScript = enemySight.GetComponent<EnemySight>();
        enemyMovementScript = enemy.GetComponent<EnemyMovement>();
        weaponPerksScript = GetComponent<WeaponPerks>();
        enemyScript = enemy.GetComponent<Enemy>();
        player = Player.instance;

        audioManager = AudioManager.instance;
        if (audioManager == null)
            Debug.LogError("No AudioManager found in the scene");

        Item weaponItem = itemDatabase.FetchItemByID(weaponID);
        name = weaponItem.ItemName;
        damage = Mathf.Round(Random.Range(weaponItem.MinDamage, weaponItem.MaxDamage) * 100.0f) / 100.0f;
        clipSize = weaponItem.ClipSize;
        ammoType = weaponItem.AmmoType;
        playerFireRate = Mathf.Round(Random.Range(weaponItem.MinFireRate, weaponItem.MaxFireRate) * 100.0f) / 100.0f;
        autoCoolDownTime = Mathf.Round((1 / playerFireRate) * 100.0f) / 100.0f;
        actionType = weaponItem.ActionType;
        soundRadius = weaponItem.SoundRadius;
        durabilityUse = weaponItem.DurabilityUse;

        weaponPerksScript.RandomizePerks(weaponID);

        finalAccuracyFactor = enemy.GetComponent<Enemy>().enemyStats.inaccuracyFactor + inaccuracyFactor;
        if (finalAccuracyFactor < 0)
            finalAccuracyFactor = 0;

        currentAmmoAmount = clipSize;

        DetermineSoundName();
    }
	
	// Update is called once per frame
	void Update () {
        CheckIfShooting();

        if (enemyMovementScript.stillSearching == false)
            initialWaitToShootTime = 1.5f;

        if (currentAmmoAmount <= 0)
            Invoke("Reload", 2f); // Enemy has infinite ammo, but will only drop what is currently loaded in their gun when they die
    }

    void CheckIfShooting()
    {
        if (enemyScript.isDead == false && enemyMovementScript.currentState == EnemyMovement.State.Attack)
        {
            if ((enemySightScript.CanPlayerBeSeen() == true && enemyScript.distanceToPlayer <= maxShootDistance)
                || (enemyMovementScript.currentTarget != null && enemyMovementScript.currentTarget == enemyMovementScript.enemyToAttack && Vector2.Distance(enemyMovementScript.transform.position, enemyMovementScript.currentTarget.position) <= maxShootDistance))
            {
                isAbleToShoot = true;
            }
            else
            {
                isAbleToShoot = false;
            }

            if (isCoolingDown == false && isAbleToShoot == true && currentAmmoAmount > 0)
            {
                if (isAutomatic == false) // For semi-auto guns
                {
                    coolDownTime = semiAutoCoolDownTime;
                    StartCoroutine(EnemyShoot());
                }
                else if (isAutomatic == true) // For automatic guns
                {
                    coolDownTime = autoCoolDownTime;
                    StartCoroutine(EnemyShoot());
                }
            }
        }
    }

    IEnumerator EnemyShoot()
    {
        StartCoroutine(Wait());

        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);

        if (enemyMovementScript.enemyToAttack != null && enemyMovementScript.currentTarget != null && enemyMovementScript.currentTarget != player.transform)
        {
            Vector2 enemyToAttackPosition = new Vector2(enemyMovementScript.enemyToAttack.position.x, enemyMovementScript.enemyToAttack.position.y);
            hit = Physics2D.Raycast(firePointPosition, (enemyToAttackPosition - firePointPosition), 100f, whatToHitWhileHacked);
        }
        else if (enemyScript.distanceToPlayer < 2f)
        {
            hit = Physics2D.Raycast(firePointPosition, (playerPosition - firePointPosition), 100f, whatToHit);
        }
        else
        {
            hit = Physics2D.Raycast(firePointPosition, (playerPosition - firePointPosition) + new Vector2(0, Random.Range(-finalAccuracyFactor, finalAccuracyFactor)), 100f, whatToHit);
        }

        Vector3 hitPos;
        Vector3 hitNormal;

        if (initialWaitToShootTime == 0)
        {
            if (hit.collider != null)
            {
                Player player = hit.collider.GetComponent<Player>();
                Victim victim = hit.collider.GetComponent<Victim>();
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (player != null || victim != null || enemy != null)
                {
                    hitPos = hit.point;
                    hitNormal = hit.normal;

                    if (isShotgun)
                    {
                        if (Vector2.Distance(firePointPosition, hitPos) >= 3.2 && Vector2.Distance(firePointPosition, hitPos) < 5)
                        {
                            if (player != null)
                                player.DamagePlayer(damage / 2);
                            else if (victim != null)
                                victim.DamageVictim(damage / 2);
                            else if (enemy != null)
                            {
                                enemy.DamageEnemy(damage / 2);
                                if (enemy.isDead == false)
                                {
                                    enemy.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
                                    enemy.GetComponent<EnemyMovement>().enemyToAttack = transform.root.transform;
                                }
                            }
                            // Debug.Log("We hit " + hit.collider.name + " and did " + damage / 2 + " damage.");
                        }
                        else if (Vector2.Distance(firePointPosition, hitPos) >= 5)
                        {
                            if (player != null)
                                player.DamagePlayer(1);
                            else if (victim != null)
                                victim.DamageVictim(1);
                            else if (enemy != null)
                            {
                                enemy.DamageEnemy(1);
                                if (enemy.isDead == false)
                                {
                                    enemy.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
                                    enemy.GetComponent<EnemyMovement>().enemyToAttack = transform.root.transform;
                                }
                            }
                            // Debug.Log("We hit " + hit.collider.name + " and did " + 1 + " damage.");
                        }
                        else
                        {
                            if (player != null)
                                player.DamagePlayer(damage);
                            else if (victim != null)
                                victim.DamageVictim(damage);
                            else if (enemy != null)
                            {
                                enemy.DamageEnemy(damage);
                                if (enemy.isDead == false)
                                {
                                    enemy.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
                                    enemy.GetComponent<EnemyMovement>().enemyToAttack = transform.root.transform;
                                }
                            }
                            // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                        }
                    }
                    else
                    {
                        if (player != null)
                            player.DamagePlayer(damage);
                        else if (victim != null)
                            victim.DamageVictim(damage);
                        else if (enemy != null)
                        {
                            enemy.DamageEnemy(damage);
                            if (enemy.isDead == false)
                            {
                                enemy.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
                                enemy.GetComponent<EnemyMovement>().enemyToAttack = transform.root.transform;
                            }
                        }
                        // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                    }
                    
                    CreateHitParticle(hitPos, hitNormal);
                }
            }

            if (Time.time >= timeToSpawnEffect)
            {
                if (hit.collider == null)
                {
                    hitPos = (playerPosition - firePointPosition) * 9999;
                    hitNormal = new Vector3(9999, 9999, 9999);
                }
                else
                {
                    hitPos = hit.point;
                    hitNormal = hit.normal;
                }

                Effect(hitPos, hitNormal);
                timeToSpawnEffect = Time.time + 1 / effectSpawnRate;
            }

            produceSoundTriggerScript.SoundTrigger(soundRadius);
            audioManager.PlaySound(gunfireSoundName);

            DecreaseAmmo();
            DecreaseDurability();

            isCoolingDown = true;
            yield return new WaitForSeconds(coolDownTime);
            isCoolingDown = false;
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(initialWaitToShootTime);
        initialWaitToShootTime = 0f;
    }

    void Effect(Vector3 hitPos, Vector3 hitNormal)
    {
        CreateBulletTrail(hitPos, hitNormal);
        CreateMuzzleFlash();
    }

    void CreateMuzzleFlash()
    {
        Transform muzzleFlashClone = Instantiate(MuzzleFlashPrefab, firePoint.position, firePoint.rotation) as Transform; // Show muzzle flash
        muzzleFlashClone.parent = firePoint;
        float size = UnityEngine.Random.Range(muzzleFlashMinSize, muzzleFlashMaxSize); // Randomize size of muzzle flash
        muzzleFlashClone.localScale = new Vector3(size, size, size);
        Destroy(muzzleFlashClone.gameObject, 0.05f);
    }

    void CreateBulletTrail(Vector3 hitPos, Vector3 hitNormal)
    {
        Transform bulletTrail = Instantiate(BulletTrailPrefab, firePoint.position, firePoint.rotation) as Transform; // Show bullet trail
        LineRenderer lr = bulletTrail.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }
        Destroy(bulletTrail.gameObject, 0.05f);
    }

    void CreateHitParticle(Vector3 hitPos, Vector3 hitNormal)
    {
        if (hitNormal != new Vector3(9999, 9999, 9999))
        {
            Transform hitParticle = Instantiate(HitEffectPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal)) as Transform;
            Destroy(hitParticle.gameObject, 1f);
            // camShake.Shake(camShakeAmt, camShakeLength); // Creates cam shake on enemy hit (apply with heavy weapons such as rocket launchers, grenades, etc.)
        }
    }

    public void Reload()
    {
        currentAmmoAmount = clipSize;
    }

    public void DecreaseAmmo()
    {
        currentAmmoAmount--;
    }

    void DecreaseDurability()
    {
        durability -= Mathf.Round(durabilityUse * 100.0f) / 100.0f;
        if (durability < 25)
        {
            durability = 25;
        }
    }

    public void DetermineSoundName()
    {
        if (!isSilenced)
        {
            gunfireSoundName = name;
        }
        else if (isBoltAction)
        {
            gunfireSoundName = "Silenced Bolt-Action";
        }
        else if (isSniper)
        {
            gunfireSoundName = "Silenced Sniper"; // Same as silenced bolt-action sound, minus the reloading
        }
        else if (isShotgun && name == "Pump Shotgun")
        {
            gunfireSoundName = "Silenced Pump Shotgun";
        }
        else if (isShotgun)
        {
            gunfireSoundName = "Silenced Shotgun";
        }
        else
        {
            gunfireSoundName = "Silenced Gun";
        }
    }
}
