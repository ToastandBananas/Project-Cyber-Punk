using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    GameObject weaponSlot1;
    GameObject weaponSlot2;

    public Material highlightMaterial;
    public Material defaultMaterial;

    Hotbar hotbarScript;
    ItemDatabase itemDatabase;

    Item item;

    GameObject playerArm;

    GameObject weaponPrefab;
    public int weaponID;

    void Start()
    {
        weaponSlot1 = GameObject.Find("WeaponSlot1");
        weaponSlot2 = GameObject.Find("WeaponSlot2");
        
        hotbarScript = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();

        playerArm = GameObject.Find("Arm");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = highlightMaterial;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                hotbarScript.EquipWeapon(weaponID);
                hotbarScript.AddItemToInventory(weaponID);
                hotbarScript.currentlyEquippedWeapon = Resources.Load("Prefabs/Items/PlayerWeapons/" + gameObject.name) as GameObject;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = defaultMaterial;
        }
    }
}
