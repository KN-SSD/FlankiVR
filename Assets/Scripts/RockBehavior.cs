using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class RockBehavior : MonoBehaviour
{
    private bool isTimerRunning = false;
    [SerializeField] private TrailRenderer trail;

    void Start()
    {
        trail = GetComponent<TrailRenderer>();
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
        trail.enabled = false;
        isTimerRunning = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        float elapsedTime = 0f;
        float decelerateDuration = 2f;

        while (elapsedTime < decelerateDuration)
        {
            elapsedTime += Time.deltaTime;

            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.deltaTime * 2f);

            yield return null;
        }

        // rb.linearVelocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(1f);


        if (!GameManager.Instance.isCanDown)
            GameManager.Instance.SwitchTurnAfterThrow();

        isTimerRunning = false;
        Destroy(gameObject);
    }
}