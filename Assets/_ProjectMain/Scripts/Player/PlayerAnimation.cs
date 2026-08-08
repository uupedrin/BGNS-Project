using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerEvents))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Animator weaponAnim;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    private int defaultWeaponSortOrder;
    private PlayerEvents playerEvents;
    private bool hasWeapon;

    private void Awake()
    {
        if (anim == null)
        {
            Debug.LogWarning("[PlayerAnimation] - Animator is null");
            anim = GetComponentInChildren<Animator>();
        }
        if(weaponSpriteRenderer == null)
        {
            weaponSpriteRenderer = weaponAnim?.GetComponent<SpriteRenderer>();
        }
        defaultWeaponSortOrder = weaponSpriteRenderer.sortingOrder;
        playerEvents = GetComponent<PlayerEvents>();
    }

    private void OnEnable()
    {
        playerEvents.OnPlayerMove += SetMovementAnimation;
        playerEvents.OnPlayerSelectWeapon += SetHoldingWeapon;
        playerEvents.OnPlayerAim += SetAimAnimation;
    }

    private void OnDisable()
    {
        playerEvents.OnPlayerMove -= SetMovementAnimation;
        playerEvents.OnPlayerSelectWeapon -= SetHoldingWeapon;
        playerEvents.OnPlayerAim -= SetAimAnimation;
    }

    private void SetMovementAnimation(bool isMoving, Vector2 moveDirection)
    {
        anim.SetBool("isMoving", isMoving);
        if (!hasWeapon) SetDirection(moveDirection);
    }

    private void SetAimAnimation(Vector2 aimDirection)
    {
        if (!hasWeapon) return;
        SetDirection(aimDirection);

        bool isAimingUp = aimDirection == Vector2.up;
        weaponSpriteRenderer.sortingOrder = isAimingUp ? defaultWeaponSortOrder - 2 : defaultWeaponSortOrder;
    }

    private void SetDirection(Vector2 dir)
    {
        anim.SetFloat("X", dir.x);
        anim.SetFloat("Y", dir.y);
        weaponAnim.SetFloat("X", dir.x);
        weaponAnim.SetFloat("Y", dir.y);
    }

    private void SetHoldingWeapon(bool hasWeapon, ItemInstance weaponInstance)
    {
        this.hasWeapon = hasWeapon;
        anim.SetBool("hasWeapon", hasWeapon);
        weaponAnim.gameObject.SetActive(hasWeapon);

        if (weaponInstance == null) return;
        weaponAnim.runtimeAnimatorController = (weaponInstance.itemData as WeaponSO).weaponAnimator;
    }
}
