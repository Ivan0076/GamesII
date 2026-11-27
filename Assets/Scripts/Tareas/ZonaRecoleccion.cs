using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ZonaRecoleccion : MonoBehaviour
{
    public RecogerObjetoPerdidos tarea;

    private void OnTriggerEnter(Collider other)
    {
        ProcesarObjeto(other);
    }

    private void OnTriggerStay(Collider other)
    {
        ProcesarObjeto(other);
    }

    private void ProcesarObjeto(Collider other)
    {
        if (tarea == null || tarea.EstaCompletada) return;

        ObjetoRecogible obj = other.GetComponentInParent<ObjetoRecogible>();
        if (obj == null) return;

        if (Time.time < obj.bloquearEntregaHasta) return;
        if (tarea.objetosRecuperados.Contains(obj.gameObject)) return;

        XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
        if (grab != null && grab.isSelected) return;

        if (tarea.EntregarObjeto(obj.gameObject))
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (grab != null)
            {
                grab.enabled = false;
            }
        }
    }
}