using UnityEngine;

public class ApagarProyectores : TareaBase
{
    [Header("Configuraci�n de proyectores")]
    public int proyectoresNecesarios = 2;   // Cantidad de proyectores a apagar
    private int apagados = 0;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        apagados = 0;
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Apaga {proyectoresNecesarios} proyectores.");
    }

    /// <summary>
    /// M�todo llamado desde cada proyector cuando se interact�a con �l.
    /// </summary>
    public void ApagarProyector()
    {
        // Evitar que se siga apagando si ya est� completada o se alcanz� el l�mite
        if (apagados >= proyectoresNecesarios || EstaCompletada) return;

        apagados++;
        Debug.Log($"Proyector apagado: {apagados}/{proyectoresNecesarios}");

        // Si se alcanz� la cantidad necesaria, completar la tarea
        if (apagados >= proyectoresNecesarios)
        {
            CompletarTarea();
        }
    }

    // --- Preparaci�n para VR (comentado) ---
    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && !EstaCompletada)
    //     {
    //         // L�gica para apagar por proximidad en VR
    //     }
    // }
}