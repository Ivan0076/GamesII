using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Mancha : MonoBehaviour, IInteractuable
{
    public LimpiarSala tarea;
    public Material materialLimpio;
    private bool limpiada = false;

    void Start()
    {
        if (tarea == null) tarea = FindAnyObjectByType<LimpiarSala>();

        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener((args) => Interactuar());
    }

    public void Interactuar()
    {
        if (limpiada || tarea == null || !tarea.EstaIniciada || tarea.EstaCompletada) return;

        limpiada = true;

        if (materialLimpio != null) GetComponent<Renderer>().material = materialLimpio;

        tarea.LimpiarMancha(this);
        gameObject.SetActive(false); // O desactiva solo el collider
    }
}