using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ZonaRecoleccion : MonoBehaviour
{
    public RecogerObjetoPerdidos tarea;

    private void OnTriggerEnter(Collider other)
    {
        if (tarea == null || tarea.EstaCompletada) return;

        ObjetoRecogible obj = other.GetComponent<ObjetoRecogible>();
        if (obj != null)
        {
            // Intentamos entregar el objeto a la tarea
            if (tarea.EntregarObjeto(obj.gameObject))
            {
                // Si se entregó con éxito, lo congelamos en la zona
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                // Desactivamos el agarre para que el jugador no lo pueda sacar de la zona
                XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
                if (grab != null) grab.enabled = false;

                // Como el ObjetoRecogible está en la lista, su método Interactuar() 
                // tampoco dejará agarrarlo con el mouse.
            }
        }
    }
}