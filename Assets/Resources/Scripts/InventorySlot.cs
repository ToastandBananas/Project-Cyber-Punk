using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotID;
    private Inventory inventory;
    public bool isOccupied;

    void Start()
    {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        if (inventory.items[slotID].ItemID == -1) // If slot is empty
        {
            isOccupied = false;
            inventory.items[droppedItem.slot] = new Item();
            inventory.items[slotID] = droppedItem.item;
            droppedItem.slot = slotID;
        }
        else if (droppedItem.slot != slotID)
        {
            isOccupied = true;
            Transform item = transform.GetChild(0);
            item.GetComponent<ItemData>().slot = droppedItem.slot;
            item.transform.SetParent(inventory.slots[droppedItem.slot].transform);
            item.transform.position = inventory.slots[droppedItem.slot].transform.position;

            droppedItem.slot = slotID;
            droppedItem.transform.SetParent(transform);
            droppedItem.transform.position = transform.position;

            inventory.items[droppedItem.slot] = item.GetComponent<ItemData>().item;
            inventory.items[slotID] = droppedItem.item;
        }
    }
}
