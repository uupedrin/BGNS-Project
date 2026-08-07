using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public partial class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionReference movementAction;
    private Rigidbody2D rb;

    private Vector2 _inputDirection = Vector2.zero;
    [SerializeField] private float moveSpeed = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        movementAction.action?.Enable();
    }

    private void OnDisable()
    {
        movementAction.action?.Disable();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        _inputDirection = movementAction.action.ReadValue<Vector2>();
        rb.linearVelocity = _inputDirection * moveSpeed;
    }
}
