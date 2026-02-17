using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SmartSocketAligner : MonoBehaviour
{
    private XRSocketInteractor socket;
    
    public Transform socketAttachPoint; 

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        socket.hoverEntered.AddListener(OnHoverEntered);
    }

    void OnDisable()
    {
        socket.hoverEntered.RemoveListener(OnHoverEntered);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Transform canTransform = args.interactableObject.transform;
        float angle = Vector3.Angle(canTransform.up, Vector3.up);

        if (angle > 90f)
            socketAttachPoint.localRotation = Quaternion.Euler(180, 0, 0);
        else
            socketAttachPoint.localRotation = Quaternion.Euler(0, 0, 0);
    }
}