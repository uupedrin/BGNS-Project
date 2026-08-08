using UnityEngine;

public class ItemInstance
{
    public ItemSO itemData;
    public int currentAmmoInClip;

    public ItemInstance(ItemSO itemData)
    {
        this.itemData = itemData;
        if (itemData is WeaponSO) currentAmmoInClip = 0;
    }
}
