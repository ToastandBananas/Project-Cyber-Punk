using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    private List<Item> itemDatabase = new List<Item>();
    private JsonData itemData;

    public static ItemDatabase instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

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
                (float)itemData[i]["minDamage"],
                (float)itemData[i]["maxDamage"],
                (int)itemData[i]["clipSize"],
                itemData[i]["ammoType"].ToString(),
                (float)itemData[i]["minFireRate"],
                (float)itemData[i]["maxFireRate"],
                (int)itemData[i]["soundRadius"],
                (float)itemData[i]["durabilityUse"],
                (bool)itemData[i]["stackable"],
                itemData[i]["actionType"].ToString(),
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
    public float MinDamage { get; set; }
    public float MaxDamage { get; set; }
    public int ClipSize { get; set; }
    public string AmmoType { get; set; }
    public float MinFireRate { get; set; }
    public float MaxFireRate { get; set; }
    public int SoundRadius { get; set; }
    public float DurabilityUse { get; set; }
    public bool Stackable { get; set; }
    public string ActionType { get; set; }
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

    public Item(int id, string name, string description, float minDamage, float maxDamage, int clipSize, string ammoType, float minFireRate, float maxFireRate, 
                int soundRadius, float durabilityUse, bool stackable, string actionType, ItemType type) // For guns
    {
        ItemID = id;
        ItemName = name;
        ItemDescription = description;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        ClipSize = clipSize;
        AmmoType = ammoType;
        MinFireRate = minFireRate;
        MaxFireRate = maxFireRate;
        SoundRadius = soundRadius;
        DurabilityUse = durabilityUse;
        Stackable = stackable;
        ActionType = actionType;
        if (ItemID < 34)
            Sprite = Resources.Load<Sprite>("Prefabs/UI/ItemIcons/" + name);
        else
            Sprite = Resources.Load<Sprite>("Prefabs/UI/ItemIcons/GadgetIcons/" + name);
        Type = type;
    }

    public Item(int id, string name, string description, bool stackable, ItemType type) // For gadgets and other items
    {
        ItemID = id;
        ItemName = name;
        ItemDescription = description;
        Stackable = stackable;
        Sprite = Resources.Load<Sprite>("Prefabs/UI/ItemIcons/GadgetIcons/" + name);
        Type = type;
    }

    public Item() // To create an empty slot in the inventory
    {
        ItemID = -1;
    }
}
