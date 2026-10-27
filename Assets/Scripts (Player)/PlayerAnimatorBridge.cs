using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimatorBridge : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private Rigidbody2D rb;

    private Animator anim;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int GroundedHash = Animator.StringToHash("Grounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int InteractHash = Animator.StringToHash("Interact");

    private void Awake() => anim = GetComponent<Animator>();

    private void Update()
    {
        anim.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool(GroundedHash, controller.IsGrounded);
    }

    public void TriggerJump() => anim.SetTrigger(JumpHash);
    public void TriggerInteract() => anim.SetTrigger(InteractHash);
}