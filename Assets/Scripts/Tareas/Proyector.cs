using UnityEngine;
using UnityEngine.Video;

/// Componente para proyectores. Al interactuar, notifica a la tarea asignada.
public class Proyector : MonoBehaviour, IInteractuable
{
    [Header("Referencia a la tarea")]
    public ApagarProyectores tarea;

    [Header("Feedback visual (opcional)")]
    public GameObject efectoApagado;
    public AudioClip sonidoApagado;

    [Header("Apariencia")]
    public Material materialApagado;
    public GameObject luzProyector;
    private Renderer rend;

    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Audio")]
    public AudioSource audioProyector;

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

        // Cambiar apariencia
        if (rend != null && materialApagado != null)
        {
            rend.material = materialApagado;
        }

        // Apagar la luz del proyector
        if (luzProyector != null)
        {
            luzProyector.SetActive(false);
        }

        // Detener el video
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }

        // Detener audio
        if (audioProyector != null)
        {
            audioProyector.Stop();
        }

        // Desactivar el collider para que no se pueda volver a apagar
        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;

        Debug.Log($"Proyector '{gameObject.name}' apagado.");
    }
}