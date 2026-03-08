using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DrinkingMinigame : MonoBehaviour
{
    [Header("--- Stan Napoju ---")]
    public float drinkingRate = 15f;        

    [Header("--- Mechanika Spamowania ---")]
    public float pressure = 0f;             
    public float pressureGain = 15f;        
    public float pressureDecay = 25f;       
    
    [Header("--- Strefy Paska (0-100) ---")]
    public float goodZoneMin = 40f;         
    public float goodZoneMax = 75f;        
    public float chokeThreshold = 95f;      
    public float chokePenaltyTime = 2.0f;   

    [Header("--- Wykrywanie Ust ---")]
    public float headDistanceLimit = 0.3f;  
    public float heightOffset = 0.15f;      
    public float requiredTilt = 80f;        

    [Header("--- UI i Input ---")]
    public Slider liquidSlider;             
    public Slider pressureSlider;           
    public Image pressureFillColor;         
    public InputActionProperty triggerInput; 

    private XRGrabInteractable grab;
    private Transform headCamera;
    private bool isChoked = false;
    private float chokeTimer = 0f;

    void Start()
    {
        grab = GetComponent<XRGrabInteractable>();
        if (Camera.main != null) headCamera = Camera.main.transform;

        if (liquidSlider) liquidSlider.maxValue = GameManager.Instance.playersDrinkLeft;
        if (pressureSlider) pressureSlider.maxValue = 100f;
    }

    void OnEnable() 
    {
        if (triggerInput.action != null) triggerInput.action.Enable();
    }

    void Update()
    {
        DecayPressure();
        UpdateUI();

        if (isChoked)
        {
            HandleChokeTimer();
            return; 
        }

        if (!grab.isSelected) return;

        if (triggerInput.action.WasPressedThisFrame())
        {
            pressure += pressureGain;
        }

        if (IsCanAtMouth())
        {
            ProcessDrinking();
        }
    }

    void ProcessDrinking()
    {
        if (pressure > chokeThreshold)
        {
            StartChoke();
        }
        else if (pressure >= goodZoneMin && pressure <= goodZoneMax)
        {
            GameManager.Instance.playersDrinkLeft -= drinkingRate * Time.deltaTime;
        }
    }

    bool IsCanAtMouth()
    {
        if (headCamera == null) return false;

        float distanceToHead = Vector3.Distance(transform.position, headCamera.position);
        
        bool heightOK = transform.position.y > (headCamera.position.y - 0.25f);

        float angle = Vector3.Angle(transform.up, Vector3.up); 
        bool tiltOK = angle > requiredTilt;

        return (distanceToHead < headDistanceLimit) && heightOK && tiltOK;
    }

    void DecayPressure()
    {
        pressure -= pressureDecay * Time.deltaTime;
        pressure = Mathf.Clamp(pressure, 0f, 100f);
        ControllerHaptic.StartHaptic(pressure/100f, 0.1f);
    }

    void StartChoke()
    {
        isChoked = true;
        chokeTimer = chokePenaltyTime;
        pressure = 0f; 
        Debug.Log("ZAKRZTUSIŁEŚ SIĘ! (BLOKADA)");
        
        if (pressureFillColor) pressureFillColor.color = Color.red;
    }

    void HandleChokeTimer()
    {
        chokeTimer -= Time.deltaTime;
        if (chokeTimer <= 0)
        {
            isChoked = false;
        }
    }

    void UpdateUI()
    {
        if (liquidSlider) liquidSlider.value = GameManager.Instance.playersDrinkLeft;
        if (pressureSlider) pressureSlider.value = pressure;

        if (pressureFillColor && !isChoked)
        {
            if (pressure >= goodZoneMin && pressure <= goodZoneMax)
                pressureFillColor.color = Color.green; 
            else if (pressure > goodZoneMax)
                pressureFillColor.color = new Color(1f, 0.5f, 0f); 
            else
                pressureFillColor.color = Color.yellow; 
        }
    }
}