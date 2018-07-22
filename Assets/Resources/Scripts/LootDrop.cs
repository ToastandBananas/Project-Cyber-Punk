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

    public void DropWeapon()
    {
        GameObject droppedWeapon = Instantiate(weaponToDrop);
        droppedWeapon.transform.position = enemy.position + new Vector3(0, .2f);
    }

    //To Do: Randomize cash drop
    //To Do: Randomize gadget drop
}
