using System.Collections.Generic;
using UnityEngine;

public class TareaBase : MonoBehaviour, ITarea
{
    [Header("Configuración de tarea")]
    [SerializeField] private string _nombreTarea = "Tarea sin nombre";

    [Header("Prerrequisitos (tareas que deben completarse antes)")]
    public List<TareaBase> prerrequisitos;

    protected bool completada = false;
    private bool iniciada = false;

    // Evento que se dispara cuando el progreso cambia
    public event System.Action<TareaBase> ProgresoActualizado;

    // Propiedades públicas
    public string nombreTarea => _nombreTarea;
    public bool EstaCompletada => completada;
    public bool EstaIniciada => iniciada;

    // Método virtual para que las hijas devuelvan su progreso (ej. "3/5")
    public virtual string ObtenerProgreso() => "";

    // Método protegido para notificar cambios de progreso
    protected void NotificarProgreso()
    {
        ProgresoActualizado?.Invoke(this);
    }

    public virtual bool PuedeIniciarse()
    {
        if (completada) return false;
        if (prerrequisitos == null || prerrequisitos.Count == 0) return true;
        foreach (var req in prerrequisitos)
        {
            if (req == null || !req.EstaCompletada)
                return false;
        }
        return true;
    }

    public virtual void IniciarTarea()
    {
        if (iniciada || completada) return;
        if (!PuedeIniciarse())
        {
            Debug.Log($"Tarea '{_nombreTarea}' no puede iniciarse (faltan prerrequisitos)");
            return;
        }
        iniciada = true;
        Debug.Log($"Tarea '{_nombreTarea}' iniciada.");
        // Lógica adicional en clases hijas
    }

    public virtual void CompletarTarea()
    {
        if (completada) return;
        completada = true;
        Debug.Log($"Tarea '{_nombreTarea}' completada.");

        GestorTareas gestor = FindFirstObjectByType<GestorTareas>();
        if (gestor != null)
            gestor.RegistrarTareaCompletada(this);
        else
            Debug.LogWarning("No se encontró GestorTareas");
    }
}