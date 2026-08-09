using UnityEngine;

[CreateAssetMenu(menuName = "Data/New Zombie", fileName = "New Zombie")]
public class ZombieSO : ScriptableObject
{
    public int maxHealth = 30;
    public float moveSpeed = 2f;
    public int damageToHouse = 10;

    public float knockbackForce = 6f;
    public float knockbackDuration = 0.2f;
    public float slowMultiplier = 0.5f;
    public float slowDuration = 1f;
}
