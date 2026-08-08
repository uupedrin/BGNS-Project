using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEvents))]
public class PlayerWeaponAim : MonoBehaviour
{
    private PlayerEvents playerEvents;
    [SerializeField] private InputActionReference aimAction;

    [SerializeField] private Transform weaponHolderObject;

    [SerializeField] private float weaponOffsetRadius = 0.5f;

    private Camera cam;

    private bool isHoldingWeapon = false;

    private void Awake()
    {
        playerEvents = GetComponent<PlayerEvents>();
    }

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        aimAction.action?.Enable();
        playerEvents.OnPlayerSelectWeapon += OnPlayerEquipWeapon;
    }

    private void OnDisable()
    {
        aimAction.action?.Disable();
        playerEvents.OnPlayerSelectWeapon -= OnPlayerEquipWeapon;
    }

    private void Update()
    {
        if (!isHoldingWeapon) return;

        HandleWeaponRotationAndPosition();

    }

    private void HandleWeaponRotationAndPosition()
    {
        Vector2 mouseScreenPos = aimAction.action.ReadValue<Vector2>();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -cam.transform.position.z));
        Vector2 aimDir = ((Vector2)mouseWorldPos - (Vector2)weaponHolderObject.position).normalized;

        weaponHolderObject.localPosition = aimDir * -weaponOffsetRadius;

        Vector2 snappedDir;
        if (Mathf.Abs(aimDir.y) > Mathf.Abs(aimDir.x)) snappedDir = Vector2.up * Mathf.Sign(aimDir.y);
        else snappedDir = Vector2.right * Mathf.Sign(aimDir.x);

        playerEvents.OnPlayerAim?.Invoke(snappedDir);

        float realAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Atan2(snappedDir.y, snappedDir.x) * Mathf.Rad2Deg;
        float offset = Mathf.DeltaAngle(snappedAngle, realAngle);

        weaponHolderObject.localRotation = Quaternion.Euler(0, 0, offset);
    }

    private void OnPlayerEquipWeapon(bool hasEquipedWeapon, ItemInstance _)
    {
        isHoldingWeapon = hasEquipedWeapon;
        weaponHolderObject.gameObject.SetActive(isHoldingWeapon);
    }
}
