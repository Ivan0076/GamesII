using UnityEngine;

public class ApagarProyectores : TareaBase
{
    [Header("Configuración de proyectores")]
    public int proyectoresNecesarios = 2;   // Cantidad de proyectores a apagar
    private int apagados = 0;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        apagados = 0;
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Apaga {proyectoresNecesarios} proyectores.");
    }

    /// <summary>
    /// Método llamado desde cada proyector cuando se interactúa con él.
    /// </summary>
    public void ApagarProyector()
    {
        // Evitar que se siga apagando si ya está completada o se alcanzó el límite
        if (apagados >= proyectoresNecesarios || EstaCompletada) return;

        apagados++;
        Debug.Log($"Proyector apagado: {apagados}/{proyectoresNecesarios}");

        // Si se alcanzó la cantidad necesaria, completar la tarea
        if (apagados >= proyectoresNecesarios)
        {
            CompletarTarea();
        }
    }

    // --- Preparación para VR (comentado) ---
    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && !EstaCompletada)
    //     {
    //         // Lógica para apagar por proximidad en VR
    //     }
    // }
}