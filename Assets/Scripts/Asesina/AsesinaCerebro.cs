using UnityEngine;
using UnityEngine.AI;

public class AsesinaCerebro : MonoBehaviour
{
    // ---- Referencias ----
    [Header("Referencias")]
    public Transform jugador;
    public NavMeshAgent agente;
    public Transform[] puntosPatrulla;
    public Renderer rendererCuerpo;

    [Header("Detección por visión (Raycast)")]
    public float radioVision = 6f;
    public float alturaOjos = 1.2f;
    public LayerMask mascaraDeteccion;

    [Header("Comportamiento de Patrulla")]
    public float tiempoEsperaPatrulla = 2f;

    [Header("Memoria del enemigo")]
    public float tiempoAntesDeOlvidar = 2f;

    [Header("Velocidades")]
    public float velocidadPatrulla = 2.5f;
    public float velocidadPersecucion = 5f;

    // ---- Estado actual ----
    private enum Estado { Patrulla, Persecucion }
    private Estado estadoActual = Estado.Patrulla;

    // ---- Variables internas ----
    private int indicePatrulla = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private bool jugadorVisible = false;
    private float temporizadorPerdida = 0f;

    // ---- Colores para cada estado ----
    private Color colorPatrulla = Color.yellow;
    private Color colorPersecucion = Color.red;

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        if (rendererCuerpo == null)
            rendererCuerpo = GetComponentInChildren<Renderer>();

        if (puntosPatrulla.Length > 0)
        {
            agente.SetDestination(puntosPatrulla[0].position);
        }

        // Aplicar la configuración inicial (color y velocidad)
        ActualizarApariencia();
    }

    void Update()
    {
        jugadorVisible = IsPlayerVisible();

        // Transiciones de estado
        if (estadoActual == Estado.Patrulla && jugadorVisible)
        {
            estadoActual = Estado.Persecucion;
            temporizadorPerdida = 0f;
            Debug.Log("¡Jugador avistado! Persiguiendo.");
            ActualizarApariencia();
        }
        else if (estadoActual == Estado.Persecucion && !jugadorVisible)
        {
            temporizadorPerdida += Time.deltaTime;

            if (temporizadorPerdida >= tiempoAntesDeOlvidar)
            {
                estadoActual = Estado.Patrulla;
                Debug.Log("Olvidé al jugador. Vuelvo a patrullar.");

                if (puntosPatrulla.Length > 0)
                {
                    indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
                    agente.SetDestination(puntosPatrulla[indicePatrulla].position);
                }
                esperando = false;
                temporizadorEspera = 0f;
                temporizadorPerdida = 0f;
                ActualizarApariencia();
            }
        }
        else
        {
            if (jugadorVisible)
                temporizadorPerdida = 0f;
        }

        // Ejecutar el estado actual
        switch (estadoActual)
        {
            case Estado.Patrulla:
                EstadoPatrulla();
                break;
            case Estado.Persecucion:
                EstadoPersecucion();
                break;
        }
    }

    // -------------------- RAYCAST --------------------
    bool IsPlayerVisible()
    {
        if (jugador == null) return false;

        Vector3 origen = transform.position + Vector3.up * alturaOjos;
        Vector3 objetivo = jugador.position + Vector3.up * 1f;
        Vector3 direccion = objetivo - origen;
        float distancia = direccion.magnitude;

        if (distancia > radioVision) return false;

        RaycastHit hit;
        if (Physics.Raycast(origen, direccion, out hit, radioVision, mascaraDeteccion))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
            else
                return false;
        }
        return false;
    }

    // -------------------- PATRULLA --------------------
    void EstadoPatrulla()
    {
        if (puntosPatrulla.Length == 0) return;

        if (!agente.pathPending && agente.remainingDistance < 0.5f && !esperando)
        {
            esperando = true;
            temporizadorEspera = tiempoEsperaPatrulla;
            agente.isStopped = true;
        }

        if (esperando)
        {
            temporizadorEspera -= Time.deltaTime;
            if (temporizadorEspera <= 0f)
            {
                esperando = false;
                agente.isStopped = false;
                indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
                agente.SetDestination(puntosPatrulla[indicePatrulla].position);
            }
        }
    }

    // -------------------- PERSECUCIÓN --------------------
    void EstadoPersecucion()
    {
        agente.SetDestination(jugador.position);
        if (agente.isStopped) agente.isStopped = false;
    }

    void ActualizarApariencia()
    {
        // 1. Cambiar color
        if (rendererCuerpo != null)
        {
            Color nuevoColor = (estadoActual == Estado.Patrulla) ? colorPatrulla : colorPersecucion;
            rendererCuerpo.material.color = nuevoColor;
        }

        // 2. Cambiar velocidad del agente
        if (agente != null)
        {
            agente.speed = (estadoActual == Estado.Patrulla) ? velocidadPatrulla : velocidadPersecucion;
        }

    }

    // -------------------- PROPIEDAD PÚBLICA PARA CONSULTAR EL ESTADO --------------------
    /// Devuelve true si la asesina está en modo persecución.

    public bool EstaPersiguiendo
    {
        get { return estadoActual == Estado.Persecucion; }
    }

    // -------------------- GIZMOS --------------------
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioVision);

        if (Application.isPlaying && jugador != null)
        {
            Vector3 origen = transform.position + Vector3.up * alturaOjos;
            Vector3 objetivo = jugador.position + Vector3.up * 1f;
            Gizmos.color = jugadorVisible ? Color.green : Color.red;
            Gizmos.DrawLine(origen, objetivo);
        }
    }
}