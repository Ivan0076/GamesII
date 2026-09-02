using UnityEngine;
using System.Collections.Generic;

public class EnemigoRequisitos : MonoBehaviour
{
    [Header("Prerrequisitos para activar este enemigo")]
    public List<TareaBase> tareasNecesarias;

    [Header("Opciones")]
    public bool comprobarAlInicio = true;

    private bool activado = false;

    void Start()
    {
        GestorTareas gestor = FindFirstObjectByType<GestorTareas>();
        if (gestor != null)
        {
            gestor.TareaCompletada += OnTareaCompletada;
        }
        else
        {
            Debug.LogWarning("No se encontró GestorTareas. Los requisitos no se actualizarán.");
        }

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

        // Llamar a IniciarEnemigo en el ControlAsesina o ControlDino del mismo GameObject
        SendMessage("IniciarEnemigo", SendMessageOptions.DontRequireReceiver);
    }
}