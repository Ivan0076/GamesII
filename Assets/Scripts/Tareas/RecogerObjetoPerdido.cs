using UnityEngine;
using System.Collections.Generic;

public class RecogerObjetoPerdidos : TareaBase
{
    [Header("Objetos a recolectar (Arrastrar desde la escena)")]
    public List<GameObject> objetos = new List<GameObject>();

    [Header("Objetos ya entregados (No asignar, se llena solo)")]
    public List<GameObject> objetosRecuperados = new List<GameObject>();

    public bool EntregarObjeto(GameObject objeto)
    {
        if (EstaCompletada) return false;
        if (objetosRecuperados.Contains(objeto)) return false;

        if (objetos.Contains(objeto))
        {
            objetos.Remove(objeto);
            objetosRecuperados.Add(objeto); // Se guarda en la lista de recuperados
            NotificarProgreso(); // 🔥 Actualiza el portapapeles (ej. de 0/3 a 1/3)
            Debug.Log($"Objeto entregado. Quedan pendientes: {objetos.Count}");

            if (objetos.Count == 0) CompletarTarea();
            return true;
        }
        return false;
    }

    public void DevolverUnObjetoAlAzar()
    {
        // Si no hay objetos recuperados, no hace nada
        if (objetosRecuperados.Count == 0)
        {
            Debug.Log("El enemigo intentó devolver un objeto, pero la lista está vacía.");
            return;
        }

        // Elegimos un índice al azar de la lista de recuperados
        int indiceAleatorio = Random.Range(0, objetosRecuperados.Count);
        GameObject objetoA_Devolver = objetosRecuperados[indiceAleatorio];

        // Le decimos al objeto que regrese a su punto inicial (Punto X)
        ObjetoRecogible scriptObjeto = objetoA_Devolver.GetComponent<ObjetoRecogible>();
        if (scriptObjeto != null)
        {
            scriptObjeto.DevolverAlOrigen();
        }

        // Lo sacamos de la lista de recuperados y lo volvemos a poner en la de pendientes
        objetosRecuperados.RemoveAt(indiceAleatorio);
        objetos.Add(objetoA_Devolver);

        NotificarProgreso();

        Debug.Log($"¡El enemigo devolvió el objeto '{objetoA_Devolver.name}' a su lugar!");
    }

    public override string ObtenerProgreso() => $"{objetos.Count} restantes";
}