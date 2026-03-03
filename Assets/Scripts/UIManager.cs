using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Input Setup")]
    [Tooltip("Assign the Input Action for the X button here.")]
    [SerializeField] private InputActionProperty restartButtonAction;
    [SerializeField] private TextMeshProUGUI finishedText;

    private void OnEnable()
    {
        restartButtonAction.action.Enable();

        restartButtonAction.action.performed += RestartScene;
    }

    private void OnDisable()
    {
        restartButtonAction.action.performed -= RestartScene;
        restartButtonAction.action.Disable();
    }

    private void RestartScene(InputAction.CallbackContext context)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Flanki");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Finished")
        {
            if (GameManager.hasPlayerWon)
                finishedText.text = "Wygrałeś!";
            else    
                finishedText.text = "Przegrałeś!";

        }
    }

}
