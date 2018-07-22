using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public Item item;
    private string data;
    private GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip");
        tooltip.SetActive(false);
    }

    void Update () {
        if (tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition + new Vector3(-97, 200);
        }
	}

    public void Activate(Item item)
    {
        this.item = item;
        ConstructDataString();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void ConstructDataString()
    {
        if (item.Type == Item.ItemType.Gun)
        {
            data = "<size=22px>" + item.ItemName + "</size>\n\n" + item.ItemDescription + "\n\nDamage: " + item.Damage + "\n\nClip Size: " + item.ClipSize + "\n\nAmmo Type: " + item.AmmoType;
        }
        else
        {
            data = "<size=22px>" + item.ItemName + "</size>\n\n" + item.ItemDescription;
        }
        tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
    }
}
