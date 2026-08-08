using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventorySlot : MonoBehaviour, IDropHandler
{
    private Image slotImage;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;

    private void Awake()
    {
        slotImage = GetComponent<Image>();
        slotImage.preserveAspect = true;
        Deselect();
    }

    public void Select()
    {
        slotImage.sprite = selectedSprite;
    }

    public void Deselect()
    {
        slotImage.sprite = deselectedSprite;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            InventoryItem inventoryItem = eventData.pointerDrag.GetComponent<InventoryItem>();
            if(inventoryItem != null)
            {
                inventoryItem.parentAfterDrag = transform;
            }
        }
    }
}
