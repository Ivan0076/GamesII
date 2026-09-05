using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PortapapelesUI : MonoBehaviour
{
    [Header("Referencias UI (Canvas)")]
    public GameObject panelTareas;          // Panel 2D (opcional)
    public TextMeshProUGUI textoTareas2D;   // Texto 2D (opcional)

    [Header("Referencias UI 3D (World Space)")]
    public TextMeshPro textoTareas3D;       // TextMeshPro en 3D

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
        {
            // Suscribirse al evento de tarea completada
            gestorTareas.TareaCompletada += OnTareaCompletada;

            // Suscribirse al evento de progreso de cada tarea
            foreach (var t in gestorTareas.tareas)
            {
                if (t is TareaBase tb)
                {
                    tb.ProgresoActualizado += OnProgresoActualizado;
                }
            }
        }
        else
        {
            Debug.LogWarning("No se encontró GestorTareas.");
        }

        // Desactivar el panel 2D al inicio
        if (panelTareas != null)
            panelTareas.SetActive(false);

        // Actualizar el texto 3D al inicio
        ActualizarListaTareas();
    }

    void Update()
    {
        if (portapapelesRecogido && Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            SoltarPortapapeles();
        }
    }

    void OnDestroy()
    {
        if (gestorTareas != null)
        {
            gestorTareas.TareaCompletada -= OnTareaCompletada;

            // Desuscribirse de los eventos de progreso
            foreach (var t in gestorTareas.tareas)
            {
                if (t is TareaBase tb)
                {
                    tb.ProgresoActualizado -= OnProgresoActualizado;
                }
            }
        }
    }

    // Se llama cuando se completa una tarea
    void OnTareaCompletada(ITarea tarea)
    {
        // Actualizar siempre el texto 3D
        ActualizarListaTareas();

        // Si el panel 2D está visible, actualizarlo también
        if (panelTareas != null && panelTareas.activeSelf)
        {
            ActualizarListaTareas2D();
        }

        Debug.Log($"UI actualizada: tarea '{tarea.nombreTarea}' completada");
    }

    // Se llama cuando el progreso de una tarea cambia (ej. recoger objeto)
    void OnProgresoActualizado(TareaBase tarea)
    {
        // Actualizar siempre el texto 3D
        ActualizarListaTareas();

        // Si el panel 2D está visible, actualizarlo también
        if (panelTareas != null && panelTareas.activeSelf)
        {
            ActualizarListaTareas2D();
        }

        Debug.Log($"Progreso actualizado: {tarea.nombreTarea} - {tarea.ObtenerProgreso()}");
    }

    /// <summary>
    /// Actualiza el texto 3D (siempre visible en el mundo)
    /// </summary>
    public void ActualizarListaTareas()
    {
        if (textoTareas3D == null || gestorTareas == null) return;

        textoTareas3D.text = ObtenerTextoListaTareas();
    }

    /// <summary>
    /// Actualiza el texto 2D (solo si está visible)
    /// </summary>
    public void ActualizarListaTareas2D()
    {
        if (textoTareas2D == null || gestorTareas == null) return;

        string texto = "LISTA DE TAREAS\n";
        texto += "---------------------------\n\n";
        texto += "Presiona Q para soltar\n\n";
        texto += ObtenerTextoListaTareas();
        textoTareas2D.text = texto;
    }

    /// <summary>
    /// Genera el texto común para 3D y 2D
    /// </summary>
    private string ObtenerTextoListaTareas()
    {
        string texto = "TAREAS:\n";
        texto += "---------------------------\n\n";

        foreach (var tarea in gestorTareas.tareas)
        {
            if (tarea == null) continue;

            // Obtener el progreso si existe
            string progreso = (tarea as TareaBase)?.ObtenerProgreso() ?? "";
            string estado;

            if (tarea.EstaCompletada)
            {
                estado = "[X]";
            }
            else
            {
                estado = "[ ]";
                // Si la tarea no está completada pero tiene progreso, lo mostramos
                if (!string.IsNullOrEmpty(progreso))
                {
                    estado = $"{estado} ({progreso})";
                }
            }

            texto += $"{estado} {tarea.nombreTarea}\n";
        }

        return texto;
    }

    public void MostrarPortapapeles(bool mostrar)
    {
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