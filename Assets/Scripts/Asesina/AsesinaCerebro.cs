using UnityEngine;
using UnityEngine.AI;

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
    public float velocidadPersecucion = 5.5f; // Un poco más rápida que el jugador

    [Header("Persecución Mejorada")]
    [Tooltip("Distancia a la que se detendrá frente al jugador (para no empujarlo)")]
    public float distanciaParada = 1.2f;
    [Tooltip("Cuánto tiempo pasa entre recálculos de ruta (0.1 es óptimo)")]
    private float tiempoActualizacionRuta = 0f;

    [Header("Audio de Alerta")]
    public AudioClip sonidoAlertaJugador;
    public AudioClip sonidoAsesina;

    private AudioSource audioSourceJugador;
    private AudioSource audioSourceAsesina;

    private enum Estado { Patrulla, Persecucion }
    private Estado estadoActual = Estado.Patrulla;

    private int indicePatrulla = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private bool jugadorVisible = false;
    private float temporizadorPerdida = 0f;

    private Color colorPatrulla = Color.yellow;
    private Color colorPersecucion = Color.red;

    void OnEnable()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        if (rendererCuerpo == null)
            rendererCuerpo = GetComponentInChildren<Renderer>();

        audioSourceAsesina = GetComponent<AudioSource>();
        if (audioSourceAsesina == null) audioSourceAsesina = gameObject.AddComponent<AudioSource>();

        if (jugador != null)
        {
            audioSourceJugador = jugador.GetComponentInChildren<AudioSource>();
            if (audioSourceJugador == null)
            {
                Camera cam = jugador.GetComponentInChildren<Camera>();
                if (cam != null) audioSourceJugador = cam.gameObject.AddComponent<AudioSource>();
            }
        }

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

            ReproducirSonidosDeDeteccion();
            GirarHaciaElJugadorInstantaneamente();
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
            if (jugadorVisible) temporizadorPerdida = 0f;
        }

        switch (estadoActual)
        {
            case Estado.Patrulla:
                EstadoPatrulla();
                break;
            case Estado.Persecucion:
                EstadoPersecucionMejorada(); // Llamamos al nuevo método
                break;
        }
    }

    // --- LÓGICA DE PERSECUCIÓN MEJORADA ---
    void EstadoPersecucionMejorada()
    {
        if (agente == null) return;

        agente.isStopped = false;
        agente.stoppingDistance = distanciaParada; // Evita que se te encime

        // 1. Actualizamos la ruta solo cada 0.1 segundos (hace los movimientos más suaves)
        tiempoActualizacionRuta += Time.deltaTime;
        if (tiempoActualizacionRuta >= 0.1f)
        {
            // 2. Anticipación: Apuntamos un poco más adelante de la posición del jugador
            Vector3 posicionObjetivo = jugador.position + (jugador.forward * 0.5f);
            agente.SetDestination(posicionObjetivo);

            tiempoActualizacionRuta = 0f;
        }

        // 3. Sistema Anti-atascos: Si el agente debería moverse pero está quieto, recalculamos
        if (agente.velocity.magnitude < 0.1f && !agente.pathPending && agente.hasPath)
        {
            agente.ResetPath();
            agente.SetDestination(jugador.position);
        }
    }

    private void ReproducirSonidosDeDeteccion()
    {
        if (audioSourceJugador != null && sonidoAlertaJugador != null)
            audioSourceJugador.PlayOneShot(sonidoAlertaJugador);
        if (audioSourceAsesina != null && sonidoAsesina != null)
            audioSourceAsesina.PlayOneShot(sonidoAsesina);
    }

    private void GirarHaciaElJugadorInstantaneamente()
    {
        if (agente != null)
        {
            agente.updateRotation = false;
            Vector3 direccion = (jugador.position - transform.position).normalized;
            direccion.y = 0;
            if (direccion != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direccion);
            agente.updateRotation = true;
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
            return hit.collider.CompareTag("Player");
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

    public bool EstaPersiguiendo => estadoActual == Estado.Persecucion;

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