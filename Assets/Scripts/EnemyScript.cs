using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EnemyScript : MonoBehaviour
{
    public Transform botHand;
    public GameObject centerCan;
    public XRSocketInteractor socket;

    public float moveSpeed = 4.0f;
    public float rotationSpeed = 10.0f;
    public float waitTime = 0.5f;
    public Vector3 homePosition = new Vector3(0, 0, 8f);

    public bool isModelBackwards = true;

    public float throwDelay = 2.0f;
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float throwForce = 8f;
    public float throwUpwardForce = 2f;

    private bool isRunningSequence = false;
    private bool isPreparingThrow = false;
    private bool hasThrownThisTurn = false;
    private Vector3 originalCanScale;

    private Animator anim;

    void Start()
    {
        if (centerCan != null)
        {
            anim = gameObject.GetComponentInChildren<Animator>();
            originalCanScale = centerCan.transform.localScale;
        }

        if (throwPoint == null)
        {
            throwPoint = botHand;
        }
    }

    void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.isPlayersTurn &&
            GameManager.Instance.isCanDown &&
            !isRunningSequence)
        {
            StartCoroutine(FetchAndResetCanSequence());
        }

        if (GameManager.Instance != null &&
            !GameManager.Instance.isPlayersTurn &&
            GameManager.Instance.isPlayerAtHisSpot &&
            !GameManager.Instance.isCanDown &&
            !isRunningSequence &&
            !isPreparingThrow &&
            !hasThrownThisTurn)
        {
            StartCoroutine(PrepareAndThrowSequence());
        }

        // Logika picia i animacji picia
        bool shouldDrink = false;
        if (GameManager.Instance != null &&
            !GameManager.Instance.isPlayersTurn &&
            hasThrownThisTurn)
        {
            if (GameManager.Instance.isCanDown || !GameManager.Instance.isPlayerAtHisSpot)
            {
                shouldDrink = true;
                float currentDrinkSpeed = Random.Range(3f, 6f);
                GameManager.Instance.enemyDrinkLeft -= currentDrinkSpeed * Time.deltaTime;
            }
        }
        
        // Zaktualizowanie animacji picia co klatkę
        if (anim != null)
        {
            anim.SetBool("isDrinking", shouldDrink);
        }
    }

    public void ResetThrowFlag()
    {
        hasThrownThisTurn = false;
    }

    IEnumerator PrepareAndThrowSequence()
    {
        isPreparingThrow = true;

        yield return new WaitForSeconds(throwDelay);

        if (GameManager.Instance != null &&
            !GameManager.Instance.isPlayersTurn &&
            GameManager.Instance.isPlayerAtHisSpot &&
            !GameManager.Instance.isCanDown &&
            !hasThrownThisTurn)
        {
            ThrowRock();
        }

        yield return new WaitForSeconds(1.5f);
        isPreparingThrow = false;
    }

    void ThrowRock()
    {
        if (rockPrefab == null || centerCan == null) return;

        hasThrownThisTurn = true;
        if (anim != null) anim.SetTrigger("Throw");
        StartCoroutine(RockDelay());
    }

    IEnumerator RockDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject rock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 targetPos = centerCan.transform.position;
            targetPos.y += 0.15f;

            float currentThrowForce = throwForce;
            float currentUpwardForce = throwUpwardForce;

            bool isHit = Random.value <= 0.45f;

            if (isHit)
            {
                targetPos.x += Random.Range(-0.05f, 0.05f);
                targetPos.z += Random.Range(-0.05f, 0.05f);
            }
            else
            {
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float missDistance = Random.Range(0.6f, 1.5f);
                
                float missX = randomDir.x * missDistance;
                float missZ = randomDir.y * missDistance;
                float missY = Random.Range(-0.2f, 0.4f);

                targetPos += new Vector3(missX, missY, missZ);

                currentThrowForce *= Random.Range(0.5f, 1.5f);
                currentUpwardForce *= Random.Range(0.5f, 1.5f);
            }

            Vector3 direction = (targetPos - throwPoint.position).normalized;

            Vector3 force = (direction * currentThrowForce) + (Vector3.up * currentUpwardForce);
            rb.AddForce(force, ForceMode.Impulse);

            rb.angularVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }
    }

    IEnumerator FetchAndResetCanSequence()
    {
        isRunningSequence = true;

        yield return StartCoroutine(MoveToTarget(centerCan.transform.position));
        
        // Animacja podnoszenia puszki
       // if (anim != null) anim.SetTrigger("Interact");
        //yield return new WaitForSeconds(waitTime);

        Rigidbody canRb = centerCan.GetComponent<Rigidbody>();
        if (canRb != null)
        {
            canRb.isKinematic = true;
            canRb.linearVelocity = Vector3.zero;
            canRb.angularVelocity = Vector3.zero;
        }

        centerCan.transform.SetParent(botHand, true);
        centerCan.transform.localPosition = Vector3.zero;
        centerCan.transform.localRotation = Quaternion.identity;

        yield return StartCoroutine(MoveToTarget(socket.transform.position));
        
        // Animacja odkładania puszki
        if (anim != null) anim.SetTrigger("Interact");
        yield return new WaitForSeconds(waitTime);

        centerCan.transform.SetParent(null);
        centerCan.transform.localScale = originalCanScale;

        socket.enabled = true;
        socket.socketActive = true;

        Transform attachTarget = socket.attachTransform != null ? socket.attachTransform : socket.transform;
        centerCan.transform.position = attachTarget.position;
        centerCan.transform.rotation = attachTarget.rotation;

        if (canRb != null) canRb.isKinematic = false;

        GameManager.Instance.isCanDown = false;

        yield return StartCoroutine(MoveToTarget(homePosition));
        yield return StartCoroutine(RotateTowards(Vector3.zero));

        isRunningSequence = false;
        GameManager.Instance.PlayerRunningTurn();
    }

    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        Vector3 targetXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        
        if (Vector3.Distance(transform.position, targetXZ) > 0.1f)
        {
            float currentSpeed = 0f;
            float acceleration = Random.Range(1.5f, 4.0f);

            if (anim != null) anim.SetBool("isRunning", true);

            while (Vector3.Distance(transform.position, targetXZ) > 0.1f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);

                Vector3 direction = (targetXZ - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Vector3 lookDirection = isModelBackwards ? -direction : direction;
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, targetXZ, currentSpeed * Time.deltaTime);

                yield return null;
            }
            
            if (anim != null) anim.SetBool("isRunning", false);
        }
        
        transform.position = targetXZ;
    }

    IEnumerator RotateTowards(Vector3 targetPosition)
    {
        Vector3 targetXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 direction = (targetXZ - transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Vector3 lookDirection = isModelBackwards ? -direction : direction;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 1.0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }
            transform.rotation = targetRotation;
        }
    }
}