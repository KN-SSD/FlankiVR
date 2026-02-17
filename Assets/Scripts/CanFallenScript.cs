using UnityEngine;

public class CanFallenScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            GameManager.Instance.SwitchCanState(true);
            Debug.Log("Puszka upadła!");
        }
    }
}
