﻿using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class Weapon : MonoBehaviour 
{
    public int weaponID;
    public LayerMask whatToHit;

    [Header("Stats:")]
    public float inaccuracyFactor = 0; // 0 equals 100 percent accurate
    public float finalAccuracyFactor;
    public float fireRate;
    public float damage;
    public string ammoType;
    public int clipSize;
    public int currentAmmoAmount;
    public string actionType;
    public bool isTwoHanded = false;
    public bool isShotgun = false;
    public bool isBoltAction = false;
    public bool isSniper = false;
    private bool canShoot = true;
    [Tooltip("Only for use with shotgun, rpg, and grenade launcher.")]
    public float coolDownTime = 1f; // For shotgun

    [Header("Actual size will depend on parent object size:")]
    public int soundRadius;

    [Header("Perk Variables:")]
    public bool isSilenced = false;
    public bool hasIncreasedClipSize = false;
    public float clipSizeMultiplier;
    public bool hasAlteredInaccuracyFactor = false;

    [Header("Effects:")]
    public Transform BulletTrailPrefab;
    public Transform MuzzleFlashPrefab;
    public Transform HitEffectPrefab;
    public Sprite aimCursorSprite;
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
    ItemDatabase itemDatabase;
    WeaponPerks weaponPerksScript;
    
    void Awake () {
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
        produceSoundTriggerScript = GetComponent<ProduceSoundTrigger>();
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();
        weaponPerksScript = GetComponent<WeaponPerks>();

        // Sound
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No AudioManager found in the scene");
        }

        Item weaponItem = itemDatabase.FetchItemByID(weaponID);
        name = weaponItem.ItemName;
        ammoType = weaponItem.AmmoType;
        actionType = weaponItem.ActionType;
        soundRadius = weaponItem.SoundRadius;

        if (player.hasEquippedStartingWeapon == false) // This section is only for the players starting weapon, otherwise these stats will be determined by the WeaponPickup script
        {
            weaponPerksScript.RandomizePerks(weaponID);
            damage = Mathf.Round(Random.Range(weaponItem.MinDamage, weaponItem.MaxDamage) * 100.0f) / 100.0f;
            fireRate = Mathf.Round(Random.Range(weaponItem.MinFireRate, weaponItem.MaxFireRate) * 100.0f) / 100.0f;
            //clipSize is set in the WeaponPerks script
            currentAmmoAmount = clipSize;

            finalAccuracyFactor = player.playerStats.inaccuracyFactor + inaccuracyFactor;
            if (finalAccuracyFactor < 0)
            {
                finalAccuracyFactor = 0;
            }

            player.hasEquippedStartingWeapon = true;
        }

        DetermineSoundName();
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
                    if (Input.GetButtonDown("Fire1") && canShoot && currentAmmoAmount > 0) // Left click while holding right click
                    {
                        Shoot();
                        DecreaseAmmo();
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
                    if (Input.GetButton("Fire1") && Time.time > timeToFire && currentAmmoAmount > 0) // For automatic guns
                    {
                        timeToFire = Time.time + 1 / fireRate;
                        Shoot();
                        DecreaseAmmo();
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
        RaycastHit2D hit = Physics2D.Raycast(firePointPosition, (mousePosition - firePointPosition) + new Vector2(0, Random.Range(-finalAccuracyFactor, finalAccuracyFactor) * Vector2.Distance(mousePosition, firePointPosition)), 100f, whatToHit);

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

                if (hit.transform.GetComponent<Enemy>().isDead == false) // If enemy is alive
                {
                    if (isShotgun)
                    {
                        hit.transform.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
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
                        hit.transform.GetComponent<EnemyMovement>().currentState = EnemyMovement.State.Attack;
                        enemyCollider.DamageEnemy(damage);
                        // Debug.Log("We hit " + hit.collider.name + " and did " + damage + " damage.");
                    }
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

    public void IncreaseAmmo(int amountToAdd)
    {
        currentAmmoAmount += amountToAdd;
    }

    public void DecreaseAmmo()
    {
        currentAmmoAmount--;
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
            gunfireSoundName = "Silenced Sniper";
        }
        else if (isShotgun && coolDownTime > 0)
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
