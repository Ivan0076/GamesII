using UnityEngine;
using UnityEngine.InputSystem;

public class InterruptorLuces : MonoBehaviour
{
    [Header("Control de luces")]
    public ControlLuces controlLuces;

    [Header("Interacción")]
    public Camera playerCamera;
    public float distanciaInteraccion = 3f;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            IntentarActivar();
        }
    }

    private void IntentarActivar()
    {
        if (playerCamera == null)
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaInteraccion))
        {
            if (hit.collider.gameObject == gameObject)
            {
                ActivarInterruptor();
            }
        }
    }

    private void ActivarInterruptor()
    {
        if (controlLuces != null)
        {
            controlLuces.EncenderLuces();
            Debug.Log("🔘 Interruptor activado.");
        }
    }
}