using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 2f;

    [SerializeField] private LayerMask hittableLayers;

    private Rigidbody2D rb;

    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction, int damage)
    {
        direction = direction.normalized;
        this.damage = damage;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        rb.linearVelocity = direction * speed;
        ObjectPoolManager.Return(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((hittableLayers.value & (1 << other.gameObject.layer)) == 0) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            ObjectPoolManager.Return(gameObject);
        }
    }
}
