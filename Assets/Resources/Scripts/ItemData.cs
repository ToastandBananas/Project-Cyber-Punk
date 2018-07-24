using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    public int amount;
    
    private Tooltip tooltip;

    MouseCursor mouseCursor;

    void Start()
    {
        tooltip = GameObject.Find("Hotbar").GetComponent<Tooltip>();
        mouseCursor = GameObject.Find("Crosshair").GetComponent<MouseCursor>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item.ItemID != -1) // If slot isn't empty
        {
            transform.SetParent(transform.parent.parent); // Set parent to slot panel so that the item doesn't appear behind any slots
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item.ItemID != -1) // If slot isn't empty
        {
            transform.position = eventData.position; // Move item's position to mouse position
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Activate(item);
        Cursor.visible = true;
        mouseCursor.spriteRenderer.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Deactivate();
        Cursor.visible = false;
        mouseCursor.spriteRenderer.enabled = true;
    }
}
