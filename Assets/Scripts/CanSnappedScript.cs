using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CanSnappedScript : MonoBehaviour
{
    private XRSocketInteractor socket;
    [SerializeField] private VisualEffect effect;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
       
    }

    void OnEnable()
    {
        socket.selectEntered.AddListener(OnCanPlaced);
    }

    private void OnCanPlaced(SelectEnterEventArgs args)
    {
        GameManager.Instance.isCanDown = false;
        effect.SendEvent("CanPlaced");

    }
}
