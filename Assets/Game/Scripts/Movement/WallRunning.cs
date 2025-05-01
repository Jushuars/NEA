using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    // Variables
    [Header("Wall Running")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("Reference")]
    public Transform orientation;
    public PlayerCamera cam;
    public float camTilt;
    public float wallRunFOV;
    private Movement pm;
    private Rigidbody rb;

    private void Start() 
    {
        // Assigns the variables to already existing components
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Movement>();
    }
    private void Update()
    {
        // Will call every frame update
        CheckForWall();
        StateMachine();
    }
    private void FixedUpdate()
    {
        if (pm.wallRunning)
            WallRunningMovement();
    }
    private void CheckForWall()
    {
        // Ray casts to the left & right of the player
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }
    private bool AboveGround() // Checks how far the player is above ground
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }
    private void StateMachine() // Checks the state of the player
    {
        // Getting Inputs
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // State 1 - Wallrunning
        if((wallLeft || wallRight) && verticalInput > 0 & AboveGround() && !exitingWall)
        {
            // Start wallrun
            if (!pm.wallRunning)
                StartWallRun();
            // Wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            if(wallRunTimer <= 0 && pm.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
            // Wall Jump
            if (Input.GetKeyDown(jumpKey))
                WallJump();
        }
        // State 2 - Exiting
        else if (exitingWall)
        {
            if(pm.wallRunning)
                StopWallRun();
            // Counts down timer
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        // State 3 - None
        else
        {
            if (pm.wallRunning)
                StopWallRun();
        }
    }
    private void StartWallRun() // Starts the process of wall running
    {
        pm.wallRunning = true;
        useGravity = true;

        wallRunTimer = maxWallRunTime;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // Apply camera effect
        cam.DoFov(wallRunFOV);
        if (wallLeft) cam.DoTilt(-camTilt);
        if (wallRight) cam.DoTilt(camTilt);
    }
    private void WallRunningMovement() // Moves the player for the wall run
    {
        rb.useGravity = useGravity;
        // Finds the direction of the wall
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;
        // Forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        // Push when player wall running on outside curve
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallForward * 100, ForceMode.Force);
        // Weaken gravity
        if(useGravity)
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }
    private void StopWallRun() // Stops all processes involving wall running
    {
        pm.wallRunning = false;
        useGravity = false;

        // Reset camera effects
        cam.DoFov(cam.baseFOV);
        cam.DoTilt(0f);
    }
    private void WallJump()
    {
        // Enter exiting state
        exitingWall = true;
        exitWallTimer = exitWallTime;
        // Checks direction the force should be applied
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        // Force added & reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
