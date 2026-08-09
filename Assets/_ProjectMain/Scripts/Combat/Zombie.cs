using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Zombie : MonoBehaviour, IDamageable
{
    [SerializeField] private ZombieSO data;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    private Rigidbody2D rb;
    private Transform targetPoint;
    private int currentHealth;
    private float currentSpeed;
    private bool isKnockedBack = false;

    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private float deathAnimDuration = 1f;
    [SerializeField] private Collider2D enemyHitTrigger;
    private float lastFacingX = 1f;
    private Color originalColor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        currentHealth = data.maxHealth;
        currentSpeed = data.moveSpeed;
        isKnockedBack = false;
        spriteRenderer.material.SetFloat("_FlashAmount", 0f);
        enemyHitTrigger.enabled = true;
        spriteRenderer.color = originalColor;

        targetPoint = HouseHealth.Instance.GetNearestAttackPoint(transform.position);
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
        {
            anim.SetBool("isMoving", false);
            return;
        }
        Vector2 direction = ((Vector2)targetPoint.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * currentSpeed;

        Vector2 animDir = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? Vector2.up * Mathf.Sign(direction.y) : Vector2.right * Mathf.Sign(direction.x);

        anim.SetBool("isMoving", true);
        anim.SetFloat("X", animDir.x);
        anim.SetFloat("Y", animDir.y);

        if (animDir.x != 0) lastFacingX = animDir.x;
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        currentHealth -= amount;
        FlashWhite().Forget();
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        ApplyKnockback(hitDirection.normalized).Forget();
        ApplySlow().Forget();
    }

    private void Die()
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        enemyHitTrigger.enabled = false;

        anim.SetFloat("X", Mathf.Sign(lastFacingX));

        anim.SetFloat("DeathVariant", Random.Range(0, 2));
        anim.SetTrigger("Death");

        PlayDeathAndReturn().Forget();
    }

    private async UniTask ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        rb.linearVelocity = direction * data.knockbackForce;
        await UniTask.WaitForSeconds(data.knockbackDuration);
        isKnockedBack = false;
    }

    private async UniTask FlashWhite()
    {
        spriteRenderer.material.SetFloat("_FlashAmount", 1f);
        await UniTask.WaitForSeconds(0.1f);
        spriteRenderer.material.DOFloat(0f, "_FlashAmount", 0.15f);
    }

    private async UniTask ApplySlow()
    {
        currentSpeed = data.moveSpeed * data.slowMultiplier;
        await UniTask.WaitForSeconds(data.slowDuration);
        currentSpeed = data.moveSpeed;
    }

    private async UniTask PlayDeathAndReturn()
    {
        await UniTask.WaitForSeconds(deathAnimDuration);
        spriteRenderer.DOFade(0f, fadeDuration).OnComplete(() => ObjectPoolManager.Return(gameObject));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out HouseHealth house)) return;

        house.TakeDamage(data.damageToHouse, Vector2.zero);
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        spriteRenderer.DOFade(0f, fadeDuration).OnComplete(() => ObjectPoolManager.Return(gameObject));
        
    }
}