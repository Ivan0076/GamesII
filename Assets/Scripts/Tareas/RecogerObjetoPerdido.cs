using UnityEngine;

public class RecogerObjetoPerdidos : TareaBase
{
    [Header("Configuración de recogida")]
    public int objetosNecesarios = 3;
    private int recogidos = 0;

    public override void IniciarTarea()
    {
        // Prevenir reinicios accidentales
        if (EstaIniciada || EstaCompletada) return;

        base.IniciarTarea(); // Esto establece iniciada = true
        recogidos = 0;
        NotificarProgreso();
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Recoge {objetosNecesarios} objetos.");
    }

    public void RecogerObjeto()
    {
        if (recogidos >= objetosNecesarios || EstaCompletada) return;

        recogidos++;
        NotificarProgreso();
        Debug.Log($"Objeto recogido: {recogidos}/{objetosNecesarios}");

        if (recogidos >= objetosNecesarios)
            CompletarTarea();
    }

    public override string ObtenerProgreso()
    {
        return $"{recogidos}/{objetosNecesarios}";
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




