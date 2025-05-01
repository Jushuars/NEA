using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    // Variables
    public Transform cameraPosition;

    private void Update()
    {
        // Gets the position from the variable and sets it as the objects position
        transform.position = cameraPosition.position;
    }
}
