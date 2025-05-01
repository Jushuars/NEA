using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;

public class Sliding : MonoBehaviour
{
    // Variables
    [Header("Reference")] // Variables giving the sliding a reference to work
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private Movement pm;

    [Header("Sliding")] // Variables to adjust sliding affects
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale; // Slide height
    private float startYScale; // Normal height
    public float jumpMultiplier; 

    [Header("Inputs")]
    public KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;


    private void Start() // Sets variables values on start of game
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<Movement>();
        startYScale = playerObj.localScale.y;
    }
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A + D keys
        verticalInput = Input.GetAxisRaw("Vertical"); // W + S keys
        // checks if slide keys is pressed and starts sliding if it does
        if (Input.GetKeyDown(KeyCode.LeftControl) && verticalInput == 1)
            StartSlide();
        else if ((Input.GetKeyUp(KeyCode.LeftControl) || verticalInput != 1) && pm.sliding)
            StopSlide();
    }
    private void FixedUpdate()
    {
        // Will run the sliding force when the player is sliding
        if(pm.sliding)
            SlidingMovement();
    }
    private void StartSlide() // Sets player to sliding
    {
        pm.sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }
    private void SlidingMovement() // Applies the force on the player when sliding
    {
        Vector3 inputDircetion = orientation.forward * verticalInput + orientation.right * horizontalInput; // Allows sliding in other directions
        // Normal sliding
        if(!pm.OnSlope() || rb.linearVelocity.y > -0.1f)
        {
            rb.AddForce(inputDircetion.normalized * slideForce, ForceMode.Force);
            slideTimer -=Time.deltaTime;
        }
        // Slope Sliding
        else
        {
            rb.AddForce(pm.GetSlopeMoveDiection(inputDircetion) * slideForce, ForceMode.Force);
        }
        // Stops sliding when slide timer reaches 0
        if (slideTimer <= 0)
            StopSlide();
    }
    private void StopSlide() // Stops all code involving sliding
    {
        pm.sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z); // Resets players y scale
    }
}
