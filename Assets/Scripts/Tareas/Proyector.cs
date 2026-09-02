using UnityEngine;

/// Componente para proyectores. Al interactuar, notifica a la tarea asignada.
public class Proyector : MonoBehaviour, IInteractuable
{
    [Header("Referencia a la tarea")]
    public ApagarProyectores tarea;         // Arrastrar desde el Inspector

    [Header("Feedback visual (opcional)")]
    public GameObject efectoApagado;        // Partículas o efecto al apagar
    public AudioClip sonidoApagado;         // Sonido al apagar

    [Header("Apariencia")]
    public Material materialApagado;        // Material que se pone al apagar (opcional)
    public GameObject luzProyector;         // La luz o emisor del proyector (opcional)
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();

        // Buscar la tarea automáticamente si no se asignó
        if (tarea == null)
            tarea = FindFirstObjectByType<ApagarProyectores>();

        if (tarea == null)
            Debug.LogWarning("No se encontró una tarea de ApagarProyectores en la escena.");
    }

    /// Método llamado por el ControladorCamaras al presionar E mientras se mira este proyector.
    public void Interactuar()
    {
        // Verificar que la tarea exista y esté iniciada
        if (tarea == null) return;
        if (!tarea.EstaIniciada)
        {
            Debug.Log("Aún no puedes apagar proyectores. Completa las tareas previas.");
            return;
        }
        if (tarea.EstaCompletada)
        {
            Debug.Log("Esta tarea ya está completada.");
            return;
        }

        // Notificar a la tarea que este proyector ha sido apagado
        tarea.ApagarProyector();

        // Feedback visual/sonoro (opcional)
        if (efectoApagado != null)
            Instantiate(efectoApagado, transform.position, Quaternion.identity);

        if (sonidoApagado != null)
            AudioSource.PlayClipAtPoint(sonidoApagado, transform.position);

        // Cambiar apariencia (opcional)
        if (rend != null && materialApagado != null)
        {
            rend.material = materialApagado;
        }

        // Apagar la luz del proyector (si existe)
        if (luzProyector != null)
        {
            luzProyector.SetActive(false);
        }

        // Desactivar el proyector o su collider para que no se pueda volver a apagar
        // O simplemente desactivar el collider
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log($"Proyector '{gameObject.name}' apagado.");
    }

    // --- Preparación para VR (comentado) ---
    // public void OnSelectEntered(SelectEnterEventArgs args)
    // {
    //     Interactuar();
    // }
}