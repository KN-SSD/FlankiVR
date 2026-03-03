using UnityEngine;

public class PlayerSpaceScript : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerSpaceCheck"))
        {
            GameManager.Instance.isPlayerAtHisSpot = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerSpaceCheck")) return;

        GameManager.Instance.isPlayerAtHisSpot = true;
        Debug.Log("Player in his space");
        if (other.CompareTag("PlayerSpaceCheck") && GameManager.Instance.isPlayerAtHisSpot && !GameManager.Instance.isCanDown && GameManager.Instance.gameStarted)
        {
            GameManager.Instance.PlayerDrinkingTurn();
        }
    }
}