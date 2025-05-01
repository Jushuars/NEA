using UnityEngine;

public class Dashing : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    public Transform orientation;
    public Transform playerCam;
    public bool dashGap;
    private Rigidbody rb;
    private Movement pm;
    private Vector3 delayedForceToApply;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;
    public float dashCharges;
    public float maxDashYSpeed;

    [Header("Camera Effects")]
    public PlayerCamera cam;
    public float dashFOV;

    [Header("Settings")]
    public bool useCameraForward = true;
    public bool allowAllDirection = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    public float dashCdTimer;
    public float dashChargeCd;
    public float dashGapCd;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.LeftShift;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Movement>();
        Invoke(nameof(addDashCharge), dashChargeCd);
        dashGap = true;
    }
    private void Update()
    {
        if (dashCharges > 3)
            dashCharges = 3;
        if (Input.GetKeyDown(dashKey) && dashCharges > 0)
            Dash();
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }
    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;
        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        Transform forwardT;

        if (useCameraForward)
            forwardT = playerCam;
        else
            forwardT = orientation;
        Vector3 direction = GetDirection(forwardT);
        Vector3 forceTopApply = direction * dashForce + orientation.up * dashUpwardForce;
        if (disableGravity)
            rb.useGravity = false;
        delayedForceToApply = forceTopApply;

        // Call delayed function after a delayed time
        Invoke(nameof(DelayedDashForce), 0.025f);

        // Starts cooldown
        Invoke(nameof(ResetDash), dashDuration);
    }
    private void DelayedDashForce() // Apply dash force after short time
    {
        if (dashCharges > 0)
            dashCharges = dashCharges - 1;
            if (resetVel)
                rb.linearVelocity = Vector3.zero;
            if (pm.activeGrapple)
                rb.angularVelocity = Vector3.zero;
                pm.activeGrapple = false;
            rb.AddForce(delayedForceToApply, ForceMode.Impulse);
            cam.DoFov(dashFOV);
    }
    private void ResetDash() // Resets dash
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;
        cam.DoFov(cam.baseFOV);

        if (disableGravity) rb.useGravity = true;
        dashGap = false;
        Invoke(nameof(DashGapReset), dashGapCd);
    }
    private void DashGapReset()
    {
        dashGap = true;
    }
    private void addDashCharge() // Add dash Charges
    {
        if (dashCharges < 3)
            dashCharges = dashCharges + 1;
        Invoke(nameof(addDashCharge), dashChargeCd);
    }
    
    private Vector3 GetDirection(Transform forwardT) // Allows dashing in multiple directions
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirection) // Allow dashing in 8 direction (if true)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;
        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;
        return direction.normalized;
    }
}
