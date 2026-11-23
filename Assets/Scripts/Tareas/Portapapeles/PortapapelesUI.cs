using UnityEngine;
using TMPro;

public class PortapapelesUI : MonoBehaviour
{
    [Header("Texto 3D del portapapeles")]
    public TextMeshPro textoTareas3D;

    [Header("Sistema de tareas")]
    public GestorTareas gestorTareas;

    // Usamos OnEnable para suscribirnos cada vez que el portapapeles se muestre
    void OnEnable()
    {
        if (gestorTareas == null)
            gestorTareas = FindAnyObjectByType<GestorTareas>();

        if (gestorTareas != null)
        {
            // Nos desuscribimos primero por si acaso, para evitar duplicados
            gestorTareas.TareaCompletada -= OnTareaCompletada;
            gestorTareas.TareaCompletada += OnTareaCompletada;

            foreach (var t in gestorTareas.tareas)
            {
                if (t is TareaBase tb)
                {
                    tb.ProgresoActualizado -= OnProgresoActualizado;
                    tb.ProgresoActualizado += OnProgresoActualizado;
                }
            }
        }
        else
        {
            Debug.LogError("[PortapapelesUI] No se encontró GestorTareas en la escena.");
        }

        ActualizarListaTareas();
    }

    void OnDisable()
    {
        // Limpiamos los eventos si el objeto se apaga
        if (gestorTareas != null)
        {
            gestorTareas.TareaCompletada -= OnTareaCompletada;
            foreach (var t in gestorTareas.tareas)
                if (t is TareaBase tb)
                    tb.ProgresoActualizado -= OnProgresoActualizado;
        }
    }

    void OnTareaCompletada(ITarea tarea)
    {
        Debug.Log($"[PortapapelesUI] ¡Evento recibido! Tarea completada: {tarea.nombreTarea}");
        ActualizarListaTareas();
    }

    void OnProgresoActualizado(TareaBase tarea)
    {
        Debug.Log($"[PortapapelesUI] ¡Evento recibido! Nuevo progreso: {tarea.ObtenerProgreso()}");
        ActualizarListaTareas();
    }

    public void MostrarPortapapeles(bool mostrar)
    {
        if (mostrar) ActualizarListaTareas();
    }

    public void ActualizarListaTareas()
    {
        if (textoTareas3D == null)
        {
            Debug.LogError("[PortapapelesUI] ¡ERROR! El campo 'TextoTareas3D' está vacío en el Inspector.");
            return;
        }
        if (gestorTareas == null) return;

        textoTareas3D.text = ObtenerTextoListaTareas();
        textoTareas3D.ForceMeshUpdate(); // Obliga a renderizar el texto al instante en VR
    }

    private string ObtenerTextoListaTareas()
    {
        string texto = "TAREAS\n-------------------------\n";
        foreach (var t in gestorTareas.tareas)
        {
            if (t == null) continue;

            string progreso = (t as TareaBase)?.ObtenerProgreso() ?? "";
            string estado = t.EstaCompletada ? "[X]" : "[ ]";

            if (!t.EstaCompletada && !string.IsNullOrEmpty(progreso))
                estado += " (" + progreso + ")";

            texto += estado + " " + t.nombreTarea + "\n";
        }
        return texto;
    }
}