using UnityEngine;

public class RecogerObjetosPerdidos : TareaBase
{
    [Header("Configuración de recogida")]
    public int objetosNecesarios = 3;      // Cantidad de objetos a recoger
    private int recogidos = 0;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        recogidos = 0;
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Recoge {objetosNecesarios} objetos.");
    }

    /// <summary>
    /// Método llamado desde cada objeto recogible cuando se interactúa con él.
    /// </summary>
    public void RecogerObjeto()
    {
        // Evitar que se siga recogiendo si ya está completada o se alcanzó el límite
        if (recogidos >= objetosNecesarios || EstaCompletada) return;

        recogidos++;
        Debug.Log($"Objeto recogido: {recogidos}/{objetosNecesarios}");

        // Si se alcanzó la cantidad necesaria, completar la tarea
        if (recogidos >= objetosNecesarios)
        {
            CompletarTarea();
        }
    }

    // --- Preparación para VR (comentado) ---
    // public void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player") && !EstaCompletada)
    //     {
    //         // Lógica para recoger por proximidad en VR
    //     }
    // }
}