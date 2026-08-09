using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Consumable", fileName = "New Consumable")]
public class ConsumableSO : ItemSO
{
    public int healAmount = 20;

    public ConsumableSO()
    {
        itemType = ItemType.Consumable;
    }
}