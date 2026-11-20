using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoRecogible : MonoBehaviour, IInteractuable
{
    public RecogerObjetoPerdidos tarea;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool agarradoPorMouse = false;
    private Transform camara;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (tarea == null) tarea = FindFirstObjectByType<RecogerObjetoPerdidos>();
        camara = Camera.main.transform;

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        if (tarea == null) return;

        // Comprobamos si el jugador debería poder agarrar el objeto
        bool puedeAgarrar = tarea.EstaIniciada && !tarea.EstaCompletada && !tarea.objetosRecuperados.Contains(gameObject);

        // Activar/Desactivar el agarre en VR dinámicamente
        if (grabInteractable != null && grabInteractable.enabled != puedeAgarrar)
        {
            grabInteractable.enabled = puedeAgarrar;
        }
    }

    // Interacción para teclado/mouse
    public void Interactuar()
    {
        // Validaciones de seguridad
        if (tarea == null || !tarea.EstaIniciada || tarea.EstaCompletada) return;
        if (tarea.objetosRecuperados.Contains(gameObject)) return;

        if (!agarradoPorMouse)
        {
            agarradoPorMouse = true;
            rb.isKinematic = true;
            transform.SetParent(camara);
            transform.localPosition = new Vector3(0, -0.3f, 1f);
        }
        else
        {
            agarradoPorMouse = false;
            rb.isKinematic = false;
            transform.SetParent(null);
        }
    }

    public void DevolverAlOrigen()
    {
        agarradoPorMouse = false;
        transform.SetParent(null);

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

        gameObject.SetActive(true);

        // El Update() se encargará de volver a activar el agarre si la tarea sigue en curso
    }
}