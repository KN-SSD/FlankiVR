using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables; 

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class RockBehavior : MonoBehaviour
{
    [Header("--- Efekty i Audio ---")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private AudioSource throwAudioSource; 
    [SerializeField] private AudioClip[] throwSounds;

    [Header("--- Ustawienia Rzutu i Fizyki ---")]
    [SerializeField] private float minThrowVelocity = 2.0f; 
    [Tooltip("Kaganiec na fizykę VR - zapobiega lotom kamienia w kosmos")]
    [SerializeField] private float maxThrowVelocity = 25.0f; 
    
    [Header("--- Wzmocnienie Siły Rzutu ---")]
    [Tooltip("Mnożnik siły wyrzutu z dłoni (np. 1.5 = 150% siły gracza)")]
    [SerializeField] private float throwForceMultiplier = 1.5f; 

    [Header("--- Lewitacja (Podążanie) ---")]
    [SerializeField] private Transform headCamera; 
    [SerializeField] private Vector3 hoverOffset = new Vector3(0.3f, -0.4f, 0.4f); 
    [SerializeField] private float hoverSpeed = 8f; 

    private bool isTimerRunning = false;
    private bool hasPlayedThrowSound = false; 
    private bool isLevitating; 

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        
        if (headCamera == null && Camera.main != null) 
            headCamera = Camera.main.transform;

        // Sprawdzamy czyja jest tura, żeby zablokować lewitację i dopasować siłę rzutu
        if (GameManager.Instance != null && !GameManager.Instance.isPlayersTurn)
        {
            // TURA WROGA: brak lewitacji, standardowa siła (1.0)
            isLevitating = false;
            rb.useGravity = true;
            
            grabInteractable.throwVelocityScale = 1.0f;
            grabInteractable.throwAngularVelocityScale = 1.0f;
        }
        else
        {
            // TURA GRACZA: lewitacja aktywna, podkręcona siła rzutu
            isLevitating = true;
            rb.useGravity = false;
            
            grabInteractable.throwVelocityScale = throwForceMultiplier;
            grabInteractable.throwAngularVelocityScale = throwForceMultiplier;
        }
        
        // Zostawiamy isKinematic na false, inaczej XR Toolkit popsuje rzut!
        rb.isKinematic = false; 
    }

    void Update()
    {
        // 1. Sprawdzamy chwyt - jeśli gracz złapie kamień, koniec lewitacji
        if (grabInteractable.isSelected)
        {
            isLevitating = false; 
            hasPlayedThrowSound = false;
            return; 
        }

        // 2. Obsługa lewitacji bez użycia isKinematic
        if (isLevitating && headCamera != null)
        {
            // Trzymamy fizykę w ryzach ręcznie na czas lewitacji
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;

            float yawY = headCamera.eulerAngles.y;
            Quaternion bodyRotation = Quaternion.Euler(0, yawY, 0);
            Vector3 targetPosition = headCamera.position + (bodyRotation * hoverOffset);

            transform.position = Vector3.Lerp(transform.position, targetPosition, hoverSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, bodyRotation, hoverSpeed * Time.deltaTime);
            
            return; 
        }

        // 3. Lot kamienia po wypuszczeniu z ręki
        if (!isLevitating)
        {
            // Przywracamy grawitację (potrzebne po zakończeniu lewitacji)
            if (!rb.useGravity) rb.useGravity = true;

            // Twarde ograniczenie prędkości po rzucie
            if (rb.linearVelocity.magnitude > maxThrowVelocity)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxThrowVelocity;
            }

            // Odtwarzanie dźwięku
            if (!hasPlayedThrowSound && rb.linearVelocity.magnitude >= minThrowVelocity)
            {
                PlayThrowSound();
                hasPlayedThrowSound = true; 
            }
        }
    }

    private void PlayThrowSound()
    {
        if (throwAudioSource != null && !throwAudioSource.isPlaying && throwSounds != null && throwSounds.Length > 0)
        {
            throwAudioSource.clip = throwSounds[Random.Range(0, throwSounds.Length)];
            throwAudioSource.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isLevitating) return; // Zabezpieczenie przed uderzaniem gracza w trakcie lewitacji

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("RockHolder"))
            return;

        if (collision.relativeVelocity.magnitude < 0.5f)
            return;

        if (!isTimerRunning)
            StartCoroutine(CheckMiss());
    }

    IEnumerator CheckMiss()
    {
        if (trail != null) trail.enabled = false;
        isTimerRunning = true;

        float elapsedTime = 0f;
        float decelerateDuration = 2f;

        // Płynne hamowanie kamienia po upadku
        while (elapsedTime < decelerateDuration)
        {
            elapsedTime += Time.deltaTime;

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 2f);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Przekazanie tury, jeśli puszka nie spadła
        if (GameManager.Instance != null && !GameManager.Instance.isCanDown)
            GameManager.Instance.SwitchTurnAfterThrow();

        isTimerRunning = false;
        Destroy(gameObject);
    }
}