using System.Collections;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour {

    public static EnemyWeapon instance;

    Enemy enemy;
    EnemySenses enemySenses;
    EnemyMovement enemyMovement;

    public Transform target;

    public int damage = 1;
    public LayerMask whatToHit;

    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitEffectPrefab;
    
    float timeToFire = 0;
    float timeToSpawnEffect = 0;
    public float effectSpawnRate = 10;
    Transform firePoint;

    // Handle camera shaking
    public float camShakeAmt = 0.05f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

    // Cache
    private AudioManager audioManager;
    public string gunfireSoundName;

    public float bulletMoveSpeed = 20f;
    public float maxShootDistance = 5.5f;

    public bool isAutomatic = false;
    bool isAbleToShoot = false;
    bool isCoolingDown = false;

    public float initialWaitToShootTime;
    public float semiAutoCoolDownTime = 0.75f;
    public float autoCoolDownTime = .2f;
    float coolDownTime;

    Rigidbody2D rb;

    Vector2 bulletMoveDirection;

    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            instance = this;
        }

        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("No fire point...Please make an empty object named FirePoint and attach to end of gun. :)");
        }
    }

    // Use this for initialization
    void Start () {
        enemySenses = EnemySenses.instance;
        enemyMovement = EnemyMovement.instance;
        enemy = Enemy.instance;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }

        // bulletMoveDirection = (target.position - transform.position).normalized * bulletMoveSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        CheckIfShooting();

        if (enemyMovement.stillSearching == false)
        {
            initialWaitToShootTime = 2f;
        }
    }

    void CheckIfShooting()
    {
        if (enemySenses.CanPlayerBeSeen() == true && enemy.distanceToPlayer <= maxShootDistance)
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
                StartCoroutine(Shoot());
            }
            else if (isAutomatic == true) // For automatic guns
            {
                coolDownTime = autoCoolDownTime;
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        StartCoroutine(Wait());

        if (initialWaitToShootTime == 0)
        {
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
        float size = UnityEngine.Random.Range(6f, 9f); // Randomize size of muzzle flash
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
