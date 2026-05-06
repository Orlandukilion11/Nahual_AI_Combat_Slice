using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Stats")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 15f;
    public float jumpForce = 7f; // Increased slightly for better feel

    [Header("Water Stats")]
    public float swimForce = 15f;    // Force to push up (Buoyancy)
    public float swimSpeed = 4f;     // Slower movement in water
    public float waterDrag = 3f;     // "Thick" feeling in water

    [Header("Funsies (Interaction)")]
    public float pushPower = 2.0f;

    [Header("References")]
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isGrounded; // The "Safety Switch" for jumping
    private bool inWater; // Are we swimming?

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Setup Physics for Character
        rb.freezeRotation = true; // Prevent tipping over
        rb.linearDamping = 1f;    // Prevent sliding like ice - normal air resistance

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 0. Safety net (void fix): if we fall too deep, teleport back to the beach
        if (transform.position.y < -20f)
        {
            transform.position = new Vector3(0, 12, 0); // Adjust Y to be safe
            rb.linearVelocity = Vector3.zero; // Stop falling instantly
        }

        // 1. Input Processing
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. Camera Direction Math
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * v + camRight * h).normalized;

        // 3. JUMP & SWIM LOGIC
        // We only jump if we press Space AND we are touching the floor NOT the water
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && !inWater)
            {
                Jump(jumpForce); // Normal Jump
            }
        }

        // While holding Space in water, swim up continuously
        if (Input.GetKey(KeyCode.Space) && inWater)
        {
            rb.AddForce(Vector3.up * swimForce, ForceMode.Acceleration);
        }
    }

    void FixedUpdate()
    {
        Move();

        // Apply artificial Gravity/Buoyancy check
        if (inWater)
        {
            // Apply a gentle upward force to simulate water holding you up
            // This prevents sinking like a stone
            //rb.AddForce(Vector3.up * 5f, ForceMode.Acceleration);

            // We apply a tiny downward force so he drifts to the bottom like a real diver.
            rb.AddForce(Vector3.down * 3f, ForceMode.Acceleration);
        }
    }

    void Move()
    {
        float currentSpeed = inWater ? swimSpeed : moveSpeed;
        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotate
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            // Move
            Vector3 targetVelocity = moveDirection * currentSpeed;
            targetVelocity.y = rb.linearVelocity.y; // Preserve Gravity
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Stop Horizontal slide, keep Vertical gravity
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Jump(float force)
    {
        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        isGrounded = false; // We are in the air now!
    }

    // --- PHYSICS INTERACTIONS ---

    // 0. Water detection
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = true;
            rb.linearDamping = waterDrag; // High drag (hard to move fast)
            rb.useGravity = false; // Turn off real gravity so we can fake buoyancy
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            inWater = false;
            rb.linearDamping = 1f; // Reset to air drag
            rb.useGravity = true;  // Turn real gravity back on
        }
    }

    // 1. GROUND CHECK (Detects if we can jump)
    // This runs every frame we are touching SOMETHING
    private void OnCollisionStay(Collision collision)
    {
        // Check if the thing we are touching is generally "below" us
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                isGrounded = true;
                break; // Found ground, stop checking
            }
        }

        // 2. PUSHING LOGIC (Combined here)
        Rigidbody body = collision.rigidbody;
        if (body != null && !body.isKinematic)
        {
            Vector3 pushDir = collision.transform.position - transform.position;
            pushDir.y = 0; // Don't push them into the floor
            pushDir.Normalize();
            body.AddForce(pushDir * pushPower, ForceMode.Force);
        }
    }

    // If we leave the ground (fall off a cliff), stop jumping
    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}