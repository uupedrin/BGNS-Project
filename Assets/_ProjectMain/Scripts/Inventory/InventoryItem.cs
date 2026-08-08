using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemInstance instance { get; private set; }
    public ItemSO item => instance.itemData;

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
    private bool wasDragged = false;
    private bool consumed = false;

    private void Awake()
    {
        itemImage = GetComponent<Image>();
        itemImage.preserveAspect = true;
    }

    public void SetItemData(ItemInstance newInstance)
    {
        instance = newInstance;
        itemImage.sprite = item.itemSprite;
        maxStackSize = item.itemsOnStackAmount;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        itemCountTxt.text = count.ToString();
        itemCountTxt.gameObject.SetActive(count > 1);
    }

    public void ReturnToPool()
    {
        consumed = true;
        ObjectPoolManager.Return(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!InventoryManager.Instance.isInventoryOpen) return;
        wasDragged = true;
        itemImage.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!InventoryManager.Instance.isInventoryOpen) return;
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!wasDragged) return;
        wasDragged = false;
        itemImage.raycastTarget = true;

        if (consumed)
        {
            consumed = false;
            return;
        }

        transform.SetParent(parentAfterDrag);
    }
}
