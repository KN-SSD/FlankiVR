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
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("RockHolder"))
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
        yield return new WaitForSeconds(3f); 

        if(!GameManager.Instance.isCanDown)
        {
            GameManager.Instance.SwitchTurnAfterThrow();
        }

        isTimerRunning = false;
        Destroy(gameObject);
    }
}