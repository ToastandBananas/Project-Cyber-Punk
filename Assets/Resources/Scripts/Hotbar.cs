using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public int startingWeaponID;

    public GameObject weaponSlot1;
    public GameObject weaponSlot2;
    public GameObject gadgetSlot1;
    public GameObject gadgetSlot2;

    public GameObject gadgetItem;
    public GameObject weaponItem;

    public GameObject currentlyEquippedWeapon;
    public GameObject secondaryWeapon;
    public int currentlyEquippedWeaponSlot = 1;
    public int secondaryWeaponAmmoAmount;
    public int secondaryWeaponClipSize;
    public string secondaryWeaponAmmoType;

    Weapon currentWeaponScript;

    public GameObject ammoText1;
    public GameObject ammoText2;

    GameObject infoPanel;
    GameObject weaponToDrop;

    Player player;

    public GameObject[] weaponObjects;

    ItemDatabase itemDatabase;
    LootDrop lootDropScript;

    public Color32 equippedColor = new Color32(138, 181, 246, 255);
    public Color32 unequippedColor = new Color32(255, 255, 255, 255);
    public Color32 brokenColor = new Color32(236, 92, 97, 255);

    public List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        itemDatabase = GetComponent<ItemDatabase>();
        
        infoPanel = GameObject.Find("InfoPanel");
        infoPanel.SetActive(false);
        player = Player.instance;

        AddItemToInventory(startingWeaponID);
        AddItemToInventory(34);

        weaponObjects = GameObject.FindGameObjectsWithTag("EquippedWeapon");
        foreach (GameObject weaponObject in weaponObjects)
        {
            if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)
            {
                weaponObject.SetActive(true);
                weaponObject.tag = "EquippedWeapon";
                currentlyEquippedWeapon = weaponObject;
                currentWeaponScript = currentlyEquippedWeapon.GetComponent<Weapon>();
                currentlyEquippedWeaponSlot = 1;
                weaponSlot1.GetComponent<Image>().color = equippedColor;
            }
            else
            {
                weaponObject.SetActive(false);
                weaponObject.tag = "InactiveWeapon";
            }
        }
    }

    void FixedUpdate()
    {
        SwapWeapon();
        DisplayAmmo();

        if (secondaryWeapon != null)
        {
            secondaryWeapon.tag = "SecondaryWeapon";
            if (secondaryWeapon.GetComponent<Weapon>().durability <= 0)
                secondaryWeapon.GetComponent<Weapon>().isBroken = true;
        }

        SetBrokenWeaponSlotColor();

        currentWeaponScript = currentlyEquippedWeapon.GetComponent<Weapon>();
        // print("Currently equipped weapon: " + currentlyEquippedWeapon);
        // print("Secondary weapon: " + secondaryWeapon);
        // print("Current weapon slot: " + currentlyEquippedWeaponSlot);
    }

    void SetBrokenWeaponSlotColor()
    {
        if (currentlyEquippedWeaponSlot == 1)
        {
            if (currentlyEquippedWeapon.GetComponent<Weapon>().isBroken)
                weaponSlot1.GetComponent<Image>().color = brokenColor;

            if (secondaryWeapon != null && secondaryWeapon.GetComponent<Weapon>().isBroken)
                weaponSlot2.GetComponent<Image>().color = brokenColor;
        }
        else if (currentlyEquippedWeaponSlot == 2)
        {
            if (currentlyEquippedWeapon.GetComponent<Weapon>().isBroken)
                weaponSlot2.GetComponent<Image>().color = brokenColor;

            if (secondaryWeapon != null && secondaryWeapon.GetComponent<Weapon>().isBroken)
                weaponSlot1.GetComponent<Image>().color = brokenColor;
        }
    }

    public void EquipWeapon(int id)
    {
        Item itemToAdd = itemDatabase.FetchItemByID(id);

        foreach (GameObject weaponObject in weaponObjects)
        {
            if (weaponObject.name == itemToAdd.ItemName)
            {
                weaponObject.SetActive(true);
                weaponObject.tag = "EquippedWeapon";

                if (weaponSlot1.GetComponent<Slot>().isEmpty == false && weaponSlot2.GetComponent<Slot>().isEmpty == false)
                {
                    weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + currentlyEquippedWeapon.name + " Item Drop") as GameObject;
                    // Currently equipped weapon and slot and secondary weapon ammo amount are set in the WeaponPickup script
                    DropWeapon(weaponToDrop, currentWeaponScript.currentAmmoAmount, currentWeaponScript.clipSize, currentWeaponScript.ammoType, currentWeaponScript.damage, currentWeaponScript.fireRate, currentWeaponScript.isSilenced, 
                                currentWeaponScript.hasIncreasedClipSize, currentWeaponScript.clipSizeMultiplier, currentWeaponScript.inaccuracyFactor, currentWeaponScript.durability, currentWeaponScript.hasAlteredDurability, 
                                currentWeaponScript.durabilityMultiplier);
                }
            }
            else
            {
                weaponObject.SetActive(false);
                weaponObject.tag = "InactiveWeapon";
            }
        }
    }

    public void AddItemToInventory(int id)
    {
        Item itemToAdd = itemDatabase.FetchItemByID(id);

        if (itemToAdd.Type == Item.ItemType.Gun)
        {
            if (weaponSlot1.GetComponent<Slot>().isEmpty)
            {
                GameObject weaponObject = Instantiate(weaponItem);

                weaponObject.GetComponent<ItemData>().item = itemToAdd;
                weaponObject.transform.SetParent(weaponSlot1.transform);
                weaponObject.transform.position = weaponSlot1.transform.position;
                weaponObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                weaponObject.name = itemToAdd.ItemName;

                weaponSlot1.GetComponent<Slot>().isEmpty = false;
            }
            else if (weaponSlot2.GetComponent<Slot>().isEmpty)
            {
                GameObject weaponObject = Instantiate(weaponItem);

                weaponObject.GetComponent<ItemData>().item = itemToAdd;
                weaponObject.transform.SetParent(weaponSlot2.transform);
                weaponObject.transform.position = weaponSlot2.transform.position;
                weaponObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                weaponObject.name = itemToAdd.ItemName;

                weaponSlot2.GetComponent<Slot>().isEmpty = false;
            }
            else if (currentlyEquippedWeaponSlot == 1) // If currently equipped weapon is in weapon slot 1
            {
                GameObject weaponObject = Instantiate(weaponItem);

                weaponObject.GetComponent<ItemData>().item = itemToAdd;
                weaponObject.transform.SetParent(weaponSlot1.transform);
                weaponObject.transform.position = weaponSlot1.transform.position;
                weaponObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                weaponObject.name = itemToAdd.ItemName;

                weaponSlot1.GetComponent<Slot>().isEmpty = false;

                Destroy(weaponSlot1.transform.GetChild(2).gameObject);
            }
            else if (currentlyEquippedWeaponSlot == 2) // If currently equipped weapon is in weapon slot 2
            {
                GameObject weaponObject = Instantiate(weaponItem);

                weaponObject.GetComponent<ItemData>().item = itemToAdd;
                weaponObject.transform.SetParent(weaponSlot2.transform);
                weaponObject.transform.position = weaponSlot2.transform.position;
                weaponObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                weaponObject.name = itemToAdd.ItemName;

                weaponSlot2.GetComponent<Slot>().isEmpty = false;

                Destroy(weaponSlot2.transform.GetChild(2).gameObject);
            }
            else
            {
                Debug.LogError("Error: You don't have an empty weapon slot and your equipped weapon doesn't match any weapon in the hotbar.");
            }
        }
        else if (itemToAdd.Type == Item.ItemType.Gadget)
        {
            if (gadgetSlot1.GetComponent<Slot>().isEmpty)
            {
                GameObject gadgetObject = Instantiate(gadgetItem);

                gadgetObject.GetComponent<ItemData>().item = itemToAdd;
                gadgetObject.transform.SetParent(gadgetSlot1.transform);
                gadgetObject.transform.position = gadgetSlot1.transform.position;
                gadgetObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                gadgetObject.name = itemToAdd.ItemName;

                gadgetSlot1.GetComponent<Slot>().isEmpty = false;
            }
            else if (gadgetSlot2.GetComponent<Slot>().isEmpty)
            {
                GameObject gadgetObject = Instantiate(gadgetItem);

                gadgetObject.GetComponent<ItemData>().item = itemToAdd;
                gadgetObject.transform.SetParent(gadgetSlot2.transform);
                gadgetObject.transform.position = gadgetSlot2.transform.position;
                gadgetObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                gadgetObject.name = itemToAdd.ItemName;

                gadgetSlot2.GetComponent<Slot>().isEmpty = false;
            }
            else // Else tell player that their inventory is full
            {
                infoPanel.transform.GetChild(0).GetComponent<Text>().text = "Not enough room in your inventory...";
                infoPanel.SetActive(true);
                Invoke("HideInfoText", 2.5f);
            }
        }
    }

    void HideInfoText()
    {
        infoPanel.SetActive(false);
    }

    public void DropWeapon(GameObject weaponToDrop, int currentAmmoAmount, int clipSize, string ammoType, float damage, float fireRate, bool isSilenced, bool hasIncreasedClipSize, float clipSizeMultiplier, 
                            float inaccuracyFactor, float durability, bool hasAlteredDurability, float durabilityMultiplier)
    {
        GameObject droppedWeapon = Instantiate(weaponToDrop);
        droppedWeapon.transform.position = player.transform.position + new Vector3(0, .2f);
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().currentAmmoAmount = currentAmmoAmount;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().clipSize = clipSize;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().ammoType = ammoType;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().damage = damage;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().fireRate = fireRate;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().isSilenced = isSilenced;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().hasIncreasedClipSize = hasIncreasedClipSize;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().clipSizeMultiplier = clipSizeMultiplier;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().inaccuracyFactor = inaccuracyFactor;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().durability = durability;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().hasAlteredDurability = hasAlteredDurability;
        droppedWeapon.transform.GetChild(0).GetComponent<WeaponPickup>().durabilityMultiplier = durabilityMultiplier;
    }

    void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (weaponSlot1.transform.childCount == 3 && currentlyEquippedWeaponSlot != 1) // If weapon slot 1 isn't empty and current weapon slot isn't already equal to 1
            {
                TransferAmmo();

                foreach (GameObject weaponObject in weaponObjects)
                {
                    // if the weapon in the object pool equals the weapon in slot 1
                    if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)
                    {
                        currentlyEquippedWeaponSlot = 1;
                        secondaryWeaponAmmoAmount = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                        secondaryWeaponClipSize = currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                        secondaryWeaponAmmoType = currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;
                        secondaryWeapon = currentlyEquippedWeapon;
                        secondaryWeapon.tag = "SecondaryWeapon";

                        weaponObject.SetActive(true);
                        weaponObject.tag = "EquippedWeapon";
                        currentlyEquippedWeapon = weaponObject;
                        
                        weaponSlot1.GetComponent<Image>().color = equippedColor;
                        weaponSlot2.GetComponent<Image>().color = unequippedColor;
                    }
                    else if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)
                    {
                        // print("This weapon is already equipped");
                        break;
                    }
                    else
                    {
                        weaponObject.SetActive(false);
                        weaponObject.tag = "InactiveWeapon";
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (weaponSlot2.transform.childCount == 3 && currentlyEquippedWeaponSlot != 2) // If weapon slot 2 isn't empty
            {
                TransferAmmo();

                foreach (GameObject weaponObject in weaponObjects)
                {
                    // if the weapon in the object pool equals the weapon in slot 2 
                    if (weaponObject.name == weaponSlot2.transform.GetChild(2).name)
                    {
                        currentlyEquippedWeaponSlot = 2;
                        secondaryWeaponAmmoAmount = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                        secondaryWeaponClipSize = currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                        secondaryWeaponAmmoType = currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;
                        secondaryWeapon = currentlyEquippedWeapon;
                        secondaryWeapon.tag = "SecondaryWeapon";
                        
                        weaponObject.SetActive(true);
                        weaponObject.tag = "EquippedWeapon";
                        
                        currentlyEquippedWeapon = weaponObject;

                        weaponSlot1.GetComponent<Image>().color = unequippedColor;
                        weaponSlot2.GetComponent<Image>().color = equippedColor;
                    }
                    else if (weaponObject.name == weaponSlot2.transform.GetChild(2).name)
                    {
                        // print("This weapon is already equipped");
                        break;
                    }
                    else
                    {
                        weaponObject.SetActive(false);
                        weaponObject.tag = "InactiveWeapon";
                    }
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (weaponSlot1.transform.childCount == 3 && weaponSlot2.transform.childCount == 3) // If both weapon slots are occupied
            {
                if (currentlyEquippedWeaponSlot == 1) // If currently equipped weapon is in weapon slot 1
                {
                    TransferAmmo();
                    foreach (GameObject weaponObject in weaponObjects)
                    {
                        if (weaponObject.name == weaponSlot2.transform.GetChild(2).name) // If the weapon in the object pool equals the weapon in weapon slot 2
                        {
                            currentlyEquippedWeaponSlot = 2;
                            secondaryWeaponAmmoAmount = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                            secondaryWeaponClipSize = currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                            secondaryWeaponAmmoType = currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;
                            secondaryWeapon = currentlyEquippedWeapon;
                            secondaryWeapon.tag = "SecondaryWeapon";
                            
                            weaponObject.SetActive(true);
                            weaponObject.tag = "EquippedWeapon";
                            currentlyEquippedWeapon = weaponObject;
                            
                            weaponSlot1.GetComponent<Image>().color = unequippedColor;
                            weaponSlot2.GetComponent<Image>().color = equippedColor;
                        }
                        else
                        {
                            weaponObject.SetActive(false);
                            weaponObject.tag = "InactiveWeapon";
                        }
                    }
                }
                else if (currentlyEquippedWeaponSlot == 2) // If currently equipped weapon is in weapon slot 2
                {
                    TransferAmmo();
                    foreach (GameObject weaponObject in weaponObjects)
                    {
                        if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)  // If the weapon in the object pool equals the weapon in weapon slot 1
                        {
                            currentlyEquippedWeaponSlot = 1;
                            secondaryWeaponAmmoAmount = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount;
                            secondaryWeaponClipSize = currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
                            secondaryWeaponAmmoType = currentlyEquippedWeapon.GetComponent<Weapon>().ammoType;
                            secondaryWeapon = currentlyEquippedWeapon;
                            secondaryWeapon.tag = "SecondaryWeapon";

                            weaponObject.SetActive(true);
                            weaponObject.tag = "EquippedWeapon";
                            currentlyEquippedWeapon = weaponObject;
                            
                            weaponSlot1.GetComponent<Image>().color = equippedColor;
                            weaponSlot2.GetComponent<Image>().color = unequippedColor;
                        }
                        else
                        {
                            weaponObject.SetActive(false);
                            weaponObject.tag = "InactiveWeapon";
                        }
                    }
                }
            }
        }
    }

    private void TransferAmmo()
    {
        if (currentWeaponScript != null && secondaryWeapon != null)
        {
            if (currentWeaponScript.ammoType == secondaryWeapon.GetComponent<Weapon>().ammoType
                && currentWeaponScript.currentAmmoAmount > 0
                && secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount < secondaryWeapon.GetComponent<Weapon>().clipSize)
            {
                while (currentWeaponScript.currentAmmoAmount > 0 && secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount < secondaryWeapon.GetComponent<Weapon>().clipSize)
                {
                    currentWeaponScript.currentAmmoAmount--;
                    secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount++;
                }
            }
        }
    }

    void DisplayAmmo()
    {
        if (currentlyEquippedWeaponSlot == 1)
        {
            ammoText1.GetComponent<Text>().text = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
            if (secondaryWeapon != null)
            {
                ammoText2.GetComponent<Text>().text = secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + secondaryWeapon.GetComponent<Weapon>().clipSize;
            }
        }
        else if (currentlyEquippedWeaponSlot == 2)
        {
            ammoText2.GetComponent<Text>().text = currentlyEquippedWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + currentlyEquippedWeapon.GetComponent<Weapon>().clipSize;
            if (secondaryWeapon != null)
            {
                ammoText1.GetComponent<Text>().text = secondaryWeapon.GetComponent<Weapon>().currentAmmoAmount + "/" + secondaryWeapon.GetComponent<Weapon>().clipSize;
            }
        }
    }
}
