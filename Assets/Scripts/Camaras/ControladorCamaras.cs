using UnityEngine;
using UnityEngine.InputSystem; // ← Necesario para el nuevo sistema

public class ControladorCamaras : MonoBehaviour
{
    // Distancia en la que el jugador puede interactuar con un objeto
    public float distanciaInteraccion;

    // Texto que se muestra para hacerle saber al jugador que puede interactuar
    public GameObject textoInteraccion;

    // Capas con las que el raycast puede impactar
    public LayerMask capasInteraccion;

    void Update()
    {
        // Variable RaycastHit que recopilará información de los objetos con los que impacte el raycast
        RaycastHit golpe;

        // Si el raycast golpea algo
        if (Physics.Raycast(transform.position, transform.forward, out golpe, distanciaInteraccion, capasInteraccion))
        {
            // Si el objeto que golpea se llama "botonSiguiente"
            if (golpe.collider.gameObject.name == "botonSiguiente")
            {
                // Activar el texto de interacción
                textoInteraccion.SetActive(true);

                // Si se presiona la tecla E (usando el nuevo Input System)
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    golpe.collider.gameObject.GetComponent<SistemaCamaras>().siguienteCam();
                }
            }
            // Si el objeto que golpea se llama "botonAnterior"
            else if (golpe.collider.gameObject.name == "botonAnterior")
            {
                // Activar el texto de interacción
                textoInteraccion.SetActive(true);

                // Si se presiona la tecla E (usando el nuevo Input System)
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
                {
                    golpe.collider.gameObject.GetComponent<SistemaCamaras>().anteriorCam();
                }
            }
            // En cualquier otro caso, ocultar el texto
            else
            {
                textoInteraccion.SetActive(false);
            }
        }
        // Si el raycast no golpea nada, ocultar el texto
        else
        {
            textoInteraccion.SetActive(false);
        }

        Debug.DrawRay(transform.position, transform.forward * distanciaInteraccion, Color.red);
    }
}