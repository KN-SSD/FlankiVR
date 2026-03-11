using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isCanDown = false;
    public bool isPlayersTurn = true;
    public bool isPlayerAtHisSpot = true;
    public bool hasPlayerThrown = false;
    public bool gameStarted = false;
    [SerializeField] private GameObject playersDrink;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject rock;
    [SerializeField] private EnemyScript enemy;
    private GameObject currentRockInstance;

    [Header("Drinking bars and ui")]
    private bool gameFinished;
    public float playersDrinkLeft;
    public float enemyDrinkLeft;
    public static bool hasPlayerWon;
    [SerializeField] private Slider playersDrinkSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameFinished = false;
            playersDrinkLeft = 100f;
            enemyDrinkLeft = 100f;
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
        hasPlayerWon = false;
        player.transform.position = new Vector3(0, 0, 0);
        SwitchDrinkVisibility(false);
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
        playersDrink.GetComponent<DrinkingMinigame>().pressure = 0f;
        playersDrink.SetActive(state);
    }

    public void SwitchTurnAfterThrow()
    {
        if (isPlayersTurn)
            PlayerRunningTurn();
        else
            PlayerDrinkingTurn();
    }

    void Update()
    {
        if (playersDrinkLeft <= 0 && !gameFinished)
        {
            hasPlayerWon = true;
            SceneManager.LoadScene("Finished");
        }
        if (enemyDrinkLeft <= 0 && !gameFinished)
        {
            hasPlayerWon = false;
            SceneManager.LoadScene("Finished");
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        playersDrinkSlider.value = playersDrinkLeft;
    }
}