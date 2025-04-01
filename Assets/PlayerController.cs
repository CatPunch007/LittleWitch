using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // Movement settings
    public float moveSpeed = 5f;
    public float jumpForce = 15f;
    
    // Dash settings
    public float dashSpeed = 12f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;
    
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
        
        // Check for dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && moveInput != 0)
        {
            StartCoroutine(Dash(moveInput));
        }
        
        // Move the character (if not dashing)
        if (!isDashing)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
        
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
    
    IEnumerator Dash(float direction)
    {
        // Start dash
        Debug.Log("Dash executed!");
        isDashing = true;
        canDash = false;
        
        // Store original gravity
        float originalGravity = rb.gravityScale;
        
        // Reduce gravity during dash (optional, makes dash feel more "horizontal")
        rb.gravityScale = 0.5f;
        
        // Apply dash velocity
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0);
        
        // Optional - create dash effect
        // Instantiate(dashEffectPrefab, transform.position, Quaternion.identity);
        
        // Wait for dash duration
        yield return new WaitForSeconds(dashDuration);
        
        // End dash
        isDashing = false;
        rb.gravityScale = originalGravity;
        
        // Start cooldown
        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }
    
    void FixedUpdate()
    {
        // Force upright position - prevents tilting/leaning
        transform.rotation = Quaternion.identity;
        
        // Ensure Z position is zero (common 2D issue)
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}