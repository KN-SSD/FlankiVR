using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class EnemyScript : MonoBehaviour
{
    [Header("Referencje")]
    public Transform botHand;             
    public GameObject centerCan;          
    public XRSocketInteractor socket;     

    [Header("Ustawienia Ruchu")]
    public float moveSpeed = 4.0f;        
    public float rotationSpeed = 10.0f;   
    public float waitTime = 0.5f;         
    public Vector3 homePosition = new Vector3(0, 0, 8f); 

    [Header("Poprawki Błędów")]
    public bool isModelBackwards = true;  

    [Header("Ustawienia Rzutu (Atak Bota)")]
    public float throwDelay = 2.0f;       
    public GameObject rockPrefab;         
    public Transform throwPoint;          
    public float throwForce = 8f;         
    public float throwUpwardForce = 2f;   

    private bool isRunningSequence = false;
    private bool isPreparingThrow = false; 
    private Vector3 originalCanScale;    

    void Start()
    {
        if (centerCan != null)
        {
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
            !isPreparingThrow)
        {
            StartCoroutine(PrepareAndThrowSequence());
        }
    }

   
    IEnumerator PrepareAndThrowSequence()
    {
        isPreparingThrow = true;
        Debug.Log($"[BOT] Szykuje się do rzutu... (Czeka {throwDelay}s)");

        yield return new WaitForSeconds(throwDelay);

        if (GameManager.Instance != null && 
            !GameManager.Instance.isPlayersTurn && 
            GameManager.Instance.isPlayerAtHisSpot && 
            !GameManager.Instance.isCanDown)
        {
            ThrowRock();
        }

        yield return new WaitForSeconds(1.5f);
        isPreparingThrow = false; 
    }

    void ThrowRock()
    {
        if (rockPrefab == null || centerCan == null) return;

        Debug.Log("[BOT] RZUT!");

        GameObject rock = Instantiate(rockPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 targetPos = centerCan.transform.position;
            targetPos.y += 0.15f; 

            Vector3 direction = (targetPos - throwPoint.position).normalized;

            Vector3 force = (direction * throwForce) + (Vector3.up * throwUpwardForce);
            
            rb.AddForce(force, ForceMode.Impulse);

            rb.angularVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        }
    }

    
    IEnumerator FetchAndResetCanSequence()
    {
        isRunningSequence = true;
        Debug.Log("[BOT] Biegne po puszke!");

        yield return StartCoroutine(MoveToTarget(centerCan.transform.position));
        yield return new WaitForSeconds(waitTime); 

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
        Debug.Log("[BOT] Puszka postawiona!");

        yield return StartCoroutine(MoveToTarget(homePosition));

        yield return StartCoroutine(RotateTowards(Vector3.zero));
        
        isRunningSequence = false;
        GameManager.Instance.PlayerRunningTurn();
    }


    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        Vector3 targetXZ = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);

        while (Vector3.Distance(transform.position, targetXZ) > 0.05f)
        {
            Vector3 direction = (targetXZ - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Vector3 lookDirection = isModelBackwards ? -direction : direction;
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetXZ, moveSpeed * Time.deltaTime);

            yield return null; 
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