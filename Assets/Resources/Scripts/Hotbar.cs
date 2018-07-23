using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {
    public GameObject weaponSlot1;
    public GameObject weaponSlot2;
    public GameObject gadgetSlot1;
    public GameObject gadgetSlot2;

    public GameObject gadgetItem;
    public GameObject weaponItem;

    public GameObject currentlyEquippedWeapon;
    public int currentlyEquippedWeaponSlot = 1;

    GameObject infoPanel;
    GameObject weaponToDrop;

    Player player;

    public int startingWeaponID;

    GameObject[] weaponObjects;

    ItemDatabase itemDatabase;
    LootDrop lootDropScript;

    public Color32 equippedColor = new Color32(138, 181, 246, 255);
    public Color32 unequippedColor = new Color32(255, 255, 255, 255);

    public List<Item> items = new List<Item>();

    // Use this for initialization
    void Start () {
        itemDatabase = GetComponent<ItemDatabase>();
        
        infoPanel = GameObject.Find("InfoPanel");
        infoPanel.SetActive(false);
        player = Player.instance;

        AddItemToInventory(startingWeaponID);

        weaponObjects = GameObject.FindGameObjectsWithTag("EquippedWeapon");
        foreach (GameObject weaponObject in weaponObjects)
        {
            if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)
            {
                weaponObject.SetActive(true);
                currentlyEquippedWeapon = weaponObject;
                currentlyEquippedWeaponSlot = 1;
                weaponSlot1.GetComponent<Image>().color = equippedColor;
            }
            else
            {
                weaponObject.SetActive(false);
            }
        }
    }

    void FixedUpdate()
    {
        SwapWeapon();
        print("Currently equipped weapon: " + currentlyEquippedWeapon);
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

    public void EquipWeapon(int id)
    {
        Item itemToAdd = itemDatabase.FetchItemByID(id);

        foreach (GameObject weaponObject in weaponObjects)
        {
            if (weaponObject.name == itemToAdd.ItemName)
            {
                weaponObject.SetActive(true);
                if (weaponSlot1.GetComponent<Slot>().isEmpty == false && weaponSlot2.GetComponent<Slot>().isEmpty == false)
                {
                    print("Currently equipped weapon: " + currentlyEquippedWeapon.name);
                    weaponToDrop = Resources.Load("Prefabs/Items/WeaponDrops/" + currentlyEquippedWeapon.name + " Item Drop") as GameObject;
                    // Currently equipped weapon and slot are set in the WeaponPickup script
                    DropWeapon(weaponToDrop);
                }
            }
            else
            {
                weaponObject.SetActive(false);
            }
        }
    }

    private void DropWeapon(GameObject weaponToDrop)
    {
        GameObject droppedWeapon = Instantiate(weaponToDrop);
        droppedWeapon.transform.position = player.transform.position + new Vector3(0, .2f);
    }

    void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            if (weaponSlot1.transform.childCount == 2) // If weapon slot 1 isn't empty
            {
                foreach (GameObject weaponObject in weaponObjects)
                {
                    // if the weapon in the object pool equals the weapon in slot 1
                    if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)
                    {
                        weaponObject.SetActive(true);
                        currentlyEquippedWeapon = weaponObject;
                        currentlyEquippedWeaponSlot = 1;
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
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            if (weaponSlot2.transform.childCount == 2) // If weapon slot 2 isn't empty
            {
                foreach (GameObject weaponObject in weaponObjects)
                {
                    // if the weapon in the object pool equals the weapon in slot 2 
                    if (weaponObject.name == weaponSlot2.transform.GetChild(2).name)
                    {
                        weaponObject.SetActive(true);
                        currentlyEquippedWeapon = weaponObject;
                        currentlyEquippedWeaponSlot = 2;
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
                    }
                }
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (weaponSlot1.transform.childCount == 2 && weaponSlot2.transform.childCount == 2) // If both weapon slots are occupied
            {
                if (currentlyEquippedWeaponSlot == 1) // If currently equipped weapon is in weapon slot 1
                {
                    foreach (GameObject weaponObject in weaponObjects)
                    {
                        if (weaponObject.name == weaponSlot2.transform.GetChild(2).name) // If the weapon in the object pool equals the weapon in weapon slot 2
                        {
                            weaponObject.SetActive(true);
                            currentlyEquippedWeapon = weaponObject;
                            currentlyEquippedWeaponSlot = 2;
                            weaponSlot1.GetComponent<Image>().color = unequippedColor;
                            weaponSlot2.GetComponent<Image>().color = equippedColor;
                        }
                        else
                        {
                            weaponObject.SetActive(false);
                        }
                    }
                }
                else if (currentlyEquippedWeaponSlot == 2) // If currently equipped weapon is in weapon slot 2
                {
                    foreach (GameObject weaponObject in weaponObjects)
                    {
                        if (weaponObject.name == weaponSlot1.transform.GetChild(2).name)  // If the weapon in the object pool equals the weapon in weapon slot 1
                        {
                            weaponObject.SetActive(true);
                            currentlyEquippedWeapon = weaponObject;
                            currentlyEquippedWeaponSlot = 1;
                            weaponSlot1.GetComponent<Image>().color = equippedColor;
                            weaponSlot2.GetComponent<Image>().color = unequippedColor;
                        }
                        else
                        {
                            weaponObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void IncreaseAmmo()
    {
        // To Do: Call these two functions when appropriate...
    }

    public void DecreaseAmmo()
    {

    }
}
