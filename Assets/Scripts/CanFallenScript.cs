using UnityEngine;

public class CanFallenScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            GameManager.Instance.SwitchCanState(true);
            GameManager.Instance.ShowPlayersDrink();
            Debug.Log("Puszka upadła!");
        }
    }
}
