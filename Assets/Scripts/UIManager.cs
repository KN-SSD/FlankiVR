using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
   [Header("Input Setup")]
    [Tooltip("Assign the Input Action for the X button here.")]
    [SerializeField] private InputActionProperty restartButtonAction;

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
}
