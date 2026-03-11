using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.VFX;

[RequireComponent(typeof(XRGrabInteractable))]
public class DrinkingMinigame : MonoBehaviour
{
    [SerializeField] private float maxDrinkingRate = 25f;        

    [Header("--- Mechanika Spamowania ---")]
    public float pressure = 0f;             
    [SerializeField] private float maxPressure = 100f; 
    [SerializeField] private float pressureGain = 15f;        
    [SerializeField] private float pressureDecay = 25f;       

    [Header("--- Wykrywanie Ust ---")]
    [SerializeField] private float headDistanceLimit = 0.3f;  
    [SerializeField] private float heightOffset = 0.15f;      
    [SerializeField] private float requiredTilt = 80f;        

    [Header("--- UI, Input i Audio ---")]
    [SerializeField] private Slider liquidSlider;             
    [SerializeField] private InputActionProperty triggerInput; 
    [SerializeField] private AudioSource drinkAudioSource; 
    private XRGrabInteractable grab;
    private Transform headCamera;
    [SerializeField] private VisualEffect drinkingEffect;


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

        if (!grab.isSelected)
        {
            StopDrinkingSound();
            return;
        }

        if (triggerInput.action.WasPressedThisFrame())
        {
            pressure += pressureGain;
            pressure = Mathf.Min(pressure, maxPressure);
        }

        if (IsCanAtMouth())
        {
            ProcessDrinking();
        }
        else
        {
            StopDrinkingSound();
        }
    }

    void ProcessDrinking()
    {
       
        if (pressure > 0f && GameManager.Instance.playersDrinkLeft > 0f)
        {
            float currentDrinkSpeed = (pressure / maxPressure) * maxDrinkingRate;
            GameManager.Instance.playersDrinkLeft -= currentDrinkSpeed * Time.deltaTime;

            PlayDrinkingSound();
             drinkingEffect.SendEvent("StartDrinking");

            if (GameManager.Instance.playersDrinkLeft <= 0f)
            {
                GameManager.Instance.playersDrinkLeft = 0f;
                StopDrinkingSound(); 
            }
        }
        else
        {
            StopDrinkingSound(); 
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
        
        ControllerHaptic.StartHaptic(pressure / maxPressure, 0.1f);
    }

    void UpdateUI()
    {
        if (liquidSlider) liquidSlider.value = GameManager.Instance.playersDrinkLeft;
    }

    private void PlayDrinkingSound()
    {
        if (drinkAudioSource != null && !drinkAudioSource.isPlaying)
        {
            drinkAudioSource.Play();
        }
    }

    private void StopDrinkingSound()
    {
        if (drinkAudioSource != null && drinkAudioSource.isPlaying)
        {
            drinkingEffect.SendEvent("StopDrinking");
            drinkAudioSource.Pause(); 
        }
    }
}