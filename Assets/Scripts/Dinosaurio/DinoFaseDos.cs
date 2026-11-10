using UnityEngine;
using UnityEngine.AI;

public class DinoFaseDos : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad;

    [Header("Detección de mirada")]
    [Range(0.7f, 1f)] public float umbralMirada = 0.9f;
    public LayerMask capasObstaculo;

    [Header("Animación")]
    public string triggerIniciarFase2 = "IniciarFase2";

    [Header("Rendición (Tiempo límite)")]
    public float tiempoAntesDeRendirse = 45f;
    public AudioSource audioSource;
    public AudioClip sonidoRendicion;

    [Header("Referencias")]
    public Transform jugador;
    public Camera camaraJugador;

    [Header("Debug")]
    public bool mostrarLogs = true;

    public System.Action OnDinoSeRinde;

    private NavMeshAgent agente;
    private Animator animator;
    private GestorSpawns gestorSpawns; // Referencia interna
    private bool activo = false;
    private bool mirando = false;
    private float tiempoEnFase2 = 0f;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // Buscamos el GestorSpawns en el mismo GameObject
        gestorSpawns = GetComponent<GestorSpawns>();

        if (jugador == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) jugador = go.transform;
        }

        if (camaraJugador == null && jugador != null)
            camaraJugador = jugador.GetComponentInChildren<Camera>(true);
        if (camaraJugador == null)
            camaraJugador = Camera.main;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void IniciarFase()
    {
        activo = true;
        tiempoEnFase2 = 0f;

        if (agente == null || jugador == null || camaraJugador == null) return;

        // Usamos el GestorSpawns interno
        Vector3 pos = transform.position;
        if (gestorSpawns != null)
        {
            pos = gestorSpawns.ObtenerPuntoAleatorio();
        }

        agente.enabled = true;
        agente.speed = velocidad;
        agente.isStopped = false;

        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 20f, NavMesh.AllAreas))
        {
            agente.Warp(hit.position);
            if (mostrarLogs) Debug.Log($"Spawn en {hit.position}");
        }

        if (animator != null && !string.IsNullOrEmpty(triggerIniciarFase2))
        {
            animator.SetTrigger(triggerIniciarFase2);
            animator.speed = 1f;
        }
        else if (animator != null)
        {
            animator.Play("Caminar", 0, 0f);
            animator.speed = 1f;
        }
    }

    void Update()
    {
        if (!activo) return;
        if (agente == null || jugador == null || camaraJugador == null) return;
        if (!agente.enabled || !agente.isOnNavMesh) return;

        tiempoEnFase2 += Time.deltaTime;
        if (tiempoEnFase2 >= tiempoAntesDeRendirse)
        {
            Rendirse();
            return;
        }

        bool mirandoAhora = EstaSiendoMirado();
        mirando = mirandoAhora;

        if (mirando)
        {
            agente.isStopped = true;
            if (animator != null) animator.speed = 0f;
        }
        else
        {
            agente.isStopped = false;
            agente.SetDestination(jugador.position);
            if (animator != null) animator.speed = 1f;
        }
    }

    private void Rendirse()
    {
        activo = false;
        if (agente != null) agente.isStopped = true;
        if (audioSource != null && sonidoRendicion != null) audioSource.PlayOneShot(sonidoRendicion);
        OnDinoSeRinde?.Invoke();
        gameObject.SetActive(false);
    }

    private bool EstaSiendoMirado()
    {
        Vector3 posCam = camaraJugador.transform.position;
        Vector3 posDino = transform.position + Vector3.up * 1.5f;
        Vector3 dir = (posDino - posCam).normalized;
        float dist = Vector3.Distance(posCam, posDino);

        float dot = Vector3.Dot(camaraJugador.transform.forward, dir);
        if (dot < umbralMirada) return false;

        if (Physics.Raycast(posCam, dir, out RaycastHit hit, dist, capasObstaculo, QueryTriggerInteraction.Ignore))
        {
            bool esElDino = hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform);
            if (!esElDino) return false;
        }
        return true;
    }
}