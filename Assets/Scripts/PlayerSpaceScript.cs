using UnityEngine;

public class PlayerSpaceScript : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.isPlayerAtHisSpot = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.isPlayerAtHisSpot = true;
        }

        if (GameManager.Instance.isPlayerAtHisSpot && !GameManager.Instance.isCanDown && GameManager.Instance.gameStarted)
        {
            GameManager.Instance.PlayerDrinkingTurn();
        }
    }
}
