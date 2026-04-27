using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public float moveSpeed = 100f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;

    private PlayerControl controls;

    void Awake()
    {
        controls = new PlayerControl();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
        if (moveInput!= Vector2.zero)
        {
            if (moveInput.x < 0) GetComponent<SpriteRenderer>().flipX = true;
            if (moveInput.x > 0) GetComponent<SpriteRenderer>().flipX = false;
            
            animator.Play("Walk Player");
        } else
        {
            animator.Play("Idle Player");
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
}
