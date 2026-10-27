using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpVelocity = 11f;
    [SerializeField] private float gravityScale = 3f;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.15f;
    [SerializeField] private float maxSlopeAngle = 55f;
    [SerializeField] private float stickForce = 2f;

    [Header("Feel")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    private Rigidbody2D rb;
    private Collider2D col;

    private float inputX;
    private bool grounded;
    private Vector2 groundNormal = Vector2.up;
    private float coyoteTimer, bufferTimer, ignoreGroundTimer;
    private bool jumpReleased;

    public bool IsGrounded => grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        inputX = ReadHorizontal();
        if (JumpPressed()) bufferTimer = jumpBuffer;
        if (JumpReleased()) jumpReleased = true;

        if (inputX != 0)
        {
            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(inputX);
            transform.localScale = s;
        }
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        bufferTimer -= dt;
        ignoreGroundTimer -= dt;

        CheckGround();
        coyoteTimer = grounded ? coyoteTime : coyoteTimer - dt;

        // Jump
        if (bufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.gravityScale = gravityScale;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
            bufferTimer = coyoteTimer = 0f;
            ignoreGroundTimer = 0.1f;
            grounded = false;
            jumpReleased = false;
            return;
        }

        if (grounded)
        {
            // Walk along the surface and switch gravity off, so slopes and
            // small height drops can't pull the player downhill.
            rb.gravityScale = 0f;
            Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x);
            Vector2 vel = tangent * (inputX * moveSpeed);
            if (inputX != 0) vel -= groundNormal * stickForce; // hug the ground
            rb.linearVelocity = vel;
        }
        else
        {
            rb.gravityScale = gravityScale;
            rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

            // Variable jump height: release early = shorter jump
            if (jumpReleased && rb.linearVelocity.y > 0f)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void CheckGround()
    {
        grounded = false;
        if (ignoreGroundTimer > 0f) return;
        if (rb.linearVelocity.y > 0.1f) return; // still going up — can't be grounded yet

        Bounds b = col.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(
            b.center, new Vector2(b.size.x * 0.9f, b.size.y), 0f,
            Vector2.down, groundCheckDistance, groundLayer);

        if (hit && Vector2.Angle(hit.normal, Vector2.up) <= maxSlopeAngle)
        {
            grounded = true;
            groundNormal = hit.normal;
        }
    }

    private float ReadHorizontal()
    {
#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        if (k == null) return 0f;
        float x = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        return x;
#else
        return Input.GetAxisRaw("Horizontal");
#endif
    }

    private bool JumpPressed()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
#else
        return Input.GetButtonDown("Jump");
#endif
    }

    private bool JumpReleased()
    {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame;
#else
        return Input.GetButtonUp("Jump");
#endif
    }
}