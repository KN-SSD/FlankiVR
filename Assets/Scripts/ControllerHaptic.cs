using UnityEngine;
using UnityEngine.XR;

public class ControllerHaptic : MonoBehaviour
{
    [Header("Która to ręka?")]
    public XRNode controllerNode = XRNode.RightHand;

    [Range(0f, 1f)]
    public float intensity = 0.5f;
    public float duration = 0.1f;

    // Zmienna do zapamiętania, czy spust był wciśnięty w poprzedniej klatce
    private bool wasTriggerPressed = false;

    void Update()
    {
        // Sprawdzamy urządzenie na bieżąco - to bardzo lekka operacja, a w 100% bezpieczna
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        
        if (device.isValid)
        {
            // Odczytujemy aktualny stan triggera
            if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed))
            {
                // Jeśli spust jest WCIŚNIĘTY teraz, ale NIE BYŁ ułamek sekundy temu
                if (isPressed && !wasTriggerPressed)
                {
                    // Wywołujemy naszą własną, publiczną funkcję
                    StartHaptic();
                }
                
                // Zapisujemy obecny stan na potrzeby kolejnej klatki
                wasTriggerPressed = isPressed;
            }
        }
    }

    // Podstawowa funkcja do wywołania z innych skryptów (używa wartości z Inspektora)
    public void StartHaptic()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);
        if (device.isValid)
        {
            device.SendHapticImpulse(0, intensity, duration);
        }
    }

    // Rozszerzona funkcja do wywołania z innych skryptów (możesz podać własną siłę i czas z kodu!)
    public static void StartHaptic(float customIntensity, float customDuration)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (device.isValid)
        {
            device.SendHapticImpulse(0, customIntensity, customDuration);
        }
    }
}