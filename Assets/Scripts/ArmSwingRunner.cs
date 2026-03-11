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

        if (isTriggerPressed && isAlternating && leftHandActive && rightHandActive && CanPlayerRun())
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        if (currentSpeed > 0.1f)
        {
            Vector3 forwardDir = headCamera.forward;
            forwardDir.y = 0;
            forwardDir.Normalize();
            characterController.Move(forwardDir * currentSpeed * Time.deltaTime);

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
        if(GameManager.Instance.isPlayerAtHisSpot && GameManager.Instance.isCanDown && !GameManager.Instance.isPlayersTurn)
        {
            return true;
        }
        else if(!GameManager.Instance.isPlayerAtHisSpot && GameManager.Instance.isCanDown && !GameManager.Instance.isPlayersTurn)
        {
            return true;
        }
        else if (!GameManager.Instance.isPlayerAtHisSpot && !GameManager.Instance.isCanDown && !GameManager.Instance.isPlayersTurn)
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