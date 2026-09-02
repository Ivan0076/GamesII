using UnityEngine;

public class LimpiarSala : TareaBase
{
    [Header("Configuración de limpieza")]
    public int manchasNecesarias = 4;      // Cantidad de manchas a limpiar
    private int limpiadas = 0;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        limpiadas = 0;
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Limpia {manchasNecesarias} manchas.");
    }

    /// <summary>
    /// Método llamado desde cada mancha cuando se interactúa con ella.
    /// </summary>
    public void LimpiarMancha()
    {
        // Evitar que se siga limpiando si ya está completada o se alcanzó el límite
        if (limpiadas >= manchasNecesarias || EstaCompletada) return;

        limpiadas++;
        Debug.Log($"Mancha limpiada: {limpiadas}/{manchasNecesarias}");

        // Si se alcanzó la cantidad necesaria, completar la tarea
        if (limpiadas >= manchasNecesarias)
        {
            CompletarTarea();
        }
    }

    // --- Preparación para VR (comentado) ---
    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && !EstaCompletada)
    //     {
    //         // Lógica para limpiar por proximidad en VR
    //     }
    // }
}