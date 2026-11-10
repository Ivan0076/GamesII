using UnityEngine;
using UnityEngine.AI;

public enum EstadosAsesina
{
    esta_atacando,
    esta_persiguiendo,
}

public class AsesinaCerebro : MonoBehaviour
{
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

    private enum Estado { Patrulla, Persecucion }
    private Estado estadoActual = Estado.Patrulla;

    private int indicePatrulla = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private bool jugadorVisible = false;
    private float temporizadorPerdida = 0f;

    private Color colorPatrulla = Color.yellow;
    private Color colorPersecucion = Color.red;

    // Cambiamos Start por OnEnable para que se reinicie cada vez que el Director la invoque
    void OnEnable()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        if (rendererCuerpo == null)
            rendererCuerpo = GetComponentInChildren<Renderer>();

        // Reiniciamos variables de estado
        estadoActual = Estado.Patrulla;
        esperando = false;
        temporizadorEspera = 0f;
        temporizadorPerdida = 0f;
        indicePatrulla = 0;

        if (puntosPatrulla.Length > 0 && agente != null)
        {
            agente.isStopped = false;
            agente.SetDestination(puntosPatrulla[0].position);
        }

        ActualizarApariencia();
    }

    void Update()
    {
        jugadorVisible = IsPlayerVisible();

        if (estadoActual == Estado.Patrulla && jugadorVisible)
        {
            estadoActual = Estado.Persecucion;
            temporizadorPerdida = 0f;
            ActualizarApariencia();
        }
        else if (estadoActual == Estado.Persecucion && !jugadorVisible)
        {
            temporizadorPerdida += Time.deltaTime;

            if (temporizadorPerdida >= tiempoAntesDeOlvidar)
            {
                estadoActual = Estado.Patrulla;

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
            if (hit.collider.CompareTag("Player")) return true;
            else return false;
        }
        return false;
    }

    void EstadoPatrulla()
    {
        if (puntosPatrulla.Length == 0 || agente == null) return;

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

    void EstadoPersecucion()
    {
        if (agente != null)
        {
            agente.SetDestination(jugador.position);
            if (agente.isStopped) agente.isStopped = false;
        }
    }

    void ActualizarApariencia()
    {
        if (rendererCuerpo != null)
        {
            Color nuevoColor = (estadoActual == Estado.Patrulla) ? colorPatrulla : colorPersecucion;
            rendererCuerpo.material.color = nuevoColor;
        }

        if (agente != null)
        {
            agente.speed = (estadoActual == Estado.Patrulla) ? velocidadPatrulla : velocidadPersecucion;
        }
    }

    public bool EstaPersiguiendo
    {
        get { return estadoActual == Estado.Persecucion; }
    }

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