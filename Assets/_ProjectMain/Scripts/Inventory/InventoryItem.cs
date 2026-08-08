using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemSO item { get; private set; }

    private Image itemImage;
    [SerializeField] private TMP_Text itemCountTxt;
    private int count = 1;
    public int Count
    {
        get { return count; }
        set
        {
            count = value;
            RefreshCountText();
        }
    }
    public int maxStackSize { get; private set; } = 1;

    [HideInInspector] public Transform parentAfterDrag;
    private void Awake()
    {
        itemImage = GetComponent<Image>();
        itemImage.preserveAspect = true;
    }

    public void SetItemData(ItemSO newItemData)
    {
        item = newItemData;

        itemImage.sprite = item.itemSprite;
        maxStackSize = item.itemsOnStackAmount;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        itemCountTxt.text = count.ToString();
        itemCountTxt.gameObject.SetActive(count > 1);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemImage.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
