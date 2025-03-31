using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    
    // Ground check
    public float rayLength = 0.6f;
    private bool isGrounded;
    
    // Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Debug.Log("Found sprite renderer in child: " + (spriteRenderer != null));
        }
        
        // Configure Rigidbody to prevent rotation
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        Debug.Log("CompletePlatformerController initialized");
    }
    
    void Update()
    {
        // Get horizontal input
        float moveInput = Input.GetAxisRaw("Horizontal");
        
        // Move the character
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on movement direction
        if (moveInput > 0)
        {
            // Moving right
            if (spriteRenderer != null) spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            // Moving left
            if (spriteRenderer != null) spriteRenderer.flipX = true;
        }
        
        // Ground check
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, rayLength);
        isGrounded = hit.collider != null;
        
        // Jump
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton3)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset y velocity
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("Jump executed!");
        }

        
        // Visual debug
        Debug.DrawRay(transform.position, Vector2.down * rayLength, isGrounded ? Color.green : Color.red);
    }
    
    void FixedUpdate()
    {
        // Force upright position - prevents tilting/leaning
        transform.rotation = Quaternion.identity;
        
        // Ensure Z position is zero (common 2D issue)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}