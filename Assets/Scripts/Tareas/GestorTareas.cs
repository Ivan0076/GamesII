using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorTareas : MonoBehaviour
{
    [Header("Lista de tareas (asignar en el Inspector)")]
    public List<TareaBase> tareas;

    private int tareasCompletadas = 0;

    // Evento que se dispara cuando cualquier tarea se completa
    public event System.Action<ITarea> TareaCompletada;

    public event System.Action TodasLasTareasCompletadas;

    void Start()
    {
        EvaluarTareas();
    }

    public void EvaluarTareas()
    {
        foreach (var t in tareas)
        {
            if (t != null && !t.EstaCompletada && t.PuedeIniciarse())
            {
                t.IniciarTarea();
            }
        }
    }

    public void RegistrarTareaCompletada(ITarea tarea)
    {
        tareasCompletadas++;
        Debug.Log($"Tarea completada: {tarea.nombreTarea} ({tareasCompletadas}/{tareas.Count})");

        // Disparar evento para cada tarea completada
        if (TareaCompletada != null)
            TareaCompletada.Invoke(tarea);

        EvaluarTareas();

        if (tareasCompletadas >= tareas.Count)
        {
            Debug.Log("¡Todas las tareas completadas! La noche ha terminado.");

            GameManager.Instance.nocheActual++;

            Debug.Log("Siguiente noche: " + GameManager.Instance.nocheActual);

            UnityEngine.SceneManagement.SceneManager.LoadScene("TransicionNoche");
        }
    }
}