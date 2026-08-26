using UnityEngine;
using UnityEngine.InputSystem;

public class NotaInteractiva : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject notaVisual;
    [SerializeField] private GameObject canvasNota;

    [Header("Interacción")]
    [SerializeField] private float distanciaInteraccion = 3f;

    private InputAction interactAction;
    private bool notaAbierta = false;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void OnEnable()
    {
        interactAction?.Enable();
    }

    private void OnDisable()
    {
        interactAction?.Disable();
    }

    private void Update()
    {
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            if (notaAbierta)
            {
                CerrarNota();
            }
            else
            {
                IntentarAbrirNota();
            }
        }
    }

    private void IntentarAbrirNota()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("NotaInteractiva: Falta asignar la cámara del jugador.");
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
        {
            if (hit.collider.GetComponentInParent<NotaInteractiva>() == this)
            {
                AbrirNota();
            }
        }
    }

    private void AbrirNota()
    {
        notaAbierta = true;

        if (notaVisual != null)
            notaVisual.SetActive(false);

        if (canvasNota != null)
            canvasNota.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Nota abierta.");
    }

    private void CerrarNota()
    {
        notaAbierta = false;

        if (notaVisual != null)
            notaVisual.SetActive(true);

        if (canvasNota != null)
            canvasNota.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Nota cerrada.");
    }
}