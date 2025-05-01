using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour
{
    // Variables
    public float sensX;
    public float sensY;
    public float baseFOV;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private void Start()
    {
        // Locks cursor in middle of screen and makes it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DoFov(baseFOV);
    }

    private void Update()
    {
        // Gets mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        // Rotates the camera
        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Locks camera from rotating past 90 degrees

        // Rotates camera and rotation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }
    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }   
}
