using UnityEngine;

public class ObjetoRecogible : MonoBehaviour, IInteractuable
{
    [Header("Referencia a la tarea")]
    public RecogerObjetosPerdidos tarea;

    [Header("Feedback visual (opcional)")]
    public GameObject efectoRecogida;
    public AudioClip sonidoRecogida;

    private void Start()
    {
        if (tarea == null)
            tarea = FindFirstObjectByType<RecogerObjetosPerdidos>();

        if (tarea == null)
            Debug.LogWarning("No se encontró una tarea de RecogerObjetosPerdidos en la escena.");
    }

    public void Interactuar()
    {
        // 🔥 Verificar que la tarea esté iniciada y no completada
        if (tarea == null) return;
        if (!tarea.EstaIniciada)
        {
            Debug.Log("Aún no puedes recoger objetos. Completa las tareas previas.");
            return;
        }
        if (tarea.EstaCompletada)
        {
            Debug.Log("Esta tarea ya está completada.");
            return;
        }

        // Notificar a la tarea
        tarea.RecogerObjeto();

        // Feedback
        if (efectoRecogida != null)
            Instantiate(efectoRecogida, transform.position, Quaternion.identity);
        if (sonidoRecogida != null)
            AudioSource.PlayClipAtPoint(sonidoRecogida, transform.position);

        // Desactivar objeto
        gameObject.SetActive(false);
        Debug.Log($"Objeto '{gameObject.name}' recogido.");
    }

    // --- Preparación para VR ---
    // public void OnSelectEntered(SelectEnterEventArgs args)
    // {
    //     Interactuar();
    // }
}