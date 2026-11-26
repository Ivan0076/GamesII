using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class BasuraRecogible : MonoBehaviour, IInteractuable
{
    public RecogerBasuraTarea tarea;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool agarradoPorMouse = false;
    private Transform camara;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void Start()
    {
        if (tarea == null) tarea = FindAnyObjectByType<RecogerBasuraTarea>();
        if (Camera.main != null) camara = Camera.main.transform;

        if (tarea != null)
        {
            tarea.RegistrarBasura(gameObject);
        }
    }

    void Update()
    {
        if (tarea == null) return;
        bool puedeAgarrar = tarea.EstaIniciada && !tarea.EstaCompletada;
        if (grabInteractable != null && grabInteractable.enabled != puedeAgarrar)
        {
            grabInteractable.enabled = puedeAgarrar;
        }
    }

    public void Interactuar()
    {
        if (tarea == null || !tarea.EstaIniciada || tarea.EstaCompletada) return;

        if (!agarradoPorMouse)
        {
            if (camara == null && Camera.main != null) camara = Camera.main.transform;
            if (camara == null) return;

            agarradoPorMouse = true;
            rb.isKinematic = true;
            transform.SetParent(camara);
            transform.localPosition = new Vector3(0f, -0.3f, 1f);
        }
        else
        {
            SoltarPorMouse();
        }
    }

    public void SoltarPorMouse()
    {
        if (!agarradoPorMouse) return;
        agarradoPorMouse = false;
        transform.SetParent(null);
        if (rb != null) rb.isKinematic = false;
    }
}