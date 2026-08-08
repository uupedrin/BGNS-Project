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

    private void Update()
    {
        HandleInput();
    }
    private void FixedUpdate()
    {
        HandleMovement();
        ApplyBounds();
    }

    private void ApplyBounds()
    {
        if (LevelBounds.Instance == null) return;

        LevelBoundsInfo bounds = LevelBounds.Instance.GetLevelBounds();

        Vector2 currentPos = rb.position;
        Vector2 currentVel = rb.linearVelocity;

        if(currentPos.x <= bounds.minX)
        {
            currentPos.x = bounds.minX;
            if (currentVel.x < 0) currentVel.x = 0;
        }
        else if(currentPos.x >= bounds.maxX)
        {
            currentPos.x = bounds.maxX;
            if (currentVel.x > 0) currentVel.x = 0;
        }

        if(currentPos.y <= bounds.minY)
        {
            currentPos.y = bounds.minY;
            if (currentVel.y < 0) currentVel.y = 0;
        }
        else if(currentPos.y >= bounds.maxY)
        {
            currentPos.y = bounds.maxY;
            if (currentVel.y > 0) currentVel.y = 0;
        }

        if(rb.position != currentPos) rb.position = currentPos;
        if(rb.linearVelocity != currentVel) rb.linearVelocity = currentVel;
    }

    private void HandleInput()
    {
        _inputDirection = movementAction.action.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
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
