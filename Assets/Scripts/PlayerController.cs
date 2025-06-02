using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /* Basic player movement variables */
    private Vector2 moveDir;
    [SerializeField] private float moveSpeed = 200f;

    /* References to components */
    [SerializeField] private Rigidbody2D rb;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the PlayerController GameObject.");
        }

        moveDir = Vector2.zero;
    }

    // Handle player input
    private void GetInput()
    {
        moveDir.Set(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); // Might change to Input.GetAxisRaw for instant response
    }

    private void MovePlayer()
    {
        rb.linearVelocity = moveSpeed * Time.fixedDeltaTime * moveDir;
    }

    // Update is called once per frame
    void Update()
    {
        // Get horizontal input
        GetInput();
    }

    // FixedUpdate is called at a fixed interval and is used for physics calculations
    private void FixedUpdate()
    {
        // Move the player based on input
        MovePlayer();
    }

}