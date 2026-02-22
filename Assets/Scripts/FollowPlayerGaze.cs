using UnityEngine;

public class FollowPlayerGaze : MonoBehaviour
{
    public Transform playerCamera; 

    public float distance = 1.5f;  
    public float fixedY = 1.2f;    
    public float smoothSpeed = 5f; 

    void LateUpdate()
    {
        if (playerCamera == null) return;

        Vector3 flatForward = playerCamera.forward;
        flatForward.y = 0;
        
        if (flatForward.sqrMagnitude > 0.001f) 
        {
            flatForward.Normalize();
        }
        else 
        {
            flatForward = playerCamera.up; 
            flatForward.y = 0;
            flatForward.Normalize();
        }

        Vector3 targetPosition = playerCamera.position + (flatForward * distance);
        targetPosition.y = fixedY; 

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

        Vector3 lookDirection = transform.position - playerCamera.position;
        lookDirection.y = 0; 
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
}