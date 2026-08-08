using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] GameObject inventoryItemPrefab;
    
    private Dictionary<ItemSO, int> inventoryCache; //cache for crafting system and save

    private const int AMOUNT_OF_SLOTS_POCKET = 4;

    [SerializeField] private GameObject inventoryContainer;
    public bool isInventoryOpen { get; private set; } = false;

    int selectedSlotId = -1;

    protected override void AwakeBehaviour()
    {
        inventoryCache = new();
    }

    private void Start()
    {
        ObjectPoolManager.CreatePool(inventoryItemPrefab, inventorySlots.Length / 2);
        ChangeSelectedSlot(0);
    }

    public ItemSO GetSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlotId];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if(itemInSlot != null)
        {
            return itemInSlot.item;
        }
        return null;
    }

    public void HandleSlotNavigation(int nextSlotPos)
    {
        int newValue = selectedSlotId + nextSlotPos;
        if (newValue < 0) newValue = AMOUNT_OF_SLOTS_POCKET-1;
        else if (newValue >= AMOUNT_OF_SLOTS_POCKET) newValue = 0;
        ChangeSelectedSlot(newValue);
    }

    private void ChangeSelectedSlot(int slotId)
    {
        if (slotId < 0 || slotId >= inventorySlots.Length) return;
        if(selectedSlotId >= 0) inventorySlots[selectedSlotId].Deselect();
        inventorySlots[slotId].Select();
        selectedSlotId = slotId;
    }

    public bool AddItem(ItemSO item)
    {
        InventorySlot currentSlot = null;
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            currentSlot = inventorySlots[i];
            InventoryItem itemInSlot = currentSlot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && item.stackable && itemInSlot.Count < itemInSlot.maxStackSize)
            {
                itemInSlot.Count++;
                if (inventoryCache.ContainsKey(item))
                {
                    inventoryCache[item] = itemInSlot.Count;
                }
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            currentSlot = inventorySlots[i];
            InventoryItem itemInSlot = currentSlot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null)
            {
                InventoryItem newItem = AddItem(item, currentSlot);
                if (inventoryCache.ContainsKey(item))
                {
                    inventoryCache[item] = newItem.Count;
                }
                else
                {
                    inventoryCache.Add(item, newItem.Count);
                }
                return true;
            }
        }
        return false;
    }

    private InventoryItem AddItem(ItemSO item, InventorySlot slot)
    {
        GameObject newItemGO = ObjectPoolManager.Get(inventoryItemPrefab, slot.transform);
        InventoryItem newItem = newItemGO.GetComponent<InventoryItem>();
        newItem.SetItemData(item);

        return newItem;
    }

    #region UI Control
    public void ToggleInventory()
    {
        SetInventoryVisibility(!isInventoryOpen);
    }

    public void SetInventoryVisibility(bool visibility)
    {
        inventoryContainer.SetActive(visibility);
        isInventoryOpen = visibility;
    }
    #endregion
}
