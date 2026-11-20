using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Proyector : MonoBehaviour, IInteractuable
{
    public ApagarProyectores tarea;
    public GameObject luzProyector;
    public VideoPlayer videoPlayer;
    public AudioSource audioProyector;
    public Material materialApagado;

    private bool apagado = false;

    void Start()
    {
        if (tarea == null) tarea = FindAnyObjectByType<ApagarProyectores>();

        XRSimpleInteractable interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
            interactable.selectEntered.AddListener((args) => Interactuar());
    }

    public void Interactuar()
    {
        if (apagado || tarea == null || !tarea.EstaIniciada || tarea.EstaCompletada) return;

        apagado = true;

        if (luzProyector != null) luzProyector.SetActive(false);
        if (videoPlayer != null) videoPlayer.Stop();
        if (audioProyector != null) audioProyector.Stop();
        if (materialApagado != null) GetComponent<Renderer>().material = materialApagado;

        // Desactivar collider para que no se pueda volver a interactuar
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        tarea.ApagarProyector(this);
    }
}