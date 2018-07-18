using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    public int slotsX, slotsY;
    public GUISkin skin;

    public List<Item> inventory = new List<Item>();
    public List<Item> slot = new List<Item>();

    private bool showInventory;
    private bool showTooltip;
    private string tooltip;

    private bool draggingItem;
    private Item itemBeingDragged;
    private int prevIndex;

    private ItemDatabase database;

    // Use this for initialization
    /*void Start () {
        for (int i = 0; i < (slotsX * slotsY); i++)
        {
            slot.Add(new Item());
            inventory.Add(new Item());
        }
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            showInventory = !showInventory; // Toggles the inventory menu display

            if (draggingItem)
            {
                inventory[prevIndex] = itemBeingDragged;
                draggingItem = false;
                itemBeingDragged = null;
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(40, 400, 100, 40), "Save"))
        {
            SaveInventory();
        }

        if (GUI.Button(new Rect(40, 450, 100, 40), "Load"))
        {
            LoadInventory();
        }

        tooltip = "";
        GUI.skin = skin;

        if (showInventory)
        {
            DrawInventory();
            Cursor.visible = true;
            if (showTooltip)
            {
                GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 200, 250), tooltip, skin.GetStyle("TooltipBackground"));
            }
        }
        else
        {
            Cursor.visible = false;
        }

        if (draggingItem)
        {
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 80, 80), itemBeingDragged.itemIcon);
        }
        showTooltip = false;
    }

    void DrawInventory()
    {
        int i = 0;
        for (int y = 0; y < slotsY; y++)
        {
            for (int x = 0; x < slotsX; x++)
            {
                Rect slotRect = new Rect(x * 80 + (Screen.width / 2) - 120, y * 80 + (Screen.height / 2) - 120, 80, 80);
                GUI.Box(slotRect, "", skin.GetStyle("SlotBackground"));
                slot[i] = inventory[i];
                if (slot[i].itemName != null)
                {
                    GUI.DrawTexture(slotRect, slot[i].itemIcon);
                    if (slotRect.Contains(Event.current.mousePosition)) // If mouse pointer is hovering over the slot
                    {
                        CreateTooltip(slot[i]);
                        showTooltip = true;
                        if (Event.current.button == 0 && Event.current.type == EventType.MouseDrag && !draggingItem) // If left mouse click and drag an item
                        {
                            draggingItem = true;
                            prevIndex = i;
                            itemBeingDragged = slot[i];
                            inventory[i] = new Item();
                        }

                        if (Event.current.type == EventType.MouseUp && draggingItem)
                        {
                            inventory[prevIndex] = inventory[i];
                            inventory[i] = itemBeingDragged;
                            draggingItem = false;
                            itemBeingDragged = null;
                        }

                        if (Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 1) // If right mouse click
                        {
                            // print("Clicked " + i);
                            if (slot[i].itemType == Item.ItemType.Consumable)
                            {
                                UseConsumable(slot[i], i, true);
                            }
                        }
                    }
                }
                else
                {
                    if (slotRect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseUp && draggingItem)
                        {
                            inventory[i] = itemBeingDragged;
                            draggingItem = false;
                            itemBeingDragged = null;
                        }
                    }
                }
                i++;
            }
        }
    }

    string CreateTooltip(Item item)
    {
        string yellowFontColor = "<color=#FFA500>";

        if (item.itemType == Item.ItemType.Gun)
        {
            tooltip = "<size=22px>" + item.itemName + "</size>\n\n" + item.itemDescription + "\n\nDamage: " + item.damage + "\n\nClip Size: " + item.clipSize + "\n\nAmmo Type: " + item.ammoType;
        }
        else
        {
            tooltip = "<size=22px>" + item.itemName + "</size>\n\n" + item.itemDescription;
        }
        return tooltip;
    }

    void RemoveItem(int id)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemID == id)
            {
                inventory[i] = new Item();
                break;
            }
        }
    }

    void AddItem(int id)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].itemName == null)
            {
                for (int j = 0; j < database.items.Count; j++)
                {
                    if (database.items[j].itemID == id)
                    {
                        inventory[i] = database.items[j];
                    }
                }
                break;
            }
        }
    }

    void UseConsumable(Item item, int slot, bool deleteItem)
    {
        switch (item.itemID)
        {
            case 2:
                {
                    print("Used consumable.");
                    break;
                }
        }
        
        if (deleteItem)
        {
            inventory[slot] = new Item();
        }
    }

    bool InventoryContains(int id)
    {
        foreach (Item item in inventory)
        {
            if (item.itemID == id)
            {
                return true;
            }
        }
        return false;
    }

    void SaveInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            PlayerPrefs.SetInt("Inventory" + i, inventory[i].itemID);
        }
    }

    void LoadInventory()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i] = PlayerPrefs.GetInt("Inventory" + i, -1) >= 0 ? database.items[PlayerPrefs.GetInt("Inventory" + i)] : new Item();
        }
    }*/
}
