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

    private bool isTimerRunning = false;
    private bool hasPlayedThrowSound = false; 

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (grabInteractable.isSelected)
        {
            hasPlayedThrowSound = false;
            return; 
        }

        if (!hasPlayedThrowSound && rb.linearVelocity.magnitude >= minThrowVelocity)
        {
            PlayThrowSound();
            hasPlayedThrowSound = true; 
        }
    }

    private void PlayThrowSound()
    {
        if (throwAudioSource != null && !throwAudioSource.isPlaying)
        {
            throwAudioSource.clip = throwSounds[Random.Range(0,throwSounds.Length)];
            throwAudioSource.Play();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
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

        while (elapsedTime < decelerateDuration)
        {
            elapsedTime += Time.deltaTime;

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 2f);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (!GameManager.Instance.isCanDown)
            GameManager.Instance.SwitchTurnAfterThrow();

        isTimerRunning = false;
        Destroy(gameObject);
    }
}