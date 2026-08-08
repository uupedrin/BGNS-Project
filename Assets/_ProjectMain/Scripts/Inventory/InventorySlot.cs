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
        if (!InventoryManager.Instance.isInventoryOpen) return;

        InventoryItem draggedItem = eventData.pointerDrag.GetComponent<InventoryItem>();
        if (draggedItem == null) return;

        InventorySlot originSlot = draggedItem.parentAfterDrag.GetComponent<InventorySlot>();
        if (originSlot == this) return;

        InventoryItem targetItem = GetComponentInChildren<InventoryItem>();

        if(targetItem == null)
        {
            draggedItem.parentAfterDrag = transform;
            return;
        }

        if(targetItem.item == draggedItem.item && draggedItem.item.stackable)
        {
            int total = targetItem.Count + draggedItem.Count;
            int maxStack = targetItem.maxStackSize;

            targetItem.Count = Mathf.Min(total, maxStack);
            int leftover = total - targetItem.Count;

            if (leftover > 0) draggedItem.Count = leftover;
            else draggedItem.ReturnToPool();
        }
        else
        {
            targetItem.transform.SetParent(originSlot.transform);
            draggedItem.parentAfterDrag = transform;
        }
    }
}
