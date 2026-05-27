using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class RockBehavior : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private AudioSource throwAudioSource;
    [SerializeField] private AudioClip[] throwSounds;

    [SerializeField] private float minThrowVelocity = 2.0f;
    [SerializeField] private float maxThrowVelocity = 25.0f;

    [SerializeField] private float throwForceMultiplier = 1.5f;

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

        if (GameManager.Instance != null && !GameManager.Instance.isPlayersTurn)
        {
            isLevitating = false;
            rb.useGravity = true;

            grabInteractable.throwVelocityScale = 1.0f;
            grabInteractable.throwAngularVelocityScale = 1.0f;
        }
        else
        {
            isLevitating = true;
            rb.useGravity = false;

            grabInteractable.throwVelocityScale = throwForceMultiplier;
            grabInteractable.throwAngularVelocityScale = throwForceMultiplier;
        }

        rb.isKinematic = false;
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            isLevitating = false;
            hasPlayedThrowSound = false;
            return;
        }

        if (isLevitating && headCamera != null)
        {
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

        if (!isLevitating)
        {
            if (!rb.useGravity) rb.useGravity = true;

            if (rb.linearVelocity.magnitude > maxThrowVelocity)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxThrowVelocity;
            }

            if (!hasPlayedThrowSound && rb.linearVelocity.magnitude >= minThrowVelocity)
            {
                PlayThrowSound();
                hasPlayedThrowSound = true;
                GameManager.Instance.hasPlayerThrown = true;
                Debug.Log("Player has thrown the rock." + GameManager.Instance.hasPlayerThrown);
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
        if (isLevitating)
            return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("RockHolder"))
            return;

        if (collision.relativeVelocity.magnitude < 0.5f)
            return;

        if (!isTimerRunning)
            StartCoroutine(CheckMiss());
    }

    IEnumerator CheckMiss()
    {
        if (trail != null)
            trail.enabled = false;

        isTimerRunning = true;

        float elapsedTime = 0f;
        float decelerateDuration = 2f;

        while (elapsedTime < decelerateDuration)
        {
            elapsedTime += Time.deltaTime;

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 2f);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (GameManager.Instance != null && !GameManager.Instance.isCanDown)
            GameManager.Instance.SwitchTurnAfterThrow();

        isTimerRunning = false;
        Destroy(gameObject);
    }
}