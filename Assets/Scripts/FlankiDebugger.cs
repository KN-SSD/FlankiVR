using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.XR.Interaction.Toolkit.Interactors; 

public class FlankiDebugger : MonoBehaviour
{
    [Header("Referencje")]
    public GameObject centerCan;          
    public XRSocketInteractor socket;   

    [Header("Ustawienia Wybicia")]
    public float debugKnockForce = 5f;    

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            DebugCanToSocket();
        }

        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            DebugKnockCan();
        }
    }

    void DebugCanToSocket()
    {
        if (centerCan == null || socket == null) return;

        socket.enabled = true;
        socket.socketActive = true;

        Rigidbody rb = centerCan.GetComponent<Rigidbody>();
        if (rb)
        {
            
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
            
            rb.isKinematic = false; 
        }

        Transform target = socket.attachTransform != null ? socket.attachTransform : socket.transform;
        centerCan.transform.position = target.position;
        centerCan.transform.rotation = target.rotation;

        Debug.Log("[DEBUG] Puszka postawiona (R).");
        GameManager.Instance.PlayerRunningTurn(); 
        GameManager.Instance.SwitchDrinkVisibility(false);
    }

    void DebugKnockCan()
    {
        if (centerCan == null || socket == null) return;

        StartCoroutine(ForceKnockout());
    }

    IEnumerator ForceKnockout()
    {
        socket.socketActive = false;
        socket.enabled = false;

        Rigidbody rb = centerCan.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            Vector3 randomDir = (Vector3.up + Random.insideUnitSphere).normalized;
            rb.AddForce(randomDir * debugKnockForce, ForceMode.Impulse);
        }

        Debug.Log("[DEBUG] Puszka wybita (K)!");

        yield return new WaitForSeconds(2.0f);
        socket.enabled = true;
        socket.socketActive = true;
    }
}