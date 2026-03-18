using UnityEngine;
using UnityEngine.XR;

public class ControllerHaptic : MonoBehaviour
{
    [SerializeField] private XRNode controllerNode = XRNode.RightHand;

    [Range(0f, 1f)]
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private float duration = 0.1f;

    private bool wasTriggerPressed = false;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);

        if (device.isValid)
        {
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed))
            {
                if (isPressed && !wasTriggerPressed)
                    StartHaptic();

                wasTriggerPressed = isPressed;
            }
        }
    }

    public void StartHaptic()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid)
            device.SendHapticImpulse(0, intensity, duration);
    }

    public static void StartHaptic(float customIntensity, float customDuration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (device.isValid)
            device.SendHapticImpulse(0, customIntensity, customDuration);
    }
}