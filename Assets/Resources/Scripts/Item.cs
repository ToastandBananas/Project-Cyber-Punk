/*using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType itemType;
    public int itemID;
    public Texture2D itemIcon;
    public string itemDescription;
    public float damage;
    public int clipSize;
    public string ammoType;

    public enum ItemType
    {
        Gun,
        Ammo,
        Gadget,
        Consumable,
        Key,
        Quest
    }

    public Item(string name, ItemType type, int id, string desc, float dam, int clipSz, string ammo)
    {
        itemName = name;
        itemType = type;
        itemID = id;
        itemIcon = Resources.Load<Texture2D>("ItemIcons/" + name);
        itemDescription = desc;
        damage = dam;
        clipSize = clipSz;
        ammoType = ammo;
    }

    public Item()
    {
        itemID = -1;
    }
}
*/