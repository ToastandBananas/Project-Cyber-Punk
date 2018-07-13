using UnityEngine;

public class Weapon : MonoBehaviour {

    public static Weapon instance;

    public LayerMask whatToHit;

    [Header("Stats:")]
    public float fireRate = 1;
    public int damage = 1;
    public string ammoType;
    public int clipSize;
    public bool isTwoHanded = false;
    public bool isShotgun = false;
    public bool isBoltAction = false;
    private bool canShoot = true;
    [Tooltip("Only for use with shotgun, rpg, and grenade launcher.")]
    public float coolDownTime = 1f; // For shotgun, rpg, and grenade launcher

    [Header("Actual size will depend on parent object size:")]
    public int soundRadius = 10000;

    [Header("Effects:")]
    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitEffectPrefab;
    public int muzzleFlashMinSize = 7;
    public int muzzleFlashMaxSize = 10;

    float timeToFire = 0;
    float timeToSpawnEffect = 0;
    [HideInInspector]public float effectSpawnRate = 10;
    Transform firePoint;
    
    // Sound
    private AudioManager audioManager;
    string gunfireSoundName;

    PlayerController playerController;
    Player player;
    ProduceSoundTrigger produceSoundTriggerScript;
    
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

        gunfireSoundName = this.name;

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
        produceSoundTriggerScript = gameObject.GetComponent<ProduceSoundTrigger>();

        // Sound
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }
    }
    
    void Update () {
        PlayerCheckIfShooting();
    }

    void PlayerCheckIfShooting()
    {
        if (Input.GetButton("Fire2") && player.isDead == false) // Holding right click
        {
            Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            if (Vector2.Distance(mousePosition, player.transform.position) > 0.4)
            {
                if (fireRate == 1)
                {
                    // print("fire rate is 1");
                    if (Input.GetButtonDown("Fire1") && canShoot) // Left click while holding right click
                    {
                        Shoot();
                        if (isShotgun || isBoltAction)
                        {
                            canShoot = false;
                            Invoke("CooledDown", coolDownTime);
                        }
                        produceSoundTriggerScript.SoundTrigger(soundRadius);
                        audioManager.PlaySound(gunfireSoundName);
                    }
                }
                else
                {
                    if (Input.GetButton("Fire1") && Time.time > timeToFire) // For automatic guns
                    {
                        timeToFire = Time.time + 1 / fireRate;
                        Shoot();
                        produceSoundTriggerScript.SoundTrigger(soundRadius);
                        audioManager.PlaySound(gunfireSoundName);
                    }
                }
            }
        }
    }

    void CooledDown()
    {
        canShoot = true;
    }

    void Shoot()
    {
        Vector2 mousePosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        Vector2 firePointPosition = new Vector2(firePoint.position.x, firePoint.position.y);
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, (mousePosition - firePointPosition) + new Vector2(0, Random.Range(-player.playerStats.accuracyFactor, player.playerStats.accuracyFactor) * Vector2.Distance(mousePosition, firePointPosition)), 100f, whatToHit);

        Vector3 hitPos;
        Vector3 hitNormal;

        // Debug.DrawLine(firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);
        
        if (hit.collider != null)
        {
            // Debug.DrawLine(firePointPosition, hit.point, Color.red);
            Enemy enemyCollider = hit.collider.GetComponent<Enemy>();
            if (enemyCollider != null)
            {
                hitPos = hit.point;
                hitNormal = hit.normal;

                if (isShotgun)
                {
                    if (Vector2.Distance(firePointPosition, hitPos) >= 3.2 && Vector2.Distance(firePointPosition, hitPos) < 5)
                    {
                        enemyCollider.DamageEnemy(damage / 2);
                        // Debug.Log("We hit " + hit.collider.name + " and did " + damage / 2 + " damage.");
                    }
                    else if (Vector2.Distance(firePointPosition, hitPos) >= 5)
                    {
                        enemyCollider.DamageEnemy(1);
                        // Debug.Log("We hit " + hit.collider.name + " and did " + 1 + " damage.");
                    }
                    else
                    {
                        enemyCollider.DamageEnemy(damage);
                        // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                    }
                }
                else
                {
                    enemyCollider.DamageEnemy(damage);
                    // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                }
                
                CreateHitParticle(hitPos, hitNormal);
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
        }
    }
}
