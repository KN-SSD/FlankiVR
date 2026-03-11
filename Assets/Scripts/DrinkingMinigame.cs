using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DrinkingMinigame : MonoBehaviour
{
    [Header("--- Stan Napoju ---")]
    [Tooltip("Maksymalna prędkość picia, osiągana przy pełnym spamowaniu")]
    public float maxDrinkingRate = 25f;        

    [Header("--- Mechanika Spamowania ---")]
    public float pressure = 0f;             
    public float maxPressure = 100f; // Twardy limit ciśnienia (i prędkości picia)
    public float pressureGain = 15f;        
    public float pressureDecay = 25f;       

    [Header("--- Wykrywanie Ust ---")]
    public float headDistanceLimit = 0.3f;  
    public float heightOffset = 0.15f;      
    public float requiredTilt = 80f;        

    [Header("--- UI i Input ---")]
    public Slider liquidSlider;             
    public InputActionProperty triggerInput; 

    private XRGrabInteractable grab;
    private Transform headCamera;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (Camera.main != null) headCamera = Camera.main.transform;

        if (liquidSlider) liquidSlider.maxValue = GameManager.Instance.playersDrinkLeft;
    }

    void OnEnable() 
    {
        if (triggerInput.action != null) triggerInput.action.Enable();
    }

    void Update()
    {
        DecayPressure();
        UpdateUI();

        if (!grab.isSelected) return;

        // Nabijanie ciśnienia przy każdym wciśnięciu
        if (triggerInput.action.WasPressedThisFrame())
        {
            pressure += pressureGain;
            // Blokujemy ciśnienie na maksymalnym poziomie (żeby nie rosło w nieskończoność)
            pressure = Mathf.Min(pressure, maxPressure);
        }

        if (IsCanAtMouth())
        {
            ProcessDrinking();
        }
    }

    void ProcessDrinking()
    {
        if (pressure > 0f)
        {
            // Prędkość picia to procent z maxDrinkingRate (od 0 do 100% maxa)
            float currentDrinkSpeed = (pressure / maxPressure) * maxDrinkingRate;
            
            GameManager.Instance.playersDrinkLeft -= currentDrinkSpeed * Time.deltaTime;

            // Zabezpieczenie przed ujemną wartością napoju
            if (GameManager.Instance.playersDrinkLeft < 0f)
            {
                GameManager.Instance.playersDrinkLeft = 0f;
            }
        }
    }

    bool IsCanAtMouth()
    {
        if (headCamera == null) return false;

        float distanceToHead = Vector3.Distance(transform.position, headCamera.position);
        bool heightOK = transform.position.y > (headCamera.position.y - heightOffset);
        float angle = Vector3.Angle(transform.up, Vector3.up); 
        bool tiltOK = angle > requiredTilt;

        return (distanceToHead < headDistanceLimit) && heightOK && tiltOK;
    }

    void DecayPressure()
    {
        pressure -= pressureDecay * Time.deltaTime;
        pressure = Mathf.Clamp(pressure, 0f, maxPressure);
        
        // Wibracja też jest teraz dostosowana do nowego maxPressure
        ControllerHaptic.StartHaptic(pressure / maxPressure, 0.1f);
    }

    void UpdateUI()
    {
        if (liquidSlider) liquidSlider.value = GameManager.Instance.playersDrinkLeft;
    }
}