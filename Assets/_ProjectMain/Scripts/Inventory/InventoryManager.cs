using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoSingleton<InventoryManager>
{
    [SerializeField] InventorySlot[] inventorySlots;
    [SerializeField] GameObject inventoryItemPrefab;
    
    private Dictionary<ItemSO, int> inventoryCache; //cache for crafting system and save

    private const int AMOUNT_OF_SLOTS_POCKET = 4;

    [SerializeField] private GameObject inventoryContainer;

    [SerializeField] private ItemDetailPanel itemDetailPanel;
    public void ShowItemDetails(ItemSO item) => itemDetailPanel.Show(item);
    public void ClearItemDetails() => itemDetailPanel.Clear();

    public bool isInventoryOpen { get; private set; } = false;

    int selectedSlotId = -1;

    protected override void AwakeBehaviour()
    {
        inventoryCache = new();
    }

    private void Start()
    {
        ObjectPoolManager.CreatePool(inventoryItemPrefab, inventorySlots.Length / 2);
        SetInventoryVisibility(false);
        ChangeSelectedSlot(0);
        SaveManager.ApplyPendingLoad();
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

    public int GetItemCount(ItemSO item) => inventoryCache.TryGetValue(item, out int count) ? count : 0;
    public int ConsumeItem(ItemSO item, int amount)
    {
        int remaining = amount;
        for (int i = 0; i < inventorySlots.Length && remaining > 0; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null || itemInSlot.item != item) continue;

            int taken = Mathf.Min(remaining, itemInSlot.Count);
            itemInSlot.Count -= taken;
            remaining -= taken;

            if(itemInSlot.Count <= 0)
            {
                ObjectPoolManager.Return(itemInSlot.gameObject);
            }
        }

        int consumed = amount - remaining;
        if (inventoryCache.ContainsKey(item))
        {
            inventoryCache[item] -= consumed;
            if (inventoryCache[item] <= 0) inventoryCache.Remove(item);
        }
        return consumed;
    }

    public ItemInstance GetSelectedInstance()
    {
        InventorySlot slot = inventorySlots[selectedSlotId];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if(itemInSlot != null)
        {
            return itemInSlot.instance;
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

    public void SelectSlot(int slotId) => ChangeSelectedSlot(slotId);

    private void ChangeSelectedSlot(int slotId)
    {
        if (slotId < 0 || slotId >= inventorySlots.Length) return;
        if(selectedSlotId >= 0) inventorySlots[selectedSlotId].Deselect();
        inventorySlots[slotId].Select();
        selectedSlotId = slotId;
    }

    public bool AddItem(ItemSO item) => AddItem(item, 1) > 0;
    public int AddItem(ItemSO item, int amount)
    {
        int remaining = amount;
        if (item.stackable)
        {
            for (int i = 0; i < inventorySlots.Length && remaining > 0; i++)
            {
                InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null || itemInSlot.item != item) continue;

                int spaceInStack = itemInSlot.maxStackSize - itemInSlot.Count;
                if (spaceInStack <= 0) continue;

                int toAdd = Mathf.Min(spaceInStack, remaining);
                itemInSlot.Count += toAdd;
                remaining -= toAdd;
            }
        }

        for (int i = 0; i < inventorySlots.Length && remaining > 0; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null) continue;

            int stackSize = item.stackable ? Mathf.Min(item.itemsOnStackAmount, remaining) : 1;
            InventoryItem newItem = AddItem(item, inventorySlots[i], stackSize);
            remaining -= stackSize;
        }

        int added = amount - remaining;
        if (added > 0)
        {
            inventoryCache.TryGetValue(item, out int current);
            inventoryCache[item] = current + added;
        }

        return added;
    }

    private InventoryItem AddItem(ItemSO item, InventorySlot slot, int amount = 1)
    {
        GameObject newItemGO = ObjectPoolManager.Get(inventoryItemPrefab, slot.transform);
        InventoryItem newItem = newItemGO.GetComponent<InventoryItem>();
        newItem.SetItemData(new ItemInstance(item));
        newItem.Count = amount;

        return newItem;
    }

    public InventoryData CaptureState()
    {
        InventoryData data = new();
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null) continue;

            data.slots.Add(new InventorySlotData
            {
                slotIndex = i,
                itemId = itemInSlot.item.itemId,
                count = itemInSlot.Count,
                currentAmmoInClip = itemInSlot.instance.currentAmmoInClip
            });
        }
        return data;
    }

    public void RestoreState(InventoryData data)
    {
        ClearInventory();
        foreach (InventorySlotData slotData in data.slots)
        {
            ItemSO item = ItemDatabase.Instance.GetItemById(slotData.itemId);
            if (item == null) continue;

            InventoryItem newItem = AddItem(item, inventorySlots[slotData.slotIndex], slotData.count);
            newItem.instance.currentAmmoInClip = slotData.currentAmmoInClip;

            inventoryCache.TryGetValue(item, out int current);
            inventoryCache[item] = current + slotData.count;
        }
    }

    private void ClearInventory()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventoryItem itemInSlot = inventorySlots[i].GetComponentInChildren<InventoryItem>();
            if(itemInSlot != null)
            {
                ObjectPoolManager.Return(itemInSlot.gameObject);
            }
        }
        inventoryCache.Clear();
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
