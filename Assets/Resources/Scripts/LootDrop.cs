using UnityEngine;

public class LootDrop : MonoBehaviour
{
    GameObject enemyWeapon;
    GameObject weaponToDrop;
    GameObject gadgetToDrop = null;
    GameObject cashToDrop;

    GameObject enemyArm;

    Transform enemy;

    float distanceToWeaponDrop;
    
	void Start () {
        enemy = GetComponent<Enemy>().transform;
        enemyArm = transform.Find("EnemyArm").gameObject;
	}

    public void DropWeapon(int currentAmmoAmount, int clipSize, string ammoType, float damage, float fireRate, bool isSilenced, bool hasIncreasedClipSize, float clipSizeMultiplier, float inaccuracyFactor)
    {
        enemyWeapon = enemyArm.transform.GetChild(0).gameObject;
        weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + enemyWeapon.name + " Item Drop") as GameObject;

        GameObject droppedWeapon = Instantiate(weaponToDrop);
        
        droppedWeapon.transform.position = enemy.position + new Vector3(0, .2f);

        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().currentAmmoAmount = currentAmmoAmount;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().clipSize = clipSize;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().ammoType = ammoType; // Need to know ammo type and amount for when player picks up ammo
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().damage = damage;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().fireRate = fireRate; // Need to know damage and fire rate so that the values are the same when player picks the weapon up
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().isSilenced = isSilenced;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().hasIncreasedClipSize = hasIncreasedClipSize;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().clipSizeMultiplier = clipSizeMultiplier;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().inaccuracyFactor = inaccuracyFactor;
    }
    
    //To Do: Randomize gadget drop
}
