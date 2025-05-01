using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    // Variables
    [Header("walking")] // These Variables will be movement related
    public float walkSpeed;
    public float moveSpeed;
    public float groundDrag;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    [Header("Sliding")]
    public float slideSpeed;
    public bool sliding;
    public float slopeIncreaseMultiplier;
    [Header("Wall Running")]
    public float wallRunSpeed;
    public bool wallRunning;
    [Header("Dashing")]
    public float dashSpeed;
    public float dashSpeedChangeFactor;
    public bool dashing;
    [Header("Grappling")]
    public bool activeGrapple;

    [Header("Keybinds")] // Variables keep track of the keybinds for different actions
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")] // List of variables which are ground check related
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")] // Variables to check the variables of the slope below the player
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool exitingSlope;

    [Header("Script Reference")] // Links other scripts to this script
    private Dashing ds;
    private Sliding ss;
    public PlayerCamera cam;

    [Header("Camera effects")] // Camera related variables
    public float grappleFov;

    [Header("Movement Related")] // Variables which are related to other uses
    public Transform orientation;
    public MovementState state;
    private MovementState lastState;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    public Rigidbody rb;
    public float baseSpeedIncreaseMultiplier;
    public float speedIncreaseMultiplier;
    public float desiredMoveSpeed;
    public float lastDesiredMoveSpeed;
    public float maxYSpeed;
    private float startYScale;
    private bool keepMomentum;
    public bool freeze;

    public enum MovementState // Holds the possible states the player can be in
    {
        walking,
        grounded,
        air,
        sliding,
        dashing,
        wallRunnning,
        freeze,
    }

    private void Start()  // Is called at the start of the program
    { 
        rb = GetComponent<Rigidbody>(); // Prevents player from falling over
        rb.freezeRotation = true;
        ds = GetComponent<Dashing>();
        ss = GetComponent<Sliding>();
        grounded = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
        speedIncreaseMultiplier = baseSpeedIncreaseMultiplier;
        StartCoroutine(SmoothlyLerpMoveSpeed());
    }

    private void Update() // Calls every Frame Updates
    {
        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // Drag handle
        if (grounded && !dashing && !activeGrapple)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        // Gets the X & Z input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        // When jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump(); 
            Invoke(nameof(ResetJump), jumpCooldown); // Resets jump after jump cooldown
        }
    }
    private void StateHandler()
    {
        // Freeze state
        if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.linearVelocity = Vector3.zero;
        }
        // Dashing state
        else if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedIncreaseMultiplier = dashSpeedChangeFactor;
        }
        // Wallrunning state
        else if (wallRunning)
        {
            state = MovementState.wallRunnning;
            desiredMoveSpeed = wallRunSpeed;
        }
        // Sliding state
        else if (sliding)
        {
            state = MovementState.sliding;
            // Sets desired speed based on if you're going down a slope
            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = walkSpeed;
            speedIncreaseMultiplier = baseSpeedIncreaseMultiplier;
        }
        // Walking state
        else if (grounded && (verticalInput != 0 || horizontalInput != 0))
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        // In air state
        else if (!grounded & !wallRunning)
        {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed;
        }
        // Grounded state
        else
        {
            state = MovementState.grounded;
            desiredMoveSpeed = walkSpeed;
        }
        // Checks if desired move speed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed !=0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
            moveSpeed = desiredMoveSpeed;

        bool desiredMoveSpeedHasChanged = (desiredMoveSpeed != lastDesiredMoveSpeed);

        if (lastState == MovementState.dashing || lastState == 0) keepMomentum = true; 

        if (desiredMoveSpeedHasChanged)
        {
            // Used to either smoothly change speed or make it instant
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // Smoothly changes speed to desired move speed
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostValue = speedIncreaseMultiplier;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, (time / difference));
            if (OnSlope()) // Increase speed more based on slope angle
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 0.5f + (slopeAngle / 180f);

                time += Time.deltaTime * boostValue * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * boostValue;

            yield return null;
        }
        moveSpeed = desiredMoveSpeed;
    }
    private void MovePlayer()
    {
        if (activeGrapple) return; // Stops function if grappling

        if (state == MovementState.dashing || !ds.dashGap) return;
        // Calculates direction of movement (Move according to direction facing)
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Moves the player (On Slope)
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDiection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // Moves the player (On ground)
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        // Moves the player (In Air)
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        // turn gravity off while on slope to prevent sliding
        if(!wallRunning)
            rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return; // Stops function if grappling

        // Limit speed on slope
        if(OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }
        // Limit speed on ground or air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
        // Limit Y vel
        if(maxYSpeed != 0 && rb.linearVelocity.y > maxYSpeed)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, maxYSpeed, rb.linearVelocity.z);
    }
    private void Jump()
    {
        // Allow player to jump on slope
        exitingSlope = true;
        // Resets Y velocity to 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        // Force created when jumping
        if(lastState == MovementState.sliding)
            rb.AddForce(transform.up * jumpForce * ss.jumpMultiplier, ForceMode.Impulse);
        else if(exitingSlope && lastState == MovementState.sliding)
            rb.AddForce(transform.up * jumpForce * ss.jumpMultiplier * 2f, ForceMode.Impulse);
        else 
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }
    private void ResetJump() // Allows player to keep jumping
    {
        readyToJump = true;
        exitingSlope = false;
    }
    public bool OnSlope() // Calculates angle of slope and adjusts movement accordingly
    {
        if (activeGrapple) return false; // Stops function if grappling

        // Raycasts below player to check the slopes angle
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    public Vector3 GetSlopeMoveDiection(Vector3 direction) // adjusts players movement to the slopes angle
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
    private void OnCollisionEnter(Collision collision) // Allows player to move again
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }
    private void ResetRestrictions() // Resets the players grapple effects
    {
        activeGrapple = false;
        cam.DoFov(cam.baseFOV);
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight) // Calculates velocity for grappling
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private Vector3 velocityToSet;
    private bool enableMovementOnNextTouch;
    private void SetVelocity() // Changes the players velocity
    {
        enableMovementOnNextTouch = true;
        rb.linearVelocity = velocityToSet;
        rb.angularVelocity = velocityToSet;

        cam.DoFov(grappleFov);
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight) // Moves player towards the position of the grapple
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 2f);
    }
}
