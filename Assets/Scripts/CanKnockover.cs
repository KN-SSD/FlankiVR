using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class CanKnockover : MonoBehaviour
{
    [Header("Ustawienia")]
    public string rockTag = "Rock"; 
    public float knockoutForce = 5f; 

    private XRGrabInteractable grab;
    private Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(rockTag))
        {
            if (grab.isSelected && grab.firstInteractorSelecting is XRSocketInteractor socket)
            {
                StartCoroutine(DisableSocketAndYeet(socket, collision));
            }
        }
    }

    IEnumerator DisableSocketAndYeet(XRSocketInteractor socket, Collision collision)
    {
        
        socket.socketActive = false; 
        socket.enabled = false;      

        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 impactDir = collision.relativeVelocity.normalized;
        rb.AddForce((impactDir + Vector3.up) * knockoutForce, ForceMode.Impulse);

        Debug.Log("Puszka wybita! Socket uśpiony.");

        yield return new WaitForSeconds(3.0f);

        socket.enabled = true;
        socket.socketActive = true;
    }
}