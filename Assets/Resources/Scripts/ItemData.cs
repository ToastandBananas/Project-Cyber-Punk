using UnityEngine;
using UnityEngine.EventSystems;

public class ItemData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
