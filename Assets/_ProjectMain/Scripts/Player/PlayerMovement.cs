using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerEvents))]
public partial class PlayerMovement : MonoBehaviour
{
    private PlayerEvents playerEvents;

    [SerializeField] private InputActionReference movementAction;
    private Rigidbody2D rb;

    private Vector2 _inputDirection = Vector2.zero;
    private Vector2 _lastDirection = Vector2.down;
    [SerializeField] private float moveSpeed = 20f;

    public bool isMoving { get; private set; } = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerEvents = GetComponent<PlayerEvents>();
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
        isMoving = _inputDirection != Vector2.zero;

        Vector2 animDirection = _lastDirection;
        if (isMoving)
        {
            _lastDirection = _inputDirection;
            if (Mathf.Abs(_lastDirection.y) > Mathf.Abs(_lastDirection.x)) animDirection = Vector2.up * _lastDirection.y;
            else animDirection = Vector2.right * _lastDirection.x;
        }

        playerEvents.OnPlayerMove?.Invoke(isMoving, animDirection);

        rb.linearVelocity = _inputDirection * moveSpeed;
    }
}
