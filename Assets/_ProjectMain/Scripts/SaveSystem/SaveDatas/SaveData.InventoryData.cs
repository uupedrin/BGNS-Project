using System.Collections.Generic;
using UnityEngine;

public partial class SaveData
{
    private InventoryData _inventoryData;
    public InventoryData inventoryData
    {
        get => _inventoryData ??= new InventoryData();
        set => _inventoryData = value;
    }
}

[System.Serializable]
public class InventoryData
{
    public List<InventorySlotData> slots = new();
}

[System.Serializable]
public class InventorySlotData
{
    public int slotIndex;
    public string itemId;
    public int count;
    public int currentAmmoInClip;
}