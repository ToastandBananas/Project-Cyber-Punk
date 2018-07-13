﻿using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

    Enemy enemyScript;
    EnemySight enemySightScript;
    EnemyMovement enemyMovementScript;
    Player player;
    Transform enemy;
    Transform enemySight;

    public LayerMask whatToHit;

    [Header("Stats:")]
    public int damage = 1;
    public string ammoType;
    public int clipSize;
    public bool isTwoHanded = false;

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

    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No fire point...Please make an empty object named FirePoint and attach to end of gun. :)");
        }
        gunfireSoundName = this.name;
    }

    // Use this for initialization
    void Start () {
        enemy = transform.root;
        enemySight = enemy.Find("Sight");

        enemySightScript = enemySight.GetComponent<EnemySight>();
        enemyMovementScript = enemy.GetComponent<EnemyMovement>();
        enemyScript = enemy.GetComponent<Enemy>();
        player = Player.instance;

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }
	}
	
	// Update is called once per frame
	void Update () {
        EnemyCheckIfShooting();

        if (enemyMovementScript.stillSearching == false)
        {
            initialWaitToShootTime = 2f;
        }
    }

    void EnemyCheckIfShooting()
    {
        if (enemyScript.isDead == false)
        {
            if (enemySightScript.CanPlayerBeSeen() == true && enemyScript.distanceToPlayer <= maxShootDistance)
            {
                isAbleToShoot = true;
            }
            else
            {
                isAbleToShoot = false;
            }

            if (isCoolingDown == false && isAbleToShoot == true)
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

        if (enemyScript.distanceToPlayer < 2f)
        {
            hit = Physics2D.Raycast(firePointPosition, (playerPosition - firePointPosition), 100f, whatToHit);
        }
        else
        {
            hit = Physics2D.Raycast(firePointPosition, (playerPosition - firePointPosition) + new Vector2(0, Random.Range(-enemyScript.enemyStats.accuracyFactor, enemyScript.enemyStats.accuracyFactor)), 100f, whatToHit);
        }

        Vector3 hitPos;
        Vector3 hitNormal;

        if (initialWaitToShootTime == 0)
        {
            if (hit.collider != null)
            {
                Player playerCollider = hit.collider.GetComponent<Player>();
                if (playerCollider != null)
                {
                    hitPos = hit.point;
                    hitNormal = hit.normal;

                    if (isShotgun)
                    {
                        if (Vector2.Distance(firePointPosition, hitPos) >= 3.2 && Vector2.Distance(firePointPosition, hitPos) < 5)
                        {
                            playerCollider.DamagePlayer(damage / 2);
                            // Debug.Log("We hit " + hit.collider.name + " and did " + damage / 2 + " damage.");
                        }
                        else if (Vector2.Distance(firePointPosition, hitPos) >= 5)
                        {
                            playerCollider.DamagePlayer(1);
                            // Debug.Log("We hit " + hit.collider.name + " and did " + 1 + " damage.");
                        }
                        else
                        {
                            playerCollider.DamagePlayer(damage);
                            // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                        }
                    }
                    else
                    {
                        playerCollider.DamagePlayer(damage);
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

            audioManager.PlaySound(gunfireSoundName);

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
}
