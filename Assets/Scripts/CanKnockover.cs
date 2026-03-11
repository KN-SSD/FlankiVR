using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class CanKnockover : MonoBehaviour
{
    [Header("Ustawienia")]
    public float knockoutForce = 5f; 

    private XRGrabInteractable grab;
    private Rigidbody rb;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] upadki;
    [SerializeField] private GameObject hitEffect;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Rock"))
        {
            if (grab.isSelected && grab.firstInteractorSelecting is XRSocketInteractor socket)
            {
                GameObject effetct = Instantiate(hitEffect.gameObject, collision.gameObject.transform.position,Quaternion.identity);

                VisualEffect eff = effetct.GetComponent<VisualEffect>();
                eff.SendEvent("OnPlay");
               Destroy(effetct,1);

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

        audioSource.clip = upadki[Random.Range(0,upadki.Length)];
        audioSource.Play();
        
        Vector3 impactDir = collision.relativeVelocity.normalized;
        rb.AddForce((impactDir + Vector3.forward) * knockoutForce, ForceMode.Impulse);

        Debug.Log("Puszka wybita! Socket uśpiony.");

        yield return new WaitForSeconds(3.0f);

        socket.enabled = true;
        socket.socketActive = true;
    }
}