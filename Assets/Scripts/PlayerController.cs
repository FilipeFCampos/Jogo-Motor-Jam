using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalVelocity;
    public float speed = 5f;
    public float jumpForce = 10f;
    private bool isFacingRight = true;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal input
        horizontalVelocity = Input.GetAxisRaw("Horizontal");

        // Handle jumping
        if (Input.GetButtonDown("Jump") && IsGrounded()) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        Flip();
    }

    // Check if the player is grounded
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.4f, groundLayer);
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalVelocity * speed, rb.linearVelocity.y);
    }
    
    // Flip the player character based on the direction of movement
    private void Flip()
    {
        if (isFacingRight && horizontalVelocity < 0f || !isFacingRight && horizontalVelocity > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
