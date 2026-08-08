using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Item")]
public class ItemSO : ScriptableObject
{
    public string itemId;
    public string itemName;
    public string itemDescription;
    public Sprite itemSprite;
    public ItemType itemType;

    public bool stackable = true;
    public int itemsOnStackAmount = 16;
}

public enum ItemType
{
    Weapon,
    Ammo,
    Resource,
    Consumable
}