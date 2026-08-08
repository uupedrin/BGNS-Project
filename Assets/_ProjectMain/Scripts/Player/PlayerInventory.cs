using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerEvents))]
public class PlayerInventory : MonoBehaviour
{
    PlayerEvents playerEvents;

    [SerializeField] private InputActionReference openInventoryAction;

    [SerializeField] private InputActionReference nextInventoryItemAction;
    [SerializeField] private InputActionReference selectInventorySlotAction;

    [SerializeField] private InputActionReference useSelectedItemAction;

    private ItemInstance currentHeldItem;
    private ItemInstance lastHeldItem = null;

    private bool hasDoneInitialSelection = false;

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

        selectInventorySlotAction?.action.Enable();
        selectInventorySlotAction.action.performed += OnSelectSlotPressed;
    }

    private void OnDisable()
    {
        openInventoryAction?.action.Disable();
        openInventoryAction.action.performed -= OnInventoryKeyPress;

        useSelectedItemAction?.action.Disable();
        useSelectedItemAction.action.performed -= OnUseSelectedItemKeyPress;

        nextInventoryItemAction?.action.Disable();

        selectInventorySlotAction?.action.Disable();
        selectInventorySlotAction.action.performed -= OnSelectSlotPressed;
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

    private void OnSelectSlotPressed(InputAction.CallbackContext context)
    {
        if (InventoryManager.Instance == null) return;

        if(int.TryParse(context.control.name, out int slotNumber))
        {
            InventoryManager.Instance.SelectSlot(slotNumber - 1);
        }
    }

    private void HandleSelectedItem()
    {
        currentHeldItem = InventoryManager.Instance.GetSelectedInstance();
        if (hasDoneInitialSelection && currentHeldItem == lastHeldItem) return;

        hasDoneInitialSelection = true;
        lastHeldItem = currentHeldItem;
        bool isWeapon = currentHeldItem != null && currentHeldItem.itemData is WeaponSO;

        playerEvents.OnPlayerSelectWeapon?.Invoke(isWeapon, isWeapon ? currentHeldItem : null);
    }
}
