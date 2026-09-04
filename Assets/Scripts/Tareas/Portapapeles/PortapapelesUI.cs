using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PortapapelesUI : MonoBehaviour
{
    [Header("Referencias UI (Canvas)")]
    public GameObject panelTareas;          // Panel 2D (opcional, puedes mantenerlo)
    public TextMeshProUGUI textoTareas2D;   // Texto 2D (opcional)

    [Header("Referencias UI 3D (World Space)")]
    public TextMeshPro textoTareas3D;       // ← NUEVO: TextMeshPro en 3D

    [Header("Referencias al sistema de tareas")]
    public GestorTareas gestorTareas;

    [Header("Referencia al objeto portapapeles")]
    public PortapapelesObjeto portapapelesObjeto;

    private bool portapapelesRecogido = false;

    void Start()
    {
        if (gestorTareas == null)
            gestorTareas = FindFirstObjectByType<GestorTareas>();

        if (gestorTareas != null)
            gestorTareas.TareaCompletada += OnTareaCompletada;
        else
            Debug.LogWarning("No se encontró GestorTareas.");

        // Desactivar el panel 2D al inicio (si existe)
        if (panelTareas != null)
            panelTareas.SetActive(false);
        else
            Debug.LogWarning("PanelTareas no asignado.");

        // Actualizar el texto 3D al inicio (mostrar estado inicial)
        ActualizarListaTareas();
    }

    void Update()
    {
        // Detectar tecla E para soltar (usando Input System)
        if (portapapelesRecogido && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SoltarPortapapeles();
        }
    }

    void OnDestroy()
    {
        if (gestorTareas != null)
            gestorTareas.TareaCompletada -= OnTareaCompletada;
    }

    void OnTareaCompletada(ITarea tarea)
    {
        // Actualizar siempre, tanto si el panel 2D está visible como el texto 3D
        ActualizarListaTareas();

        // Si el panel 2D está visible, también actualizar (opcional)
        if (panelTareas != null && panelTareas.activeSelf)
        {
            ActualizarListaTareas2D();
        }

        Debug.Log($"UI actualizada: tarea '{tarea.nombreTarea}' completada");
    }

    /// <summary>
    /// Actualiza el texto 3D (siempre visible en el mundo)
    /// </summary>
    public void ActualizarListaTareas()
    {
        if (textoTareas3D == null || gestorTareas == null) return;

        string texto = "TAREAS:\n";
        texto += "---------------------------\n\n";
        foreach (var tarea in gestorTareas.tareas)
        {
            if (tarea == null) continue;
            string estado = tarea.EstaCompletada ? "[X]" : "[ ]";
            texto += $"{estado} {tarea.nombreTarea}\n";
        }

        textoTareas3D.text = texto;
        Debug.Log("Lista de tareas actualizada en 3D.");
    }

    /// <summary>
    /// Actualiza el texto 2D (solo si está visible)
    /// </summary>
    public void ActualizarListaTareas2D()
    {
        if (textoTareas2D == null || gestorTareas == null) return;

        string texto = "LISTA DE TAREAS\n";
        texto += "---------------------------\n\n";
        foreach (var tarea in gestorTareas.tareas)
        {
            if (tarea == null) continue;
            string estado = tarea.EstaCompletada ? "[X]" : "[ ]";
            texto += $"{estado} {tarea.nombreTarea}\n";
        }

        textoTareas2D.text = texto;
    }

    public void MostrarPortapapeles(bool mostrar)
    {
        // Solo controla el panel 2D (el texto 3D siempre está visible)
        if (panelTareas != null)
        {
            panelTareas.SetActive(mostrar);
            portapapelesRecogido = mostrar;
        }

        if (mostrar)
        {
            ActualizarListaTareas2D();
        }
    }

    private void SoltarPortapapeles()
    {
        if (portapapelesObjeto != null)
        {
            portapapelesObjeto.Soltar();
            portapapelesRecogido = false;
        }
    }
}