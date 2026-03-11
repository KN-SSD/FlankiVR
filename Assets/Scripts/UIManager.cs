using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    [Header("Input Setup")]
    [Tooltip("Assign the Input Action for the X button here.")]
    [SerializeField] private InputActionProperty restartButtonAction;
    [SerializeField] private GameObject wonImage;
    [SerializeField] private GameObject lostImage;
    [SerializeField] private AudioSource finishedAudioSource;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;


    [SerializeField] private GameObject menuPanel;

    [SerializeField] private GameObject creditsPanel;


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

    public void ShowCredits()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void HideCredits()
    {
        menuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    void Awake()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "Finished")
        {
            if (GameManager.hasPlayerWon)
            {
                wonImage.SetActive(true);
                finishedAudioSource.clip = win;
                finishedAudioSource.Play();
            }
            else
            {
                lostImage.SetActive(true);
                finishedAudioSource.clip = lose;
                finishedAudioSource.Play();
            }


        }
    }
}
