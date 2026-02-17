using UnityEditorInternal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isCanDown = false;
    public bool isPlayersTurn = true;
    [SerializeField] GameObject playersDrink;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SwitchCanState(bool state)
    {
        isCanDown = state;
    }

    public void ShowPlayersDrink()
    {
        playersDrink.SetActive(true);
    }

}
