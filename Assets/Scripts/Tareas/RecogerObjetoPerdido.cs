using UnityEngine;
using System.Collections.Generic;

public class RecogerObjetoPerdidos : TareaBase
{
    [Header("Objetos a recolectar (Arrastrar desde la escena)")]
    public List<GameObject> objetos = new List<GameObject>();

    [Header("Objetos ya entregados (No asignar, se llena solo)")]
    public List<GameObject> objetosRecuperados = new List<GameObject>();

    // Evento que avisa cuando el jugador entrega un objeto en la zona
    public System.Action OnObjetoEntregado;

    public bool EntregarObjeto(GameObject objeto)
    {
        if (EstaCompletada) return false;
        if (objetosRecuperados.Contains(objeto)) return false;

        if (objetos.Contains(objeto))
        {
            objetos.Remove(objeto);
            objetosRecuperados.Add(objeto);
            NotificarProgreso();
            Debug.Log($"Objeto entregado. Quedan pendientes: {objetos.Count}");

            OnObjetoEntregado?.Invoke();

            if (objetos.Count == 0) CompletarTarea();
            return true;
        }
        return false;
    }

    public void ActualizarProgresoExterno()
    {
        NotificarProgreso();
    }
    public override string ObtenerProgreso() => $"{objetos.Count} restantes";
}