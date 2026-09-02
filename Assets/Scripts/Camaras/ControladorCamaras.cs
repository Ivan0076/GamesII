using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorCamaras : MonoBehaviour
{
    public float distanciaInteraccion;
    public GameObject textoInteraccion;
    public LayerMask capasInteraccion;

    void Update()
    {
        RaycastHit golpe;
        if (Physics.Raycast(transform.position, transform.forward, out golpe, distanciaInteraccion, capasInteraccion))
        {
            GameObject obj = golpe.collider.gameObject;

            // --- Botones de cámara (funcionalidad original) ---
            if (obj.name == "botonSiguiente")
            {
                textoInteraccion.SetActive(true);
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    obj.GetComponent<SistemaCamaras>().siguienteCam();
                }
            }
            else if (obj.name == "botonAnterior")
            {
                textoInteraccion.SetActive(true);
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    obj.GetComponent<SistemaCamaras>().anteriorCam();
                }
            }
            else
            {
                // --- Nuevo: Interactuables genéricos ---
                IInteractuable interactuable = obj.GetComponent<IInteractuable>();
                if (interactuable != null)
                {
                    textoInteraccion.SetActive(true);
                    if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        interactuable.Interactuar();
                    }
                }
                else
                {
                    textoInteraccion.SetActive(false);
                }
            }
        }
        else
        {
            textoInteraccion.SetActive(false);
        }

        Debug.DrawRay(transform.position, transform.forward * distanciaInteraccion, Color.red);
    }
}