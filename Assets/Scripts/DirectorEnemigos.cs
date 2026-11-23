using UnityEngine;
using System.Collections;

public class DirectorEnemigos : MonoBehaviour
{
    [Header("Tareas necesarias (Opcional, dejar vacío si no se requiere)")]
    public TareaBase tareaCerrarCine;
    public TareaBase tareaApagarProyectores;

    [Header("Tarea Objetos Perdidos (Para el Fantasma)")]
    public RecogerObjetoPerdidos tareaObjetosPerdidos;
    public float delayDadoFantasmaTrasEntrega = 5f; // Segundos tras entregar objeto antes de empezar a tirar dado

    [Header("Enemigos (Asignar Controladores)")]
    public ControlDino controlDino;
    public ControlAsesina controlAsesina;
    public ControlFantasma controlFantasma;

    [Header("Niveles de IA (Porcentaje 0% al 100%)")]
    [Range(0, 100)] public int probabilidadDino = 0;
    [Range(0, 100)] public int probabilidadAsesina = 0;
    [Range(0, 100)] public int probabilidadFantasma = 0;

    [Header("Tiempos del Director")]
    public float tiempoGraciaInicial = 10f; // Tiempo antes de empezar a tirar dados
    public float intervaloDados = 8f;       // Cada cuántos segundos se lanza el dado
    public int maxEnemigosActivos = 2;      // Límite de enemigos al mismo tiempo

    [Header("Cooldowns (Tiempo de espera tras irse)")]
    public float cooldownDino = 30f;
    public float cooldownAsesina = 40f;
    public float cooldownFantasma = 35f;

    // Variables internas
    private bool juegoActivo = false;
    private int enemigosActivos = 0;

    // Temporizadores de cooldown
    private float tiempoCooldownDino = 0f;
    private float tiempoCooldownAsesina = 0f;
    private float tiempoCooldownFantasma = 0f;

    // Control de entrega para el Fantasma
    private bool fantasmaHabilitadoPorEntrega = false;
    private float tiempoEsperaEntregaFantasma = 0f;

    // Estados de los enemigos
    private bool dinoEnJuego = false;
    private bool asesinaEnJuego = false;
    private bool fantasmaEnJuego = false;

    void Start()
    {
        // Suscribirse a los eventos de los enemigos para saber cuándo se van
        if (controlDino != null) controlDino.OnDinoRetirado += DinoRetirado;
        if (controlAsesina != null) controlAsesina.OnAsesinaRetirada += AsesinaRetirada;
        if (controlFantasma != null) controlFantasma.OnFantasmaRetirado += FantasmaRetirado;

        // Suscribirse a la entrega de objetos perdidos
        if (tareaObjetosPerdidos != null)
            tareaObjetosPerdidos.OnObjetoEntregado += AlEntregarObjetoPerdido;

        StartCoroutine(RutinaDirector());
    }

    void AlEntregarObjetoPerdido()
    {
        fantasmaHabilitadoPorEntrega = true;
        tiempoEsperaEntregaFantasma = delayDadoFantasmaTrasEntrega;
        Debug.Log($"Objeto entregado. Tirada de dado del Fantasma habilitada en {delayDadoFantasmaTrasEntrega} segundos.");
    }

    void Update()
    {
        // Revisar si las tareas están completas para encender el Director
        if (!juegoActivo)
        {
            if (tareaCerrarCine == null && tareaApagarProyectores == null)
            {
                juegoActivo = true;
                Debug.Log("Director activado (sin tareas previas asignadas).");
            }
            else if (tareaCerrarCine != null && tareaApagarProyectores != null)
            {
                if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
                {
                    juegoActivo = true;
                    Debug.Log("Director activado (tareas iniciales completadas).");
                }
            }
            else if (tareaCerrarCine != null && tareaCerrarCine.EstaCompletada)
            {
                juegoActivo = true;
            }
            else if (tareaApagarProyectores != null && tareaApagarProyectores.EstaCompletada)
            {
                juegoActivo = true;
            }
        }

        // Actualizar Cooldowns y espera post-entrega
        if (tiempoCooldownDino > 0) tiempoCooldownDino -= Time.deltaTime;
        if (tiempoCooldownAsesina > 0) tiempoCooldownAsesina -= Time.deltaTime;
        if (tiempoCooldownFantasma > 0) tiempoCooldownFantasma -= Time.deltaTime;
        if (tiempoEsperaEntregaFantasma > 0) tiempoEsperaEntregaFantasma -= Time.deltaTime;
    }

    IEnumerator RutinaDirector()
    {
        while (!juegoActivo) yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(tiempoGraciaInicial);

        while (true)
        {
            yield return new WaitForSeconds(intervaloDados);

            if (enemigosActivos >= maxEnemigosActivos) continue;

            // --- TIRADA PARA EL DINO (0 a 100) ---
            if (!dinoEnJuego && tiempoCooldownDino <= 0f)
            {
                int dado = Random.Range(0, 100);
                if (dado < probabilidadDino)
                {
                    ActivarDino();
                }
            }

            // --- TIRADA PARA LA ASESINA (0 a 100) ---
            if (!asesinaEnJuego && tiempoCooldownAsesina <= 0f && enemigosActivos < maxEnemigosActivos)
            {
                int dado = Random.Range(0, 100);
                if (dado < probabilidadAsesina)
                {
                    ActivarAsesina();
                }
            }

            // --- TIRADA PARA EL FANTASMA (0 a 100) ---
            bool hayObjetosParaRobar = tareaObjetosPerdidos != null && tareaObjetosPerdidos.objetosRecuperados.Count > 0;

            if (!fantasmaEnJuego &&
                tiempoCooldownFantasma <= 0f &&
                enemigosActivos < maxEnemigosActivos &&
                fantasmaHabilitadoPorEntrega &&
                tiempoEsperaEntregaFantasma <= 0f &&
                hayObjetosParaRobar)
            {
                int dado = Random.Range(0, 100);
                if (dado < probabilidadFantasma)
                {
                    ActivarFantasma();
                }
            }
        }
    }

    void ActivarDino()
    {
        dinoEnJuego = true;
        enemigosActivos++;
        controlDino.Aparecer();
        Debug.Log($"¡{probabilidadDino}% de acierto para el Dino! ({enemigosActivos}/{maxEnemigosActivos} activos)");
    }

    void ActivarAsesina()
    {
        asesinaEnJuego = true;
        enemigosActivos++;
        controlAsesina.Aparecer();
        Debug.Log($"¡{probabilidadAsesina}% de acierto para la Asesina! ({enemigosActivos}/{maxEnemigosActivos} activos)");
    }

    void ActivarFantasma()
    {
        fantasmaEnJuego = true;
        enemigosActivos++;
        controlFantasma.Aparecer();
        Debug.Log($"¡{probabilidadFantasma}% de acierto para el Fantasma! ({enemigosActivos}/{maxEnemigosActivos} activos)");
    }

    void DinoRetirado()
    {
        dinoEnJuego = false;
        enemigosActivos--;
        tiempoCooldownDino = cooldownDino;
        Debug.Log($"El Dino entró en Cooldown por {cooldownDino} segundos.");
    }

    void AsesinaRetirada()
    {
        asesinaEnJuego = false;
        enemigosActivos--;
        tiempoCooldownAsesina = cooldownAsesina;
        Debug.Log($"La Asesina entró en Cooldown por {cooldownAsesina} segundos.");
    }

    void FantasmaRetirado()
    {
        fantasmaEnJuego = false;
        enemigosActivos--;
        tiempoCooldownFantasma = cooldownFantasma;
        Debug.Log($"El Fantasma entró en Cooldown por {cooldownFantasma} segundos.");
    }

    public void ReiniciarNoche()
    {
        StopAllCoroutines();
        juegoActivo = false;
        enemigosActivos = 0;
        dinoEnJuego = false;
        asesinaEnJuego = false;
        fantasmaEnJuego = false;
        tiempoCooldownDino = 0f;
        tiempoCooldownAsesina = 0f;
        tiempoCooldownFantasma = 0f;
        fantasmaHabilitadoPorEntrega = false;
        tiempoEsperaEntregaFantasma = 0f;

        if (controlDino != null) controlDino.gameObject.SetActive(false);
        if (controlAsesina != null) controlAsesina.DesaparecerAsesina();
        if (controlFantasma != null) controlFantasma.DesaparecerFantasma();

        StartCoroutine(RutinaDirector());
        Debug.Log("Noche reiniciada.");
    }
}