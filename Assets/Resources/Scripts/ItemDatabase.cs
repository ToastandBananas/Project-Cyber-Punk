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

        Debug.Log(itemDatabase[0].ItemName);
    }

    void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            itemDatabase.Add(new Item((int)itemData[i]["itemID"], itemData[i]["itemName"].ToString(), itemData[i]["itemDescription"].ToString()));
        }
    }
}

public class Item
{
    public int ItemID { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }

    public Item(int id, string name, string description)
    {
        ItemID = id;
        ItemName = name;
        ItemDescription = description;
    }
}
