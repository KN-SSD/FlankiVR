using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isCanDown = false;
    public bool isPlayersTurn = true;
    public bool isPlayerAtHisSpot = true;
    public bool hasPlayerThrown = false;
    public bool gameStarted = false;
    [SerializeField] GameObject playersDrink;
    [SerializeField] GameObject player;
    [SerializeField] GameObject rock;
    [SerializeField] EnemyScript enemy;
    private GameObject currentRockInstance;
    private Rigidbody rockRb;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        player.transform.position = new Vector3(0, 0, 0);
        SwitchDrinkVisibility(false);
        //PlayerDrinkingTurn();
        gameStarted = true;
    }
    public void PlayerDrinkingTurn()
    {
        Debug.Log("Tura Gracza: Pijesz!");
        isPlayersTurn = true;
        SwitchDrinkVisibility(false);

        if (currentRockInstance == null)
        {
            currentRockInstance = Instantiate(rock, new Vector3(0.5f, 1.07f, 0.4f), Quaternion.identity);
        }
    }

    public void PlayerRunningTurn()
    {
        Debug.Log("Tura Gracza: Biegniesz!");
        isPlayersTurn = false;
        SwitchDrinkVisibility(false);

        if (enemy != null)
        {
            enemy.ResetThrowFlag();
        }
    }



    public void SwitchDrinkVisibility(bool state)
    {
        playersDrink.SetActive(state);
    }

    public void SwitchTurnAfterThrow()
    {
        if (isPlayersTurn)
            PlayerRunningTurn();
        else
            PlayerDrinkingTurn();
    }

}