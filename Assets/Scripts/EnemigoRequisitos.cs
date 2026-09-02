using UnityEngine;
using System.Collections.Generic;

public class EnemigoRequisitos : MonoBehaviour
{
    [Header("Prerrequisitos")]
    public List<TareaBase> tareasNecesarias;

    [Header("Opciones")]
    public bool comprobarAlInicio = true;

    // Referencia a los controladores (puedes usar uno u otro)
    public ControlAsesina controlAsesina;
    public ControlDino controlDino;

    private bool activado = false;

    void Start()
    {
        // Buscar controladores si no se asignaron
        if (controlAsesina == null)
            controlAsesina = GetComponent<ControlAsesina>();
        if (controlDino == null)
            controlDino = GetComponent<ControlDino>();

        // Suscribirse al evento de tareas completadas
        GestorTareas gestor = FindFirstObjectByType<GestorTareas>();
        if (gestor != null)
            gestor.TareaCompletada += OnTareaCompletada;

        if (comprobarAlInicio)
            EvaluarRequisitos();
    }

    void OnDestroy()
    {
        GestorTareas gestor = FindFirstObjectByType<GestorTareas>();
        if (gestor != null)
            gestor.TareaCompletada -= OnTareaCompletada;
    }

    public void OnTareaCompletada(ITarea tarea)
    {
        if (activado) return;
        if (tareasNecesarias != null && tareasNecesarias.Contains((TareaBase)tarea))
        {
            EvaluarRequisitos();
        }
    }

    public void EvaluarRequisitos()
    {
        if (activado) return;

        if (tareasNecesarias == null || tareasNecesarias.Count == 0)
        {
            ActivarEnemigo();
            return;
        }

        bool todasCompletas = true;
        foreach (var tarea in tareasNecesarias)
        {
            if (tarea == null || !tarea.EstaCompletada)
            {
                todasCompletas = false;
                break;
            }
        }

        if (todasCompletas)
            ActivarEnemigo();
    }

    private void ActivarEnemigo()
    {
        if (activado) return;
        activado = true;

        Debug.Log($"Enemigo '{gameObject.name}' activado (requisitos cumplidos).");

        // Llamar al controlador correspondiente
        if (controlAsesina != null)
            controlAsesina.IniciarEnemigo();
        else if (controlDino != null)
            controlDino.IniciarDino();
        else
            gameObject.SetActive(true);
    }
}