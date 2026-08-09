using UnityEngine;

[CreateAssetMenu(menuName = "Data/Loot Table")]
public class LootTableSO : ScriptableObject
{
    [System.Serializable]
    public class LootEntry
    {
        public ItemSO item;
        public int minAmount = 1;
        public int maxAmount = 1;
        public float weight = 1f;
    }

    public LootEntry[] entries;

    public LootEntry RollLoot()
    {
        float totalWeight = 0f;
        foreach (LootEntry entry in entries) totalWeight += entry.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var entry in entries)
        {
            cumulative += entry.weight;
            if (roll <= cumulative) return entry;
        }
        return entries[entries.Length - 1];
    }
}
