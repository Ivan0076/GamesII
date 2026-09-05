using UnityEngine;

public class LimpiarSala : TareaBase
{
    [Header("Configuración de limpieza")]
    public int manchasNecesarias = 4;
    private int limpiadas = 0;

    public override void IniciarTarea()
    {
        if (EstaIniciada || EstaCompletada) return;

        base.IniciarTarea();
        limpiadas = 0;
        NotificarProgreso();
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Limpia {manchasNecesarias} manchas.");
    }

    public void LimpiarMancha()
    {
        if (limpiadas >= manchasNecesarias || EstaCompletada) return;

        limpiadas++;
        NotificarProgreso();
        Debug.Log($"Mancha limpiada: {limpiadas}/{manchasNecesarias}");

        if (limpiadas >= manchasNecesarias)
            CompletarTarea();
    }

    public override string ObtenerProgreso()
    {
        return $"{limpiadas}/{manchasNecesarias}";
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


