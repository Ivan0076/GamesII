using UnityEngine;
using System.Collections;
public class DirectorEnemigos : MonoBehaviour
{
    [Header("Tareas necesarias para activar el caos")]
    public TareaBase tareaCerrarCine;
    public TareaBase tareaApagarProyectores;

    [Header("Enemigos (Asignar Controladores)")]
    public ControlDino controlDino;
    public ControlAsesina controlAsesina;
    // public ControlEnemigo3 controlEnemigo3; // Cuando lo tengas

    [Header("Niveles de IA (Dificultad 1 al 20)")]
    [Range(0, 20)] public int nivelIADino = 5;
    [Range(0, 20)] public int nivelIAAsesina = 0;

    [Header("Tiempos del Director")]
    public float intervaloDados = 5f;       // Cada cuántos segundos se lanza el dado
    public int maxEnemigosActivos = 2;      // Límite de enemigos al mismo tiempo

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
        if (!juegoActivo && tareaCerrarCine != null && tareaApagarProyectores != null)
        {
            if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
            {
                juegoActivo = true;
                Debug.Log("🎬 ¡Director activado! Las tareas iniciales están hechas.");
            }
        }

        // Actualizar Cooldowns
        if (tiempoCooldownDino > 0) tiempoCooldownDino -= Time.deltaTime;
        if (tiempoCooldownAsesina > 0) tiempoCooldownAsesina -= Time.deltaTime;
    }

    IEnumerator RutinaDirector()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervaloDados);

            if (!juegoActivo) continue; // No hacer nada si no se han hecho las tareas
            if (enemigosActivos >= maxEnemigosActivos) continue; // Límite alcanzado

            // --- TIRADA DE DADOS PARA EL DINO ---
            if (!dinoEnJuego && tiempoCooldownDino <= 0f)
            {
                int dado = Random.Range(1, 21); // Dado del 1 al 20
                if (dado <= nivelIADino)
                {
                    ActivarDino();
                }
            }

            // --- TIRADA DE DADOS PARA LA ASESINA ---
            if (!asesinaEnJuego && tiempoCooldownAsesina <= 0f && enemigosActivos < maxEnemigosActivos)
            {
                int dado = Random.Range(1, 21);
                if (dado <= nivelIAAsesina)
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
        Debug.Log($"🎲 ¡Dado exitoso para el Dino! ({enemigosActivos}/{maxEnemigosActivos} activos)");
    }

    void ActivarAsesina()
    {
        asesinaEnJuego = true;
        enemigosActivos++;
        controlAsesina.Aparecer();
        Debug.Log($"🎲 ¡Dado exitoso para la Asesina! ({enemigosActivos}/{maxEnemigosActivos} activos)");
    }

    void DinoRetirado()
    {
        dinoEnJuego = false;
        enemigosActivos--;
        tiempoCooldownDino = cooldownDino;
        Debug.Log($"⏳ El Dino entró en Cooldown por {cooldownDino} segundos.");
    }

    void AsesinaRetirada()
    {
        asesinaEnJuego = false;
        enemigosActivos--;
        tiempoCooldownAsesina = cooldownAsesina;
        Debug.Log($"⏳ La Asesina entró en Cooldown por {cooldownAsesina} segundos.");
    }

    // Método para reiniciar todo (llámalo cuando el jugador muera o reinicie la noche)
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
