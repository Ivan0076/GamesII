using UnityEngine;
using UnityEngine.AI;

public class AsesinaCerebro : MonoBehaviour
{
    // ---- Referencias ----
    [Header("Referencias")]
    public Transform jugador;
    public NavMeshAgent agente;
    public Transform[] puntosPatrulla;

    [Header("Detección por visión (Raycast)")]
    public float radioVision = 6f;
    public float alturaOjos = 1.2f;
    public LayerMask mascaraDeteccion;  // Capas que BLOQUEAN la visión (paredes, suelos, etc.)

    [Header("Comportamiento de Patrulla")]
    public float tiempoEsperaPatrulla = 2f;

    [Header("Memoria del enemigo")]
    public float tiempoAntesDeOlvidar = 2f; // Tiempo que recuerda al jugador tras perderlo de vista

    // ---- Estado actual ----
    private enum Estado { Patrulla, Persecucion }
    private Estado estadoActual = Estado.Patrulla;

    // ---- Variables internas ----
    private int indicePatrulla = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private bool jugadorVisible = false;
    private float temporizadorPerdida = 0f;  // cuenta el tiempo sin ver al jugador

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (jugador == null) jugador = GameObject.FindGameObjectWithTag("Player").transform;

        if (puntosPatrulla.Length > 0)
        {
            agente.SetDestination(puntosPatrulla[0].position);
        }
    }

    void Update()
    {
        // ---- 1. Detectar si el jugador está visible (RAYCAST) ----
        jugadorVisible = IsPlayerVisible();

        // ---- 2. Transiciones de estado CON MEMORIA ----
        if (estadoActual == Estado.Patrulla && jugadorVisible)
        {
            estadoActual = Estado.Persecucion;
            temporizadorPerdida = 0f; // Reiniciamos el contador al verlo
            Debug.Log("¡Jugador avistado! Persiguiendo.");
        }
        else if (estadoActual == Estado.Persecucion && !jugadorVisible)
        {
            // El jugador no está visible por lo que empezamos a contar el tiempo de "olvido"
            temporizadorPerdida += Time.deltaTime;

            if (temporizadorPerdida >= tiempoAntesDeOlvidar)
            {
                // Pasó el tiempo suficiente por lo que volvemos a patrullar
                estadoActual = Estado.Patrulla;
                Debug.Log("Olvidé al jugador. Vuelvo a patrullar.");

                // Reiniciamos la patrulla en el siguiente punto
                if (puntosPatrulla.Length > 0)
                {
                    indicePatrulla = (indicePatrulla + 1) % puntosPatrulla.Length;
                    agente.SetDestination(puntosPatrulla[indicePatrulla].position);
                }
                esperando = false;
                temporizadorEspera = 0f;
                temporizadorPerdida = 0f; // Reiniciamos el contador para la próxima
            }
        }
        else
        {
            // Si el jugador es visible o estamos en patrulla, reiniciamos el contador de pérdida
            // (esto evita que se acumule tiempo mientras no debería)
            if (jugadorVisible)
                temporizadorPerdida = 0f;
        }

        // 3. Ejecutar el estado actual 
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

    // -------------------- MÉTODO DE RAYCAST (VISIÓN) --------------------
    bool IsPlayerVisible()
    {
        if (jugador == null) return false;

        // Punto de origen: desde los "ojos" del enemigo
        Vector3 origen = transform.position + Vector3.up * alturaOjos;
        // Punto al que apunta: centro del jugador (a la altura del pecho)
        Vector3 objetivo = jugador.position + Vector3.up * 1f;
        Vector3 direccion = objetivo - origen;
        float distancia = direccion.magnitude;

        // Si está más lejos de lo que alcanza a ver, no es visible
        if (distancia > radioVision) return false;

        // Lanzamos el rayo
        RaycastHit hit;
        if (Physics.Raycast(origen, direccion, out hit, radioVision, mascaraDeteccion))
        {
            // Si el rayo impacta contra el jugador, está visible
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                // Impactó contra una pared u obstáculo → no visible
                return false;
            }
        }

        // Si no impactó con nada (caso muy raro), no visible
        return false;
    }

    // -------------------- ESTADO PATRULLA --------------------
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

    // -------------------- ESTADO PERSECUCIÓN --------------------
    void EstadoPersecucion()
    {
        agente.SetDestination(jugador.position);
        if (agente.isStopped) agente.isStopped = false;
    }

    // -------------------- VISUALIZACIÓN EN EDITOR --------------------
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