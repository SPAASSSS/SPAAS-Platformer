using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    public InputActionReference moveAction;
    public InputActionReference jumpAction;

    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 80f;

    public float jumpForce = 14f;
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    public float coyoteTime = 0.08f;
    public float jumpBuffer = 0.10f;

    public Transform graphics;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        jumpAction?.action.Enable();
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        jumpAction?.action.Disable();
    }

    private void Update()
    {
        moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        bool grounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        coyoteCounter = grounded ? coyoteTime : coyoteCounter - Time.deltaTime;

        if (jumpBufferCounter > 0f) jumpBufferCounter -= Time.deltaTime;
        if (jumpAction != null && jumpAction.action.WasPressedThisFrame())
            jumpBufferCounter = jumpBuffer;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            DoJump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        bool jumpHeld = jumpAction != null && jumpAction.action.IsPressed();
        if (!jumpHeld && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        float moveX = moveInput.x;

        if (moveX > 0.01f)
            graphics.localScale = new Vector3(1, 1, 1);
        else if (moveX < -0.01f)
            graphics.localScale = new Vector3(-1, 1, 1);
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveInput.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate;

        rb.AddForce(new Vector2(movement, 0f));
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -moveSpeed, moveSpeed), rb.linearVelocity.y);
    }

    private void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
