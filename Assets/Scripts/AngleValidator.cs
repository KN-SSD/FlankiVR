using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AngleValidator : MonoBehaviour, IXRSelectFilter
    {
    [SerializeField] private float maxAngle = 45f;

    public bool canProcess => isActiveAndEnabled;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        float angle = Vector3.Angle(interactable.transform.up, Vector3.up);
        
        bool isUpright = angle < maxAngle;

        bool isUpsideDown = angle > (180f - maxAngle);

        return isUpright || isUpsideDown;
    }
}