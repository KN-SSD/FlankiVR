using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class HandAnimatorController : MonoBehaviour
{
    [Header("Limity zamykania dłoni (Min, Max)")]
    [SerializeField] Vector2 m_TriggerRange = new Vector2(0f, 0.8f);
    [SerializeField] XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    
    [SerializeField] Vector2 m_GripRightRange = new Vector2(0f, 0.8f);
    [SerializeField] XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");

    [Header("Ustawienia płynności (Smoothing)")]
    [SerializeField] float animationSpeed = 15f; 

    private Animator anim;
    
    // Zmienne do przechowywania aktualnego stanu palców
    private float currentTriggerValue;
    private float currentGripValue;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. Odczytujemy surową wartość nacisku od 0.0 do 1.0
        float rawTrigger = m_TriggerInput.ReadValue();
        float rawGrip = m_GripInput.ReadValue();

        // 2. Skalujemy surową wartość do Twoich limitów (wyliczamy, gdzie palce MAJĄ się znaleźć)
        float targetTrigger = Mathf.Lerp(m_TriggerRange.x, m_TriggerRange.y, rawTrigger);
        float targetGrip = Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, rawGrip);

        // 3. Płynnie przesuwamy obecną pozycję palców w stronę docelowej
        currentTriggerValue = Mathf.Lerp(currentTriggerValue, targetTrigger, Time.deltaTime * animationSpeed);
        currentGripValue = Mathf.Lerp(currentGripValue, targetGrip, Time.deltaTime * animationSpeed);

        // 4. Wysyłamy finalną, gładką wartość do Animatora
        anim.SetFloat("Trigger", currentTriggerValue);
        anim.SetFloat("Grip", currentGripValue);
    }
}