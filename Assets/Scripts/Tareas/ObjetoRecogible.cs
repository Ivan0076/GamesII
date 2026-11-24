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

    // 🔥 AWKAE SE EJECUTA TAN PRONTO COMO CARGA LA ESCENA
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Congelamos la física temporalmente para que el objeto no caiga antes de sacar la posición
        if (rb != null) rb.isKinematic = true;

        // 🔥 EL TRUCO DEL RAYCAST: Ajustar al suelo inmediatamente
        Vector3 puntoDeAparicion = transform.position;

        // Lanzamos un rayo hacia abajo desde el objeto, hasta 5 metros
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 5f))
        {
            // Si toca el suelo, guardamos ese punto exacto
            puntoDeAparicion = hit.point;

            // Lo subimos un poquito para que no se hunda en el piso
            puntoDeAparicion += Vector3.up * 0.05f;
        }

        posicionInicial = puntoDeAparicion;
        rotacionInicial = transform.rotation;

        // Lo movemos al punto exacto calculado
        transform.position = posicionInicial;

        Debug.Log($"📦 '{gameObject.name}' guardó su origen instantáneamente en: {posicionInicial}");
    }

    void Start()
    {
        // El resto de inicializaciones van en Start
        if (tarea == null) tarea = FindAnyObjectByType<RecogerObjetoPerdidos>();

        if (Camera.main != null) camara = Camera.main.transform;

        // Devolvemos las físicas a la normalidad una vez que ya tenemos la posición
        if (rb != null) rb.isKinematic = false;
    }

    void Update()
    {
        if (tarea == null) return;

        bool puedeAgarrar = tarea.EstaIniciada && !tarea.EstaCompletada && !tarea.objetosRecuperados.Contains(gameObject);

        if (grabInteractable != null && grabInteractable.enabled != puedeAgarrar)
        {
            grabInteractable.enabled = puedeAgarrar;
        }
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

        rb.isKinematic = true;

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        Debug.Log($"🔄 ¡El fantasma teletransportó '{gameObject.name}' a {posicionInicial}!");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

        gameObject.SetActive(true);
    }

    // Herramienta visual para ver el origen en el Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(posicionInicial, 0.1f);
    }
}