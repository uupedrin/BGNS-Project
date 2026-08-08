using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Database", fileName = "Item Database")]
public class ItemDatabase : ScriptableObject
{
    private static ItemDatabase _instance;
    public static ItemDatabase Instance => _instance ?? Resources.Load<ItemDatabase>("ItemDatabase");

    [SerializeField] private ItemSO[] items;
    private Dictionary<string, ItemSO> lookupTable;

    public ItemSO GetItemById(string id)
    {
        lookupTable ??= BuildLookupDict();
        return lookupTable.TryGetValue(id, out ItemSO item) ? item : null;
    }

    private Dictionary<string, ItemSO> BuildLookupDict()
    {
        Dictionary<string, ItemSO> dict = new();
        foreach (ItemSO item in items)
        {
            if (!string.IsNullOrEmpty(item.itemId)) dict[item.itemId] = item;
        }
        return dict;
    }
}
