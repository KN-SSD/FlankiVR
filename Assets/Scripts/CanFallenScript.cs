using System;
using UnityEngine;

public class CanFallenScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            GameManager.Instance.isCanDown = true;
            Debug.Log("Puszka upadła!");
            if(GameManager.Instance.isCanDown && GameManager.Instance.isPlayersTurn)
            {
                GameManager.Instance.SwitchDrinkVisibility(true);
            }
        }
    }  
}