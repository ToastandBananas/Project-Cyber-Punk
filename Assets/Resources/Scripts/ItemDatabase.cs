using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    private List<Item> itemDatabase = new List<Item>();
    private JsonData itemData;

    private void Awake()
    {
        //items.Add(new Item("AWD", Item.ItemType.Gun, 0, "The weakest sniper rifle.", 6, 10, "7.62mm"));
        //items.Add(new Item("Big Daddy", Item.ItemType.Gun, 1, "The most powerful sniper rifle.", 10, 10, ".50cal"));
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));
        ConstructItemDatabase();
    }

    public Item FetchItemByID(int id)
    {
        for (int i = 0; i < itemDatabase.Count; i++)
        {
            if (itemDatabase[i].ItemID == id)
            {
                return itemDatabase[i];
            }
        }
        return null;
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            itemDatabase.Add(new Item(
                (int)itemData[i]["itemID"],
                itemData[i]["itemName"].ToString(),
                itemData[i]["itemDescription"].ToString(),
                (float)itemData[i]["damage"],
                (int)itemData[i]["clipSize"],
                itemData[i]["ammoType"].ToString(),
                (bool)itemData[i]["stackable"],
                (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), itemData[i]["type"].ToString())
            ));
        }
    }
}

public class Item
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public float Damage { get; set; }
    public int ClipSize { get; set; }
    public string AmmoType { get; set; }
    public bool Stackable { get; set; }
    public Sprite Sprite { get; set; }
    public enum ItemType
    {
        Gun,
        Ammo,
        Gadget,
        Consumable,
        Key,
        Quest
    }
    public ItemType Type { get; set; }

    public Item(int id, string name, string description, float damage, int clipSize, string ammoType, bool stackable, ItemType type) // For guns
    {
        ItemID = id;
        ItemName = name;
        ItemDescription = description;
        Damage = damage;
        ClipSize = clipSize;
        AmmoType = ammoType;
        Stackable = stackable;
        Sprite = Resources.Load<Sprite>("Prefabs/UI/ItemIcons/" + name);
        Type = type;
    }

    public Item(int id, string name, string description, bool stackable, ItemType type) // For gadgets and other items
    {
        ItemID = id;
        ItemName = name;
        ItemDescription = description;
        Stackable = stackable;
        Sprite = Resources.Load<Sprite>("Prefabs/UI/ItemIcons/" + name);
        Type = type;
    }

    public Item() // To create an empty slot in the inventory
    {
        ItemID = -1;
    }
}
