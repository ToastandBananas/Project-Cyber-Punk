using UnityEngine;
using UnityEngine.UI;

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
                if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot1.transform.GetChild(2).name)
                {
                    hotbarScript.currentlyEquippedWeaponSlot = 1;
                    hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.equippedColor;
                    hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.unequippedColor;
                }
                else if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot2.transform.GetChild(2).name)
                {
                    hotbarScript.currentlyEquippedWeaponSlot = 2;
                    hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.unequippedColor;
                    hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.equippedColor;
                }

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
