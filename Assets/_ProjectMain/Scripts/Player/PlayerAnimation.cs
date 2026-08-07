using UnityEngine;

[RequireComponent(typeof(PlayerEvents))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private PlayerEvents playerEvents;

    private void Awake()
    {
        if (anim == null)
        {
            Debug.LogWarning("[PlayerAnimation] - Animator is null");
            anim = GetComponentInChildren<Animator>();
        }
        playerEvents = GetComponent<PlayerEvents>();
    }

    private void OnEnable()
    {
        playerEvents.OnPlayerMove += SetMovementAnimation;
    }

    private void OnDisable()
    {
        playerEvents.OnPlayerMove -= SetMovementAnimation;
    }

    private void SetMovementAnimation(bool isMoving, Vector2 moveDirection)
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("X_Input", moveDirection.x);
        anim.SetFloat("Y_Input", moveDirection.y);
    }
}
