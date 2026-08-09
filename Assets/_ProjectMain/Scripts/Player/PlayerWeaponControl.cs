using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Threading;

[RequireComponent(typeof(PlayerEvents))]
[RequireComponent(typeof(PlayerWeaponAim))]
public class PlayerWeaponControl : MonoBehaviour
{
    private PlayerEvents playerEvents;
    private PlayerWeaponAim weaponAim;

    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private Animator weaponAnim;
    [SerializeField] private WeaponAnimationEvents weaponAnimEvents;

    private ItemInstance weaponInstance;
    private WeaponSO weaponData;
    private float nextFireTime;

    private bool isReloading = false;
    private int pendingReloadAmount;

    private void Awake()
    {
        playerEvents = GetComponent<PlayerEvents>();
        weaponAim = GetComponent<PlayerWeaponAim>();
    }

    private void OnEnable()
    {
        attackAction.action?.Enable();

        reloadAction.action?.Enable();
        reloadAction.action.performed += OnReloadPressed;

        playerEvents.OnPlayerSelectWeapon += OnWeaponChanged;

        weaponAnimEvents.OnAmmoInsert += HandleReloadAmmoInsert;
        weaponAnimEvents.OnReloadComplete += HandleReloadComplete;
    }

    private void OnDisable()
    {
        attackAction.action?.Disable();

        reloadAction.action?.Disable();
        reloadAction.action.performed -= OnReloadPressed;

        playerEvents.OnPlayerSelectWeapon -= OnWeaponChanged;

        weaponAnimEvents.OnAmmoInsert -= HandleReloadAmmoInsert;
        weaponAnimEvents.OnReloadComplete -= HandleReloadComplete;
    }

    private void Update()
    {
        if (attackAction.action.IsPressed()) OnAttackHeld();
    }

    private void OnWeaponChanged(bool hasWeapon, ItemInstance instance)
    {
        isReloading = false;

        weaponInstance = hasWeapon ? instance : null;
        weaponData = hasWeapon ? instance.itemData as WeaponSO : null;

        InventoryManager.Instance.SetAmmoUIVisible(hasWeapon);
        if (hasWeapon) NotifyAmmoChanged();
    }

    private void NotifyAmmoChanged()
    {
        InventoryManager.Instance.UpdateAmmoUI(weaponInstance.currentAmmoInClip);
    }

    private void OnAttackHeld()
    {
        if (PauseUI.IsPaused || InventoryManager.Instance.isInventoryOpen || weaponInstance == null || isReloading || Time.time < nextFireTime) return;
        if (weaponInstance.currentAmmoInClip <= 0) return; //click sound

        weaponInstance.currentAmmoInClip--;
        nextFireTime = Time.time + weaponData.fireRate;
        weaponAnim.SetTrigger("Shoot");
        EndShootAnim(weaponData.fireRate).Forget();
        SpawnProjectile();
        NotifyAmmoChanged();
    }

    private async UniTask EndShootAnim(float delay)
    {
        await UniTask.WaitForSeconds(delay);
        weaponAnim.SetTrigger("ShootEnds");
    }

    private void OnReloadPressed(InputAction.CallbackContext context)
    {
        if (PauseUI.IsPaused || weaponInstance == null || isReloading) return;

        int needed = weaponData.clipSize - weaponInstance.currentAmmoInClip;
        if (needed <= 0) return;

        int available = InventoryManager.Instance.GetItemCount(weaponData.ammoType);
        if (available <= 0) return;

        pendingReloadAmount = needed;
        isReloading = true;
        weaponAnim.SetTrigger("Reload");
    }

    private void HandleReloadAmmoInsert()
    {
        if (!isReloading) return;

        int taken = InventoryManager.Instance.ConsumeItem(weaponData.ammoType, pendingReloadAmount);
        weaponInstance.currentAmmoInClip += taken;
        NotifyAmmoChanged();
    }

    private void HandleReloadComplete()
    {
        if (!isReloading) return;
        isReloading = false;
    }
    
    private void SpawnProjectile()
    {
        GameObject projectileGO = ObjectPoolManager.Get(weaponData.projectilePrefab, weaponAim.FirePoint.position, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        projectile.Setup(weaponAim.AimDirection, weaponData.damage);
    }
}
