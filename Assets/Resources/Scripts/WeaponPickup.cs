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
    Weapon playerEquippedWeapon;
    GameObject playerSecondaryWeapon;

    GameObject weaponPrefab;
    public int weaponID;

    public string ammoType;
    public int currentAmmoAmount;
    int roomInPlayersEquippedWeaponClip;
    int roomInPlayersSecondaryWeaponClip;

    void Start()
    {
        weaponSlot1 = GameObject.Find("WeaponSlot1");
        weaponSlot2 = GameObject.Find("WeaponSlot2");
        
        hotbarScript = GameObject.Find("Hotbar").GetComponent<Hotbar>();
        itemDatabase = GameObject.Find("Hotbar").GetComponent<ItemDatabase>();

        playerArm = GameObject.Find("Arm");
        playerEquippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerSecondaryWeapon = hotbarScript.secondaryWeapon;
    }

    void Update()
    {
        playerEquippedWeapon = GameObject.FindGameObjectWithTag("EquippedWeapon").GetComponent<Weapon>();
        playerSecondaryWeapon = hotbarScript.secondaryWeapon;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GetComponent<SpriteRenderer>().material = highlightMaterial;

            if (playerEquippedWeapon.currentAmmoAmount < playerEquippedWeapon.clipSize && playerEquippedWeapon.ammoType == ammoType)
            {
                roomInPlayersEquippedWeaponClip = playerEquippedWeapon.clipSize - playerEquippedWeapon.currentAmmoAmount;

                while (this.currentAmmoAmount > 0 && roomInPlayersEquippedWeaponClip > 0)
                {
                    this.currentAmmoAmount--;
                    playerEquippedWeapon.IncreaseAmmo(1);
                    roomInPlayersEquippedWeaponClip--;
                }
            }
            else if (hotbarScript.secondaryWeaponAmmoAmount < hotbarScript.secondaryWeaponClipSize && hotbarScript.secondaryWeaponAmmoType == ammoType)
            {
                roomInPlayersSecondaryWeaponClip = playerSecondaryWeapon.GetComponent<Weapon>().clipSize - playerSecondaryWeapon.GetComponent<Weapon>().currentAmmoAmount;

                while (this.currentAmmoAmount > 0 && roomInPlayersSecondaryWeaponClip > 0)
                {
                    this.currentAmmoAmount--;
                    playerSecondaryWeapon.GetComponent<Weapon>().currentAmmoAmount++;
                    roomInPlayersSecondaryWeaponClip--;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (playerEquippedWeapon.name != this.name && (playerSecondaryWeapon == null || playerSecondaryWeapon.name != this.name)) // The player may not have two of the same weapon
                {
                    hotbarScript.secondaryWeaponAmmoAmount = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                    hotbarScript.secondaryWeaponClipSize = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                    hotbarScript.secondaryWeaponAmmoType = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;
                    hotbarScript.secondaryWeapon = hotbarScript.currentlyEquippedWeapon;

                    hotbarScript.EquipWeapon(weaponID);
                    hotbarScript.AddItemToInventory(weaponID);

                    foreach (GameObject weaponObject in hotbarScript.weaponObjects)
                    {
                        if (weaponObject.name == this.name)
                        {
                            hotbarScript.currentlyEquippedWeapon = weaponObject;
                        }
                    }

                    if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot1.transform.GetChild(2).name)
                    {
                        hotbarScript.currentlyEquippedWeaponSlot = 1;
                        GameObject.Find("AmmoText1").GetComponent<Text>().text = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                        hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.equippedColor;
                        hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.unequippedColor;
                    }
                    else if (hotbarScript.currentlyEquippedWeapon.name == weaponSlot2.transform.GetChild(2).name)
                    {
                        hotbarScript.currentlyEquippedWeaponSlot = 2;
                        GameObject.Find("AmmoText2").GetComponent<Text>().text = hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + hotbarScript.currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                        hotbarScript.weaponSlot1.GetComponent<Image>().color = hotbarScript.unequippedColor;
                        hotbarScript.weaponSlot2.GetComponent<Image>().color = hotbarScript.equippedColor;
                    }

                    Destroy(gameObject);
                }
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
