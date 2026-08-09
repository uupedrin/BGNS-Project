using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField] private float interactionRadius = 0.5f;

    private IInteractable currentInteractable = null;

    private void OnEnable()
    {
        interactAction.action?.Enable();
    }

    private void OnDisable()
    {
        interactAction.action?.Disable();
    }

    private void Update()
    {
        if (interactAction.action.WasPressedThisFrame()) //I need to call this way because for some reason subscribing to action.performed is not working
        {
            OnInteractPressed();
        }
    }

    private void FixedUpdate()
    {
        Collider2D[] interactables = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactionLayer);
        IInteractable closestInteractable = null;
        float minDistance = Mathf.Infinity;
        for (int i = 0; i < interactables.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, interactables[i].transform.position);
            if(distance < minDistance)
            {
                IInteractable current = interactables[i].GetComponent<IInteractable>();
                if (current == null || !current.ValidForInteraction()) continue;
                closestInteractable = current;
                minDistance = distance;
            }
        }
        if(closestInteractable != currentInteractable) currentInteractable?.Highlight(false);
        currentInteractable = closestInteractable;
        currentInteractable?.Highlight(true);
    }

    private void OnInteractPressed()
    {
        if(currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
