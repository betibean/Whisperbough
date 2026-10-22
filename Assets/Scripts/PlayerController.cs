using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 3f;

    [Header("Character graphics")]
    [SerializeField] private Transform visualRoot;

    [Header("Character animations")]
    [SerializeField] private Animator animator;

    private Vector2 movement;
    private bool facingLeft;

    void Awake()
    {

    }

    public void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    void Update()
    {
        if (movement.x < 0f) facingLeft = true;
        else if (movement.x > 0f) facingLeft = false;

        if (visualRoot != null)
        {
            Vector3 s = visualRoot.localScale;
            s.x = Mathf.Abs(s.x) * (facingLeft ? -1f : 1f);
            visualRoot.localScale = s;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(movement.x));
        }
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            float dx = movement.x * movementSpeed * Time.fixedDeltaTime;
            float dy = movement.y * movementSpeed * Time.fixedDeltaTime;
            transform.Translate(new Vector3(dx, dy, 0f), Space.World);
        }
    }
}
