using UnityEngine;

/// <summary>
/// Componente para manchas de suciedad. Al interactuar, notifica a la tarea asignada.
/// </summary>
public class Mancha : MonoBehaviour, IInteractuable
{
    [Header("Referencia a la tarea")]
    public LimpiarSala tarea;              // Arrastrar desde el Inspector

    [Header("Feedback visual (opcional)")]
    public GameObject efectoLimpieza;      // Partículas o efecto al limpiar
    public AudioClip sonidoLimpieza;       // Sonido al limpiar

    [Header("Apariencia")]
    public Material materialLimpio;        // Material que se pone al limpiar (opcional)
    private Renderer rend;

    private void Start()
    {
        // Guardar referencia al Renderer si existe
        rend = GetComponent<Renderer>();

        // Buscar la tarea automáticamente si no se asignó
        if (tarea == null)
            tarea = FindFirstObjectByType<LimpiarSala>();

        if (tarea == null)
            Debug.LogWarning("No se encontró una tarea de LimpiarSala en la escena.");
    }

    /// <summary>
    /// Método llamado por el ControladorCamaras al presionar E mientras se mira esta mancha.
    /// </summary>
    public void Interactuar()
    {
        // Verificar que la tarea exista y esté iniciada
        if (tarea == null) return;
        if (!tarea.EstaIniciada)
        {
            Debug.Log("Aún no puedes limpiar. Completa las tareas previas.");
            return;
        }
        if (tarea.EstaCompletada)
        {
            Debug.Log("Esta tarea ya está completada.");
            return;
        }

        // Notificar a la tarea que esta mancha ha sido limpiada
        tarea.LimpiarMancha();

        // Feedback visual/sonoro (opcional)
        if (efectoLimpieza != null)
            Instantiate(efectoLimpieza, transform.position, Quaternion.identity);

        if (sonidoLimpieza != null)
            AudioSource.PlayClipAtPoint(sonidoLimpieza, transform.position);

        // Cambiar apariencia (opcional)
        if (rend != null && materialLimpio != null)
        {
            rend.material = materialLimpio;
        }

        // Desactivar o destruir la mancha para que no se pueda volver a limpiar
        // O puedes simplemente desactivar el collider y cambiar el material
        gameObject.SetActive(false);

        Debug.Log($"Mancha '{gameObject.name}' limpiada.");
    }

    // --- Preparación para VR (comentado) ---
    // Si usas XR Interaction Toolkit, puedes añadir:
    // public void OnSelectEntered(SelectEnterEventArgs args)
    // {
    //     Interactuar();
    // }
}