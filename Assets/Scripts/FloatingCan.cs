using UnityEngine;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class FloatingCan : MonoBehaviour
{
    [SerializeField] private Transform headCamera;
    [SerializeField] private Vector3 offset = new Vector3(0.3f, -0.4f, 0.4f);

    [SerializeField] private float returnSpeed = 8f;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    private Rigidbody rb;
    private bool wasHeld = false;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        if (headCamera == null && Camera.main != null)
            headCamera = Camera.main.transform;

        rb.useGravity = false;
    }

    void Update()
    {
        if (headCamera == null)
            return;

        if (grab.isSelected)
        {
            wasHeld = true;
            if (rb.isKinematic) rb.isKinematic = false;

            return;
        }

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float yawY = headCamera.eulerAngles.y;
        Quaternion bodyRotation = Quaternion.Euler(0, yawY, 0);
        Vector3 targetPosition = headCamera.position + (bodyRotation * offset);

        transform.position = Vector3.Lerp(transform.position, targetPosition, returnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, bodyRotation, returnSpeed * Time.deltaTime);
    }
}