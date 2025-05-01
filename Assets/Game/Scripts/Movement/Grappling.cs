using Unity.VisualScripting;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    // Variables
    [Header("Reference")]
    private Movement pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;


    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overShootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse2;

    private bool grappling;

    private void Start() // Runs at the start of the code
    {
        pm = GetComponent<Movement>();
    }
    private void Update() // Runs every frame
    {
        if(Input.GetKeyDown(grappleKey)) StartGrapple();

        if (grapplingCdTimer > 0) // Countdown for grapple cooldown
            grapplingCdTimer -= Time.deltaTime;
    }
    private void LateUpdate() // Updates grappling position
    {
        if(grappling)
            lr.SetPosition(0, gunTip.position);
    }
    private void StartGrapple() // Checks if player can grapple
    {
        if (grapplingCdTimer > 0) return;
        grappling = true;

        pm.freeze = true;

        RaycastHit hit;
        // Cast grapple if it can reach
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable) && !pm.OnSlope())
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else // Stops grapple if it cant reach
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(ResetGrapple), grappleDelayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1,grapplePoint);
    }
    private void ExecuteGrapple() // How the grapple works
    {
        pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

        pm.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }
    public void StopGrapple() // Resets the grapple
    {
        pm.freeze = false;
        grappling = false;
        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
    public void ResetGrapple() // Resets grapple if object not hit
    {
        pm.freeze = false;
        grappling = false;
        lr.enabled = false;
    }
}
