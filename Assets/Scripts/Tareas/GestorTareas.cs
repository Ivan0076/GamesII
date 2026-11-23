using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorTareas : MonoBehaviour
{
    [Header("Lista de tareas (asignar en el Inspector)")]
    public List<TareaBase> tareas;

    public event System.Action<ITarea> TareaCompletada;
    public event System.Action TodasLasTareasCompletadas;

    void Start()
    {
        EvaluarTareas();
    }

    public void EvaluarTareas()
    {
        foreach (var t in tareas)
            if (t != null && !t.EstaCompletada && t.PuedeIniciarse())
                t.IniciarTarea();
    }

    public void RegistrarTareaCompletada(ITarea tarea)
    {
        Debug.Log($"Tarea completada: {tarea.nombreTarea}");

        TareaCompletada?.Invoke(tarea);
        EvaluarTareas();

        // Contar directo de la lista (robusto)
        int total = 0, completadas = 0;
        foreach (var t in tareas)
        {
            if (t == null) continue;
            total++;
            if (t.EstaCompletada) completadas++;
        }

        Debug.Log($"Progreso global: {completadas}/{total}");

        if (total > 0 && completadas >= total)
        {
            Debug.Log("¡Todas las tareas completadas! La noche ha terminado.");
            TodasLasTareasCompletadas?.Invoke();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.nocheActual++;
                SceneManager.LoadScene("TransicionNoche");
            }
        }
    }
}