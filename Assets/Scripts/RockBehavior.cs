using System.Collections;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{

    private bool isTimerRunning = false;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("RockHolder"))
            return;

        if (!isTimerRunning)
            StartCoroutine(CheckMiss());
        
    }

    IEnumerator CheckMiss()
    {
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
