using UnityEngine;
using System.Collections.Generic;

public class ApagarProyectores : TareaBase
{
    [Header("Proyectores a apagar (Arrastrar desde la escena)")]
    public List<Proyector> proyectores = new List<Proyector>();

    public void ApagarProyector(Proyector proyector)
    {
        if (EstaCompletada) return;

        // Si el proyector está en la lista, lo quitamos
        if (proyectores.Contains(proyector))
        {
            proyectores.Remove(proyector);
            NotificarProgreso(); // Actualiza el portapapeles
            Debug.Log($"Proyector apagado. Quedan: {proyectores.Count}");

            if (proyectores.Count == 0) CompletarTarea();
        }
    }

    public override string ObtenerProgreso() => $"{proyectores.Count} restantes";
}