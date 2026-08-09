using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Weapon", fileName = "New Weapon")]
public class WeaponSO : ItemSO
{
    [Header("Weapon Stats")]
    public int damage = 10;
    public float fireRate = 0.5f;

    public RuntimeAnimatorController weaponAnimator;

    public ItemSO ammoType;
    public GameObject projectilePrefab;
    public int clipSize = 1;

    public WeaponSO()
    {
        itemType = ItemType.Weapon;
        stackable = false;
    }
}
