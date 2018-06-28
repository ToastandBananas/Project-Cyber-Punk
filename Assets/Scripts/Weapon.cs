using UnityEngine;

public class Weapon : MonoBehaviour {

    public static Weapon instance;
    
    public float fireRate = 1;
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

    PlayerController playerController;
    Player player;

    // Use this for initialization
    void Awake () {
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

    void Start()
    {
        playerController = PlayerController.instance;
        player = Player.instance;

        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("No CameraShake script found on GM object.");
        }

        // Caching
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }
    }

    // Update is called once per frame
    void Update () {
        CheckIfShooting();
    }

    void CheckIfShooting()
    {
        if (Input.GetButton("Fire2")) // Holding right click
        {
            if (fireRate == 1)
            {
                print("fire rate is 1");
                if (Input.GetButtonDown("Fire1")) // Left click while holding right click
                {
                    Shoot();
                    audioManager.PlaySound(gunfireSoundName);
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && Time.time > timeToFire) // For automatic guns
                {
                    timeToFire = Time.time + 1 / fireRate;
                    Shoot();
                    audioManager.PlaySound(gunfireSoundName);
                }
            }
        }
    }

    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, (mousePosition - firePointPosition) + new Vector2(0, Random.Range(-player.playerStats.accuracyFactor, player.playerStats.accuracyFactor)), 100f, whatToHit);

        Vector3 hitPos;
        Vector3 hitNormal;

        // Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);

        if (hit.collider != null)
        {
            // Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Enemy enemyCollider = hit.collider.GetComponent<Enemy>();
            if(enemyCollider != null)
            {
                hitPos = hit.point;
                hitNormal = hit.normal;

                enemyCollider.DamageEnemy(damage);
                CreateHitParticle(hitPos, hitNormal);
                // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
            }
        }

        if (Time.time >= timeToSpawnEffect)
        {
            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 9999;
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
