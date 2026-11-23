using UnityEngine;

public class ApagarProyectores : TareaBase
{
    [Header("Configuración de la Noche")]
    [Tooltip("¿Cuántos proyectores debe apagar el jugador esta noche?")]
    public int proyectoresNecesarios = 2;   // Lo cambias desde el Inspector por noche
    private int apagados = 0;

    public override void IniciarTarea()
    {
        // Si ya se inició, no reseteamos el contador por accidente
        if (EstaIniciada) return;

        base.IniciarTarea();
        apagados = 0;
        NotificarProgreso(); // Actualiza el portapapeles a "0/X" al iniciar
        Debug.Log($"Tarea '{nombreTarea}' iniciada. Apaga {proyectoresNecesarios} proyectores.");
    }

    public void ApagarProyector()
    {
        // Evitar que se siga apagando si ya se completó o se alcanzó el límite
        if (apagados >= proyectoresNecesarios || EstaCompletada) return;

        apagados++;
        NotificarProgreso(); // Actualiza el portapapeles
        Debug.Log($"Proyector apagado: {apagados}/{proyectoresNecesarios}");

        // Si se alcanzó la cantidad necesaria, completar la tarea
        if (apagados >= proyectoresNecesarios)
        {
            CompletarTarea();
        }
    }

    // Muestra el progreso en el portapapeles: "1/2", "2/5", etc.
    public override string ObtenerProgreso() => $"{apagados}/{proyectoresNecesarios}";
}