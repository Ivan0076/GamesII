using UnityEngine;
using System.Collections;

public class DirectorEnemigos : MonoBehaviour
{
    [Header("Tareas necesarias (Opcional)")]
    public TareaBase tareaCerrarCine;
    public TareaBase tareaApagarProyectores;

    [Header("Tarea Objetos Perdidos (Para el Fantasma)")]
    public RecogerObjetoPerdidos tareaObjetosPerdidos;

    [Header("Control de Enemigos en el Nivel")]
    public bool permitirDino = true;
    public bool permitirAsesina = true;
    public bool permitirFantasma = true;

    [Header("Enemigos (Asignar Controladores)")]
    public ControlDino controlDino;
    public ControlAsesina controlAsesina;
    public ControlFantasma controlFantasma;

    [Header("Probabilidades Base (0% al 100%)")]
    [Range(0, 100)] public int probabilidadBaseDino = 10;
    [Range(0, 100)] public int probabilidadBaseAsesina = 10;
    [Range(0, 100)] public int probabilidadBaseFantasma = 10;

    [Header("Sistema de Piedad")]
    public int incrementoPorFallo = 5;
    public int probabilidadMaxima = 80;

    [Header("Tiempos del Director")]
    public float tiempoGraciaInicial = 10f;
    public float intervaloDados = 8f;
    public int maxEnemigosActivos = 2;

    [Header("Cooldowns")]
    public float cooldownDino = 30f;
    public float cooldownAsesina = 40f;
    public float cooldownFantasma = 35f;

    private int probActualDino;
    private int probActualAsesina;
    private int probActualFantasma;

    private bool juegoActivo = false;
    private int enemigosActivos = 0;

    private float tiempoCooldownDino = 0f;
    private float tiempoCooldownAsesina = 0f;
    private float tiempoCooldownFantasma = 0f;

    private bool dinoEnJuego = false;
    private bool asesinaEnJuego = false;
    private bool fantasmaEnJuego = false;

    void Start()
    {
        Debug.Log("🟢 DirectorEnemigos: Script encendido. Esperando condiciones iniciales...");

        probActualDino = probabilidadBaseDino;
        probActualAsesina = probabilidadBaseAsesina;
        probActualFantasma = probabilidadBaseFantasma;

        if (controlDino != null) controlDino.OnDinoRetirado += DinoRetirado;
        if (controlAsesina != null) controlAsesina.OnAsesinaRetirada += AsesinaRetirada;
        if (controlFantasma != null) controlFantasma.OnFantasmaRetirado += FantasmaRetirado;

        StartCoroutine(RutinaDirector());
    }

    void Update()
    {
        if (!juegoActivo)
        {
            if (tareaCerrarCine == null && tareaApagarProyectores == null)
            {
                juegoActivo = true;
                Debug.Log("Director ACTIVADO: No hay tareas iniciales asignadas (Modo Prueba).");
            }
            else if (tareaCerrarCine != null && tareaApagarProyectores != null)
            {
                if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
                {
                    juegoActivo = true;
                    Debug.Log("Director ACTIVADO: Ambas tareas iniciales completadas.");
                }
            }
            else if (tareaCerrarCine != null && tareaCerrarCine.EstaCompletada)
            {
                juegoActivo = true;
                Debug.Log("Director ACTIVADO: Tarea Cerrar Cine completada.");
            }
            else if (tareaApagarProyectores != null && tareaApagarProyectores.EstaCompletada)
            {
                juegoActivo = true;
                Debug.Log("Director ACTIVADO: Tarea Apagar Proyectores completada.");
            }
        }

        if (tiempoCooldownDino > 0) tiempoCooldownDino -= Time.deltaTime;
        if (tiempoCooldownAsesina > 0) tiempoCooldownAsesina -= Time.deltaTime;
        if (tiempoCooldownFantasma > 0) tiempoCooldownFantasma -= Time.deltaTime;
    }

    IEnumerator RutinaDirector()
    {
        while (!juegoActivo) yield return new WaitForSeconds(1f);

        Debug.Log($"Director: Iniciando tiempo de gracia de {tiempoGraciaInicial} segundos...");
        yield return new WaitForSeconds(tiempoGraciaInicial);

        Debug.Log("Director: Tiempo de gracia terminado. Empezando a lanzar dados.");

        while (true)
        {
            yield return new WaitForSeconds(intervaloDados);

            if (enemigosActivos >= maxEnemigosActivos)
            {
                Debug.Log("Límite de enemigos alcanzado. Esperando...");
                continue;
            }

            // --- TIRADA PARA EL DINO ---
            if (permitirDino && !dinoEnJuego && tiempoCooldownDino <= 0f)
            {
                int dado = Random.Range(0, 100);
                if (dado < probActualDino) { ActivarDino(); probActualDino = probabilidadBaseDino; }
                else { probActualDino = Mathf.Clamp(probActualDino + incrementoPorFallo, probabilidadBaseDino, probabilidadMaxima); }
            }

            // --- TIRADA PARA LA ASESINA ---
            if (permitirAsesina && !asesinaEnJuego && tiempoCooldownAsesina <= 0f && enemigosActivos < maxEnemigosActivos)
            {
                int dado = Random.Range(0, 100);
                if (dado < probActualAsesina) { ActivarAsesina(); probActualAsesina = probabilidadBaseAsesina; }
                else { probActualAsesina = Mathf.Clamp(probActualAsesina + incrementoPorFallo, probabilidadBaseAsesina, probabilidadMaxima); }
            }

            // --- TIRADA PARA EL FANTASMA ---
            bool puedeAparecerFantasma = tareaObjetosPerdidos != null;

            if (permitirFantasma && !fantasmaEnJuego && tiempoCooldownFantasma <= 0f && enemigosActivos < maxEnemigosActivos && puedeAparecerFantasma)
            {
                int dado = Random.Range(0, 100);
                if (dado < probActualFantasma) { ActivarFantasma(); probActualFantasma = probabilidadBaseFantasma; }
                else { probActualFantasma = Mathf.Clamp(probActualFantasma + incrementoPorFallo, probabilidadBaseFantasma, probabilidadMaxima); }
            }
        }
    }

    void ActivarDino()
    {
        dinoEnJuego = true; enemigosActivos++;
        controlDino.Aparecer();
        Debug.Log($"¡El DINO apareció! (Probabilidad en esta tirada: {probActualDino}%). Enemigos activos: {enemigosActivos}");
    }

    void ActivarAsesina()
    {
        asesinaEnJuego = true; enemigosActivos++;
        controlAsesina.Aparecer();
        Debug.Log($"¡La ASESINA apareció! (Probabilidad en esta tirada: {probActualAsesina}%). Enemigos activos: {enemigosActivos}");
    }

    void ActivarFantasma()
    {
        fantasmaEnJuego = true; enemigosActivos++;
        controlFantasma.Aparecer();
        Debug.Log($"¡El FANTASMA apareció! (Probabilidad en esta tirada: {probActualFantasma}%). Enemigos activos: {enemigosActivos}");
    }

    void DinoRetirado() { dinoEnJuego = false; enemigosActivos--; tiempoCooldownDino = cooldownDino; Debug.Log("El Dino se fue. Cooldown iniciado."); }
    void AsesinaRetirada() { asesinaEnJuego = false; enemigosActivos--; tiempoCooldownAsesina = cooldownAsesina; Debug.Log("La Asesina se fue. Cooldown iniciado."); }
    void FantasmaRetirado() { fantasmaEnJuego = false; enemigosActivos--; tiempoCooldownFantasma = cooldownFantasma; Debug.Log("El Fantasma se fue. Cooldown iniciado."); }

    public void ReiniciarNoche()
    {
        StopAllCoroutines();
        juegoActivo = false; enemigosActivos = 0;
        dinoEnJuego = false; asesinaEnJuego = false; fantasmaEnJuego = false;
        tiempoCooldownDino = 0f; tiempoCooldownAsesina = 0f; tiempoCooldownFantasma = 0f;

        probActualDino = probabilidadBaseDino;
        probActualAsesina = probabilidadBaseAsesina;
        probActualFantasma = probabilidadBaseFantasma;

        if (controlDino != null) controlDino.gameObject.SetActive(false);
        if (controlAsesina != null) controlAsesina.DesaparecerAsesina();
        if (controlFantasma != null) controlFantasma.DesaparecerFantasma();

        StartCoroutine(RutinaDirector());
        Debug.Log("Director: Noche reiniciada manualmente.");
    }
}