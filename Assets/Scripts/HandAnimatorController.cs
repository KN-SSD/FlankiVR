using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class HandAnimatorController : MonoBehaviour
{
    [SerializeField] Vector2 m_TriggerRange = new Vector2(0f, 0.8f);
    [SerializeField] XRInputValueReader<float> m_TriggerInput = new XRInputValueReader<float>("Trigger");
    
    [SerializeField] Vector2 m_GripRightRange = new Vector2(0f, 0.8f);
    [SerializeField] XRInputValueReader<float> m_GripInput = new XRInputValueReader<float>("Grip");
    [SerializeField] float animationSpeed = 15f; 
    private Animator anim;
    private float currentTriggerValue;
    private float currentGripValue;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float rawTrigger = m_TriggerInput.ReadValue();
        float rawGrip = m_GripInput.ReadValue();

        float targetTrigger = Mathf.Lerp(m_TriggerRange.x, m_TriggerRange.y, rawTrigger);
        float targetGrip = Mathf.Lerp(m_GripRightRange.x, m_GripRightRange.y, rawGrip);

        currentTriggerValue = Mathf.Lerp(currentTriggerValue, targetTrigger, Time.deltaTime * animationSpeed);
        currentGripValue = Mathf.Lerp(currentGripValue, targetGrip, Time.deltaTime * animationSpeed);

        anim.SetFloat("Trigger", currentTriggerValue);
        anim.SetFloat("Grip", currentGripValue);
    }
}