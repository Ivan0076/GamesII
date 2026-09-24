using UnityEngine;
using UnityEngine.AI;

public class DinoFaseDos : MonoBehaviour
{
    [Header("Spawn aleatorio")]
    public Transform[] puntosSpawn;

    [Header("Movimiento")]
    public float velocidad = 3.5f;
    public float distanciaGameOver = 2f;

    [Header("Detección de mirada")]
    [Range(0.7f, 1f)] public float umbralMirada = 0.9f;
    public LayerMask capasObstaculo;   // ← DEBE incluir capa del dino + paredes

    [Header("Animación")]
    public string triggerIniciarFase2 = "IniciarFase2";   // Trigger exacto del Animator

    [Header("Game Over")]
    public GameObject panelGameOver;

    [Header("Referencias")]
    public Transform jugador;
    public Camera camaraJugador;

    [Header("Debug")]
    public bool mostrarLogs = true;

    private NavMeshAgent agente;
    private Animator animator;
    private bool activo = false;
    private bool mirando = false;

    void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (jugador == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null) jugador = go.transform;
        }

        if (camaraJugador == null && jugador != null)
            camaraJugador = jugador.GetComponentInChildren<Camera>(true);
        if (camaraJugador == null)
            camaraJugador = Camera.main;

        if (mostrarLogs)
            Debug.Log($"Fase2 → agente={(agente != null)}, anim={(animator != null)}, " +
                      $"jugador={(jugador != null)}, cam={(camaraJugador != null)}");
    }

    public void IniciarFase()
    {
        activo = true;

        if (agente == null || jugador == null || camaraJugador == null)
        {
            Debug.LogError("Faltan referencias en DinoFaseDos.");
            return;
        }

        // Spawn aleatorio
        Vector3 pos = transform.position;
        if (puntosSpawn != null && puntosSpawn.Length > 0)
            pos = puntosSpawn[Random.Range(0, puntosSpawn.Length)].position;

        agente.enabled = true;
        agente.speed = velocidad;
        agente.isStopped = false;

        // Colocar sobre NavMesh
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, 20f, NavMesh.AllAreas))
        {
            agente.Warp(hit.position);
            if (mostrarLogs) Debug.Log($"Spawn en {hit.position}");
        }
        else
        {
            Debug.LogError($"No hay NavMesh cerca de {pos}. Rebake.");
            activo = false;
            return;
        }

        // ANIMACIÓN: activar trigger "IniciarFase2" (así se cambia al estado Caminar)
        if (animator != null && !string.IsNullOrEmpty(triggerIniciarFase2))
        {
            animator.SetTrigger(triggerIniciarFase2);
            animator.speed = 1f;
            if (mostrarLogs)
                Debug.Log($"Trigger '{triggerIniciarFase2}' activado.");
        }
        else if (animator != null)
        {
            // Fallback: si no hay trigger, intentar Play directo
            animator.Play("Caminar", 0, 0f);
            animator.speed = 1f;
            if (mostrarLogs) Debug.Log("Play('Caminar') llamado como fallback.");
        }

        if (panelGameOver != null) panelGameOver.SetActive(false);

        if (mostrarLogs) Debug.Log("Fase 2 iniciada.");
    }

    void Update()
    {
        if (!activo) return;
        if (agente == null || jugador == null || camaraJugador == null) return;
        if (!agente.enabled || !agente.isOnNavMesh) return;

        // 1. ¿El jugador lo mira?
        bool mirandoAhora = EstaSiendoMirado();

        if (mirandoAhora != mirando && mostrarLogs)
            Debug.Log(mirandoAhora ? "Mirando → pausa" : " No mira → persigue");

        mirando = mirandoAhora;

        // 2. Pausar o perseguir
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

            if (mostrarLogs && Time.frameCount % 90 == 0)
            {
                Debug.Log($"dist={Vector3.Distance(transform.position, jugador.position):F2} " +
                          $"vel={agente.velocity.magnitude:F2} " +
                          $"pathPending={agente.pathPending}");
            }
        }

        // 3. Game Over
        if (Vector3.Distance(transform.position, jugador.position) <= distanciaGameOver)
            GameOver();
    }

    // ====================================================================
    // DETECCIÓN DE MIRADA (con verificación completa)
    // ====================================================================

    private bool EstaSiendoMirado()
    {
        Vector3 posCam = camaraJugador.transform.position;
        Vector3 posDino = transform.position + Vector3.up * 1.5f;
        Vector3 dir = (posDino - posCam).normalized;
        float dist = Vector3.Distance(posCam, posDino);

        // 1. ¿Está dentro del cono?
        float dot = Vector3.Dot(camaraJugador.transform.forward, dir);
        if (dot < umbralMirada)
            return false;

        // 2. ¿Hay algo entre la cámara y el dino?
        if (Physics.Raycast(posCam, dir, out RaycastHit hit, dist,
                            capasObstaculo, QueryTriggerInteraction.Ignore))
        {
            bool esElDino = hit.collider.gameObject == gameObject ||
                            hit.collider.transform.IsChildOf(transform);

            if (mostrarLogs && Time.frameCount % 30 == 0)
                Debug.Log($"Raycast: '{hit.collider.gameObject.name}' " +
                          $"(capa: {LayerMask.LayerToName(hit.collider.gameObject.layer)}) " +
                          $"esDino={esElDino}");

            // Si golpeó algo que NO es el dino → bloqueado por pared/obstáculo
            if (!esElDino)
                return false;
        }
        else
        {
            // El raycast NO golpeó nada:
            // Significa que ni el dino ni las paredes están en capasObstaculo
            if (mostrarLogs && Time.frameCount % 120 == 0)
                Debug.LogWarning("Raycast sin impacto. Añade la capa del dino Y de las paredes a capasObstaculo.");
        }

        return true;
    }

    private void GameOver()
    {
        if (!activo) return;
        activo = false;

        Debug.Log("GAME OVER");
        if (agente.enabled && agente.isOnNavMesh) agente.isStopped = true;
        if (animator != null) animator.speed = 0f;
        if (panelGameOver != null) panelGameOver.SetActive(true);
    }
}