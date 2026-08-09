using DG.Tweening;
using UnityEngine;

public class HouseHealth : MonoSingleton<HouseHealth>, IDamageable
{
    [SerializeField] private int maxHealth = 100;
    public int CurrentHealth { get; private set; }
    public float CurrentHealthNormalided => (float)CurrentHealth / maxHealth;

    [SerializeField] private Transform[] attackSpots;

    [SerializeField] private float shakeStrength = 0.15f;
    [SerializeField] private float shakeDuration = 0.3f;

    protected override void AwakeBehaviour()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount, Vector2 _)
    {
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount); //Defeat Condition
        transform.DOShakePosition(shakeDuration, shakeStrength);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
    }

    public Transform GetNearestAttackPoint(Vector2 initialPosition)
    {
        Transform nearest = attackSpots[0];
        float minDist = Vector2.Distance(initialPosition, nearest.position);

        for (int i = 0; i < attackSpots.Length; i++)
        {
            float dist = Vector2.Distance(initialPosition, attackSpots[i].position);
            if(dist < minDist)
            {
                minDist = dist;
                nearest = attackSpots[i];
            }
        }
        return nearest;
    }
}
