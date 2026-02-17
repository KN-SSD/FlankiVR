using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CanSnappedScript : MonoBehaviour
{
    private XRSocketInteractor socket;


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
        GameManager.Instance.SwitchCanState(false);
        Debug.Log("STOP! Puszka stoi - koniec picia!");        
    }
}
