using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class ObjetoRecogible : MonoBehaviour, IInteractuable
{
    public RecogerObjetoPerdidos tarea;

    public Transform puntoDeOrigen;

    [HideInInspector] public float bloquearEntregaHasta = 0f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool agarradoPorMouse = false;
    private Transform camara;

    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (puntoDeOrigen != null)
        {
            posicionInicial = puntoDeOrigen.position;
            rotacionInicial = puntoDeOrigen.rotation;
        }
        else
        {
            Debug.LogError($"❌ ¡El objeto '{gameObject.name}' no tiene Punto De Origen asignado! Regresará al lugar equivocado.");
            posicionInicial = transform.position;
            rotacionInicial = transform.rotation;
        }
    }

    void Start()
    {
        if (tarea == null) tarea = FindAnyObjectByType<RecogerObjetoPerdidos>();
        if (Camera.main != null) camara = Camera.main.transform;
    }

    void Update()
    {
        if (tarea == null) return;
        bool puedeAgarrar = tarea.EstaIniciada && !tarea.EstaCompletada && !tarea.objetosRecuperados.Contains(gameObject);
        if (grabInteractable != null && grabInteractable.enabled != puedeAgarrar) grabInteractable.enabled = puedeAgarrar;
    }

    public void Interactuar()
    {
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
        bloquearEntregaHasta = Time.time + 1.5f;

        if (puntoDeOrigen != null)
        {
            posicionInicial = puntoDeOrigen.position;
            rotacionInicial = puntoDeOrigen.rotation;
        }

        gameObject.SetActive(true);

        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.position = posicionInicial;
            rb.rotation = rotacionInicial;
        }

        transform.SetPositionAndRotation(posicionInicial, rotacionInicial);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"'{gameObject.name}' volvió a su punto de origen.");
    }

    private void OnDrawGizmosSelected()
    {
        if (puntoDeOrigen != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, puntoDeOrigen.position);
            Gizmos.DrawWireSphere(puntoDeOrigen.position, 0.2f);
        }
    }
}