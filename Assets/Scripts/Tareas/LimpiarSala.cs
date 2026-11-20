using UnityEngine;
using System.Collections.Generic;

public class LimpiarSala : TareaBase
{
    [Header("Manchas a limpiar (Arrastrar desde la escena)")]
    public List<Mancha> manchas = new List<Mancha>();

    public void LimpiarMancha(Mancha mancha)
    {
        if (EstaCompletada) return;

        if (manchas.Contains(mancha))
        {
            manchas.Remove(mancha);
            NotificarProgreso();
            Debug.Log($"Mancha limpiada. Quedan: {manchas.Count}");

            if (manchas.Count == 0) CompletarTarea();
        }
    }

    public override string ObtenerProgreso() => $"{manchas.Count} restantes";
}