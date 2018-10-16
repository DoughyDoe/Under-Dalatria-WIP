using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTarget;
    private float smoothSpeed = 10f;
    public Vector3 offset;


    void FixedUpdate()
    {
        //offset is for the Z position to be infront of the player instead of inside
        Vector3 desiredPosition = playerTarget.position + offset;
        // takes your current position desired position and how fast you want to go between the two points
        // Time.deltaTime makes it so that the smoothing speed is frame independant
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); 
        transform.position = smoothedPosition;
    }

}
