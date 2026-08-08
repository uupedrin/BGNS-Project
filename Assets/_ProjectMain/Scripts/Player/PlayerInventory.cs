using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEvents))]
public class PlayerInventory : MonoBehaviour
{
    PlayerEvents playerEvents;

    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference nextInventoryItemAction;
    [SerializeField] private InputActionReference useSelectedItemAction;

    private ItemSO currentHeldItem;

    private void Awake()
    {
        playerEvents = GetComponent<PlayerEvents>();
    }

    private void OnEnable()
    {
        openInventoryAction?.action.Enable();
        openInventoryAction.action.performed += OnInventoryKeyPress;

        useSelectedItemAction?.action.Enable();
        useSelectedItemAction.action.performed += OnUseSelectedItemKeyPress;

        nextInventoryItemAction?.action.Enable();
    }

    private void OnDisable()
    {
        openInventoryAction?.action.Disable();
        openInventoryAction.action.performed -= OnInventoryKeyPress;

        nextInventoryItemAction?.action.Disable();

        useSelectedItemAction?.action.Disable();
        useSelectedItemAction.action.performed -= OnUseSelectedItemKeyPress;
    }

    private void Update()
    {
        HandleInventoryItemScroll();
        HandleSelectedItem();
    }

    private void OnInventoryKeyPress(InputAction.CallbackContext context)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }

    private void OnUseSelectedItemKeyPress(InputAction.CallbackContext context)
    {
        if(currentHeldItem != null)
        {
            Debug.Log("Usando item");
        }
        else
        {
            Debug.Log("Não tem item pra usar");
        }
    }

    private void HandleInventoryItemScroll()
    {
        if (InventoryManager.Instance == null) return;
        int nextSlotPos = (int)nextInventoryItemAction.action.ReadValue<float>();
        if (nextSlotPos == 0) return;
        InventoryManager.Instance.HandleSlotNavigation(nextSlotPos);
    }

    private void HandleSelectedItem()
    {
        currentHeldItem = InventoryManager.Instance.GetSelectedItem();
        bool isWeapon = currentHeldItem != null && currentHeldItem is WeaponSO;

        playerEvents.OnPlayerSelectWeapon?.Invoke(isWeapon, null);
    }
}
