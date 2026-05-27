using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class ArmSwingRunner : MonoBehaviour
{
    [Header("Fizyka Biegu")]
    [SerializeField] private float maxSpeed = 8.0f;
    [SerializeField] private float acceleration = 10.0f;
    [SerializeField] private float deceleration = 15.0f;

    [Header("Wymagania Ruchu")]
    [SerializeField] private float minHandMove = 0.02f; 

    [Header("Ręce, Input i Audio")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    [SerializeField] private InputActionProperty triggerPress;
    [SerializeField] private AudioSource runningAudioSource; 

    private CharacterController characterController;
    private Transform headCamera;

    private Vector3 prevLeftPos;
    private Vector3 prevRightPos;
    private float currentSpeed = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (Camera.main != null) headCamera = Camera.main.transform;

        if (leftHand) prevLeftPos = leftHand.position;
        if (rightHand) prevRightPos = rightHand.position;
    }

    void OnEnable()
    {
        if (triggerPress.action != null) triggerPress.action.Enable();
    }

    void OnDisable()
    {
        if (triggerPress.action != null) triggerPress.action.Disable();
    }

    void Update()
    {
        if (leftHand == null || rightHand == null) return;

        float leftDeltaY = leftHand.position.y - prevLeftPos.y;
        float rightDeltaY = rightHand.position.y - prevRightPos.y;

        bool isTriggerPressed = triggerPress.action.ReadValue<float>() > 0.1f;
        
        bool isAlternating = (leftDeltaY * rightDeltaY) < 0;

        bool leftHandActive = Mathf.Abs(leftDeltaY) > minHandMove;
        bool rightHandActive = Mathf.Abs(rightDeltaY) > minHandMove;
        bool canRun = CanPlayerRun();

        if (isTriggerPressed && isAlternating && leftHandActive && rightHandActive && canRun)
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        
        // Natychmiast resetuj prędkość jeśli gracz nie może biegać
        if (!canRun)
        {
            currentSpeed = 0f;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        if (currentSpeed > 0.1f)
        {
            Vector3 forwardDir = headCamera.forward;
            forwardDir.y = 0;
            forwardDir.Normalize();
            
            Vector3 moveDirection = forwardDir * currentSpeed * Time.deltaTime;
            Vector3 newPosition = transform.position + moveDirection;
            
            // Granica z = 0.5 - gracz ma granicę tylko na swojej turze
            // Na turze przeciwnika może się poruszać wszędzie
            if (newPosition.z > 0.5f && GameManager.Instance.isPlayersTurn && transform.position.z <= 0.5f)
            {
                moveDirection.z = 0.5f - transform.position.z;
            }
            
            characterController.Move(moveDirection);

            PlayRunningSound();
        }
        else
        {
            StopRunningSound();
        }

        prevLeftPos = leftHand.position;
        prevRightPos = rightHand.position;
    }

    private bool CanPlayerRun()
    {
        // Gracz nie może się ruszać jeśli już rzucił
        if (GameManager.Instance.hasPlayerThrown)
        {
            return false;
        }
        
        // Jeśli gracz jest poza swoim polem (z > 0.5), powinien móc wrócić
        if (transform.position.z > 0.5f)
        {
            return true;
        }
        
        // Warunek 1: Tura przeciwnika, puszka leży
        if (GameManager.Instance.isCanDown && !GameManager.Instance.isPlayersTurn)
        {
            return true;
        }
        // Warunek 2: Tura gracza, a gracz jeszcze nie rzucił
        else if (GameManager.Instance.isPlayersTurn && !GameManager.Instance.hasPlayerThrown)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PlayRunningSound()
    {
        if (runningAudioSource != null && !runningAudioSource.isPlaying)
        {
            runningAudioSource.Play();
        }
    }

    private void StopRunningSound()
    {
        if (runningAudioSource != null && runningAudioSource.isPlaying)
        {
            runningAudioSource.Pause(); 
        }
    }
}