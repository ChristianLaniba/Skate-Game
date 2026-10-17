using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Movement (WASD)")]
    public float moveAcceleration = 15f;
    public float walkMaxSpeed = 6f;
    public float coastDeceleration = 8f;
    public float turnSpeed = 125f;
    public float carveStrength = 1f;

    [Header("Kick (Shift)")]
    public float boostMaxSpeed = 25f;         // speed the board is instantly set to
    public float boostCooldown = 1f;          // seconds before you can kick again
    public float overspeedDeceleration = 15f; // how fast the kick fades back down

    [Header("Jumping")]
    public float jumpForce = 175f;
    public float jumpBufferTime = 0.2f;
    public float groundRayDistance = 0.8f;

    [Header("Air Control")]
    public float airLevelSpeed = 5f;

    [Header("Spin (Mouse Flick)")]
    public float flickThreshold = 15f;
    public float spinSpeed = 20f;
    public float spinCooldown = 0.3f;

    [Header("Trick (E) - 360 Horizontal Spin")]
    public float trickSpinSpeed = 720f;
    public float trickCooldown = 0.2f;

    public bool IsDoingTrick => trickRemaining > 0f;

    private Rigidbody rb;
    private bool isGrounded;
    private float jumpBufferTimer;
    private float lastSpinTime;

    private bool kickQueued;
    private float kickCooldownTimer;

    private bool trickQueued;
    private float trickRemaining;
    private float trickDirection = 1f;
    private float lastTrickTime = -999f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Ignore input while paused
        if (Time.timeScale == 0f) return;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            kickQueued = true;

        if (Input.GetKeyDown(KeyCode.E))
            trickQueued = true;
    }

    void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // --- Ground check ---
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundRayDistance);

        float turnInput = Input.GetAxis("Horizontal");
        float moveInput = Input.GetAxis("Vertical");

        // --- Kick (instant speed) ---
        if (kickCooldownTimer > 0f) kickCooldownTimer -= dt;

        if (kickQueued)
        {
            if (kickCooldownTimer <= 0f && isGrounded)
            {
                // Kick backward only if S is held, otherwise forward
                float dir = moveInput < -0.01f ? -1f : 1f;
                Vector3 kickVel = transform.forward * (dir * boostMaxSpeed);
                rb.linearVelocity = new Vector3(kickVel.x, rb.linearVelocity.y, kickVel.z);
                kickCooldownTimer = boostCooldown;
            }
            kickQueued = false;
        }

        // --- Trick start (E) ---
        if (trickQueued)
        {
            if (!IsDoingTrick && Time.time - lastTrickTime > trickCooldown)
            {
                trickRemaining = 360f;
                trickDirection = turnInput < -0.01f ? -1f : 1f;
                lastTrickTime = Time.time;
            }
            trickQueued = false;
        }

        // --- Trick rotation (360 yaw) ---
        if (IsDoingTrick)
        {
            float step = Mathf.Min(trickSpinSpeed * dt, trickRemaining);
            transform.Rotate(0f, trickDirection * step, 0f, Space.World);
            trickRemaining -= step;
        }

        // --- Turning (ground only, paused during trick) ---
        if (isGrounded && !IsDoingTrick)
        {
            transform.Rotate(0f, turnInput * turnSpeed * dt, 0f);
        }

        // --- Carve (paused during trick) ---
        if (isGrounded && !IsDoingTrick && Mathf.Abs(turnInput) > 0.01f)
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.sqrMagnitude > 0.001f)
            {
                float forwardDot = Vector3.Dot(flatVel.normalized, transform.forward);
                Vector3 travelDir = forwardDot >= 0f ? transform.forward : -transform.forward;
                Vector3 redirected = travelDir * flatVel.magnitude;
                Vector3 newFlatVel = Vector3.Lerp(flatVel, redirected, carveStrength);
                rb.linearVelocity = new Vector3(newFlatVel.x, rb.linearVelocity.y, newFlatVel.z);
            }
        }

        // --- Ground movement: slow WASD, kick fades back down ---
        if (isGrounded)
        {
            bool hasMoveInput = Mathf.Abs(moveInput) > 0.01f;

            Vector3 flat = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float fwdSpeed = Vector3.Dot(flat, transform.forward);

            float targetSpeed = hasMoveInput ? moveInput * walkMaxSpeed : 0f;

            float rate = hasMoveInput ? moveAcceleration : coastDeceleration;
            if (Mathf.Abs(fwdSpeed) > walkMaxSpeed)
                rate = overspeedDeceleration;   // fading out of a kick

            fwdSpeed = Mathf.MoveTowards(fwdSpeed, targetSpeed, rate * dt);

            Vector3 newFlat = transform.forward * fwdSpeed;
            rb.linearVelocity = new Vector3(newFlat.x, rb.linearVelocity.y, newFlat.z);
        }

        // --- Flick spin (mouse, airborne) ---
        if (!isGrounded && Input.GetMouseButton(0) && Time.time - lastSpinTime > spinCooldown)
        {
            float mouseX = Input.GetAxis("Mouse X");

            if (Mathf.Abs(mouseX) > flickThreshold * dt)
            {
                rb.angularVelocity = transform.forward * (-mouseX * spinSpeed);
                lastSpinTime = Time.time;
            }
        }

        // --- Jump (buffered) ---
        if (jumpBufferTimer > 0f)
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                jumpBufferTimer = 0f;
            }
            else
            {
                jumpBufferTimer -= dt;
            }
        }

        // --- Auto-level (only when not spinning or doing a trick) ---
        if (rb.angularVelocity.magnitude < 1f && !IsDoingTrick)
        {
            float levelStrength = isGrounded ? airLevelSpeed * 0.2f : airLevelSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, Vector3.up),
                Vector3.up
            );

            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, levelStrength * dt));
        }
    }
}