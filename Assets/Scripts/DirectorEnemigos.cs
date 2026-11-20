using UnityEngine;
using System.Collections;

public class DirectorEnemigos : MonoBehaviour
{
    [Header("Tareas necesarias (Opcional, dejar vacío si no se requiere)")]
    public TareaBase tareaCerrarCine;
    public TareaBase tareaApagarProyectores;

    [Header("Enemigos (Asignar Controladores)")]
    public ControlDino controlDino;
    public ControlAsesina controlAsesina;
    // public ControlEnemigo3 controlEnemigo3;

    [Header("Niveles de IA (Porcentaje 0% al 100%)")]
    [Range(0, 100)] public int probabilidadDino = 0;
    [Range(0, 100)] public int probabilidadAsesina = 0;

    [Header("Tiempos del Director")]
    public float tiempoGraciaInicial = 10f; // Tiempo antes de empezar a tirar dados
    public float intervaloDados = 8f;       // Cada cuántos segundos se lanza el dado
    public int maxEnemigosActivos = 2;       // Límite de enemigos al mismo tiempo

    [Header("Cooldowns (Tiempo de espera tras irse)")]
    public float cooldownDino = 30f;
    public float cooldownAsesina = 40f;

    // Variables internas
    private bool juegoActivo = false;
    private int enemigosActivos = 0;

    // Temporizadores de cooldown
    private float tiempoCooldownDino = 0f;
    private float tiempoCooldownAsesina = 0f;

    // Estados de los enemigos
    private bool dinoEnJuego = false;
    private bool asesinaEnJuego = false;

    void Start()
    {
        // Suscribirse a los eventos de los enemigos para saber cuándo se van
        if (controlDino != null) controlDino.OnDinoRetirado += DinoRetirado;
        if (controlAsesina != null) controlAsesina.OnAsesinaRetirada += AsesinaRetirada;

        StartCoroutine(RutinaDirector());
    }

    void Update()
    {
        // Revisar si las tareas están completas para encender el Director
        if (!juegoActivo)
        {
            // Si no se asignó ninguna tarea, el director se activa de inmediato
            if (tareaCerrarCine == null && tareaApagarProyectores == null)
            {
                juegoActivo = true;
                Debug.Log("Director activado (sin tareas previas asignadas).");
            }
            // Si hay tareas asignadas, comprobar que estén completas
            else if (tareaCerrarCine != null && tareaApagarProyectores != null)
            {
                if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
                {
                    juegoActivo = true;
                    Debug.Log("Director activado (tareas iniciales completadas).");
                }
            }
            // Si solo se asignó una tarea, comprobar solo esa
            else if (tareaCerrarCine != null && tareaCerrarCine.EstaCompletada)
            {
                juegoActivo = true;
            }
            else if (tareaApagarProyectores != null && tareaApagarProyectores.EstaCompletada)
            {
                juegoActivo = true;
            }
        }

        // Actualizar Cooldowns
        if (tiempoCooldownDino > 0) tiempoCooldownDino -= Time.deltaTime;
        if (tiempoCooldownAsesina > 0) tiempoCooldownAsesina -= Time.deltaTime;
    }

    IEnumerator RutinaDirector()
    {
        // Esperar a que el director se active
        while (!juegoActivo) yield return new WaitForSeconds(1f);

        // Tiempo de gracia inicial para que el jugador explore
        yield return new WaitForSeconds(tiempoGraciaInicial);

        while (true)
        {
            yield return new WaitForSeconds(intervaloDados);

            if (enemigosActivos >= maxEnemigosActivos) continue; // Límite alcanzado

            // --- TIRADA PARA EL DINO (0 a 100) ---
            if (!dinoEnJuego && tiempoCooldownDino <= 0f)
            {
                int dado = Random.Range(0, 100); // Genera un número del 0 al 99
                if (dado < probabilidadDino)     // Si el dado es menor al %, aparece
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

    public void ReiniciarNoche()
    {
        StopAllCoroutines();
        juegoActivo = false;
        enemigosActivos = 0;
        dinoEnJuego = false;
        asesinaEnJuego = false;
        tiempoCooldownDino = 0f;
        tiempoCooldownAsesina = 0f;

        if (controlDino != null) controlDino.gameObject.SetActive(false);
        if (controlAsesina != null) controlAsesina.DesaparecerAsesina();

        StartCoroutine(RutinaDirector());
        Debug.Log("Noche reiniciada.");
    }
}