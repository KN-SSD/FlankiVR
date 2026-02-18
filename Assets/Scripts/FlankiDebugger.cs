using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; // <--- KONIECZNE DLA NOWEGO SYSTEMU
using UnityEngine.XR.Interaction.Toolkit.Interactors; // Namespace dla XRSocketInteractor (w nowszym XR Toolkit)

public class FlankiDebugger : MonoBehaviour
{
    [Header("Referencje")]
    public GameObject centerCan;          // Puszka na środku
    public XRSocketInteractor socket;     // Socket na środku (baza)

    [Header("Ustawienia Wybicia")]
    public float debugKnockForce = 5f;    // Z jaką siłą wywalić puszkę przyciskiem

    void Update()
    {
        // Sprawdzamy, czy klawiatura jest w ogóle podłączona (bezpiecznik)
        if (Keyboard.current == null) return;

        // 1. [R]eset - Postaw puszkę na miejscu (Baza)
        // ZAMIAST: Input.GetKeyDown(KeyCode.R)
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            DebugCanToSocket();
        }

        // 2. [K]nock - Wywal puszkę (Symulacja trafienia)
        // ZAMIAST: Input.GetKeyDown(KeyCode.K)
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            DebugKnockCan();
        }
    }

    // --- FUNKCJA 1: POSTAWIENIE PUSZKI ---
    void DebugCanToSocket()
    {
        if (centerCan == null || socket == null) return;

        // Upewniamy się, że socket działa
        socket.enabled = true;
        socket.socketActive = true;

        // Resetujemy fizykę puszki
        Rigidbody rb = centerCan.GetComponent<Rigidbody>();
        if (rb)
        {
            // UWAGA: W Unity 6 używa się linearVelocity, w starszych velocity.
            // Jeśli podkreśla Ci linearVelocity na czerwono, zmień na: rb.velocity
            rb.linearVelocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero;
            
            // Możemy ją na chwilę usztywnić, żeby socket łatwiej złapał
            rb.isKinematic = false; 
        }

        // Teleportujemy puszkę IDEALNIE do punktu zaczepienia socketu
        Transform target = socket.attachTransform != null ? socket.attachTransform : socket.transform;
        centerCan.transform.position = target.position;
        centerCan.transform.rotation = target.rotation;

        Debug.Log("[DEBUG] Puszka postawiona (R).");
        GameManager.Instance.PlayerRunningTurn(); 
        GameManager.Instance.SwitchDrinkVisibility(false);
    }

    // --- FUNKCJA 2: WYWALENIE PUSZKI ---
    void DebugKnockCan()
    {
        if (centerCan == null || socket == null) return;

        // Używamy tej samej logiki co w skrypcie CanKnockout - wyłączamy socket
        StartCoroutine(ForceKnockout());
    }

    IEnumerator ForceKnockout()
    {
        // Wyłączamy socket, żeby puścił
        socket.socketActive = false;
        socket.enabled = false;

        // Dajemy kopa puszce
        Rigidbody rb = centerCan.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
            // Strzał losowo w górę i w bok
            Vector3 randomDir = (Vector3.up + Random.insideUnitSphere).normalized;
            rb.AddForce(randomDir * debugKnockForce, ForceMode.Impulse);
        }

        Debug.Log("[DEBUG] Puszka wybita (K)!");

        // Czekamy chwilę i włączamy socket z powrotem
        yield return new WaitForSeconds(2.0f);
        socket.enabled = true;
        socket.socketActive = true;
    }
}