using UnityEngine;

public class LootDrop : MonoBehaviour
{
    GameObject enemyWeapon;
    GameObject weaponToDrop;
    GameObject gadgetToDrop = null;
    GameObject cashToDrop;

    GameObject enemyArm;

    Transform enemy;

	// Use this for initialization
	void Start () {
        enemy = GetComponent<Enemy>().transform;
        enemyArm = transform.Find("EnemyArm").gameObject;
        enemyWeapon = enemyArm.transform.GetChild(0).gameObject;
        weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + enemyWeapon.name + " Item Drop") as GameObject;
	}

    public void DropWeapon(int currentAmmoAmount, string ammoType, float damage, float fireRate)
    {
        GameObject droppedWeapon = Instantiate(weaponToDrop);
        droppedWeapon.transform.position = enemy.position + new Vector3(0, .2f);
        currentAmmoAmount = enemyWeapon.GetComponent<EnemyWeapon>().currentAmmoAmount;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().currentAmmoAmount = currentAmmoAmount;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().ammoType = ammoType; // Need to know ammo type and amount for when player picks up ammo
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().damage = damage;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().fireRate = fireRate; // Need to know damage and fire rate so that the values are the same when player picks the weapon up
    }

    //To Do: Randomize cash drop
    //To Do: Randomize gadget drop
}
