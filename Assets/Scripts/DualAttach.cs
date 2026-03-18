using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DualAttach : MonoBehaviour
{
    [Header("Ustawienia Punktów")]
    [SerializeField] private Transform handAttachPoint;   
    [SerializeField] private Transform socketAttachPoint; 

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        
        grabInteractable.attachTransform = handAttachPoint;
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
            grabInteractable.attachTransform = socketAttachPoint;
        else
            grabInteractable.attachTransform = handAttachPoint;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        grabInteractable.attachTransform = handAttachPoint;
    }
}