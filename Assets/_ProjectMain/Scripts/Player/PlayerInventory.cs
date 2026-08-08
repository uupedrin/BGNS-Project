using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private InputActionReference openInventoryAction;
    [SerializeField] private InputActionReference nextInventoryItemAction;

    private void OnEnable()
    {
        openInventoryAction?.action.Enable();
        openInventoryAction.action.performed += OnInventoryKeyPress;

        nextInventoryItemAction?.action.Enable();
    }

    private void OnDisable()
    {
        openInventoryAction?.action.Disable();
        openInventoryAction.action.performed -= OnInventoryKeyPress;

        nextInventoryItemAction?.action.Disable();
    }

    private void Update()
    {
        HandleInventoryItemScroll();
    }

    private void OnInventoryKeyPress(InputAction.CallbackContext context)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ToggleInventory();
        }
    }

    private void HandleInventoryItemScroll()
    {
        int nextSlotPos = (int)nextInventoryItemAction.action.ReadValue<float>();
        if (nextSlotPos == 0) return;
        InventoryManager.Instance.HandleSlotNavigation(nextSlotPos);
    }
}
