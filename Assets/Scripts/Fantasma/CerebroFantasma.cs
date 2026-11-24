using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum EstadoFantasma { Inactivo, EsperandoRobo, Jumpscare, Manos }

public class CerebroFantasma : MonoBehaviour
{
    [Header("Fantasma")]
    public GameObject objetoFantasma;
    public Animator animatorFantasma;

    [Header("Conexión con Tarea")]
    public RecogerObjetoPerdidos tareaObjetos;

    [Header("Probabilidad de Robo")]
    [Range(0, 100)] public int probabilidadRobar = 50;

    [Header("Sistema de Pánico")]
    public SistemaPanico sistemaPanico;
    public float panicoQueAumenta = 25f;

    [Header("Manos")]
    public GameObject manoIzquierda;
    public GameObject manoDerecha;
    public float tiempoManosActivas = 4f;
    public float tiempoManosInactivas = 6f;
    public int ciclosManosMaximos = 2;

    [Header("Tiempos")]
    public float tiempoEsperaAntesDeRobar = 4f;
    public float duracionJumpscare = 1.5f;

    [Header("Jumpscare")]
    public Transform jugador;
    public float distanciaJumpscare = 1.5f;
    public float ajusteAlturaJumpscare = -0.2f;
    public AudioSource audioSourceJumpscare;
    public AudioClip sonidoJumpscare;

    public System.Action OnFantasmaTermino;

    private float temporizador;
    private bool manosActivas = false;
    private bool cicloManosIniciado = false;
    private int ciclosCompletados = 0;
    private EstadoFantasma estadoActual = EstadoFantasma.Inactivo;
    private bool debeRobarAlTerminarJumpscare = false;

    void Awake()
    {
        DetenerYLimpiar();
    }

    public void IniciarCiclo()
    {
        Debug.Log("CerebroFantasma: IniciarCiclo() llamado.");
        DetenerYLimpiar();
        temporizador = tiempoEsperaAntesDeRobar;
        estadoActual = EstadoFantasma.EsperandoRobo;

        if (jugador == null && Camera.main != null)
            jugador = Camera.main.transform;

        if (jugador == null)
            Debug.LogError("❌ CerebroFantasma: No se encontró al 'jugador' (Camera.main es null y no hay asignación manual).");

        Debug.Log($"CerebroFantasma: Estado cambiado a EsperandoRobo. Temporizador iniciado en {temporizador} segundos.");
    }

    public void DetenerYLimpiar()
    {
        CancelInvoke();
        estadoActual = EstadoFantasma.Inactivo;
        manosActivas = false;
        cicloManosIniciado = false;
        ciclosCompletados = 0;
        debeRobarAlTerminarJumpscare = false;

        if (objetoFantasma != null) objetoFantasma.SetActive(false);
        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);
    }

    void Update()
    {
        if (Time.frameCount % 60 == 0)
            Debug.Log($"CerebroFantasma Update corriendo. Estado: {estadoActual}, Tiempo restante: {temporizador:F1}");

        if (estadoActual == EstadoFantasma.EsperandoRobo)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0)
            {
                Debug.Log("CerebroFantasma: Temporizador terminado. Evaluando robo y saltando...");
                DeterminarSiRobara();
                EjecutarJumpscare();
            }
        }
        else if (estadoActual == EstadoFantasma.Manos)
        {
            if (!cicloManosIniciado) return;

            temporizador -= Time.deltaTime;

            if (temporizador <= 0)
            {
                if (manosActivas)
                {
                    DesactivarManos();
                    ciclosCompletados++;

                    if (ciclosCompletados >= ciclosManosMaximos)
                    {
                        DetenerYLimpiar();
                        OnFantasmaTermino?.Invoke();
                        return;
                    }
                    temporizador = tiempoManosInactivas;
                }
                else
                {
                    ActivarManos();
                    temporizador = tiempoManosActivas;
                }
            }
        }
    }

    void EjecutarJumpscare()
    {
        Debug.Log("CerebroFantasma: ¡Ejecutando Jumpscare!");
        estadoActual = EstadoFantasma.Jumpscare;

        if (sistemaPanico != null)
        {
            sistemaPanico.AumentarPanico(panicoQueAumenta);
        }

        if (jugador != null && objetoFantasma != null)
        {
            Vector3 frenteHorizontal = Vector3.ProjectOnPlane(jugador.forward, Vector3.up).normalized;
            if (frenteHorizontal == Vector3.zero) frenteHorizontal = jugador.forward;

            Vector3 posicionJumpscare = jugador.position + frenteHorizontal * distanciaJumpscare + Vector3.up * ajusteAlturaJumpscare;

            objetoFantasma.transform.position = posicionJumpscare;
            objetoFantasma.transform.LookAt(new Vector3(jugador.position.x, objetoFantasma.transform.position.y, jugador.position.z));

            objetoFantasma.SetActive(true);
            Debug.Log("🧠 CerebroFantasma: Modelo del fantasma activado frente al jugador.");
        }

        if (animatorFantasma != null) animatorFantasma.SetTrigger("Jumpscare");
        if (audioSourceJumpscare != null && sonidoJumpscare != null) audioSourceJumpscare.PlayOneShot(sonidoJumpscare);

        Invoke(nameof(OcultarFantasmaYEmpezarManos), duracionJumpscare);
    }

    void OcultarFantasmaYEmpezarManos()
    {
        if (objetoFantasma != null) objetoFantasma.SetActive(false);

        if (debeRobarAlTerminarJumpscare)
        {
            EjecutarRobo();
        }

        estadoActual = EstadoFantasma.Manos;
        cicloManosIniciado = false;
        ciclosCompletados = 0;
        ActivarManos();
    }

    void DeterminarSiRobara()
    {
        debeRobarAlTerminarJumpscare = false;

        if (tareaObjetos == null)
        {
            return;
        }
        if (tareaObjetos.objetosRecuperados.Count == 0)
        {
            return;
        }

        int dado = Random.Range(0, 100);
        if (dado < probabilidadRobar)
        {
            debeRobarAlTerminarJumpscare = true;
        }
    }

    void EjecutarRobo()
    {
        if (tareaObjetos == null || tareaObjetos.objetosRecuperados.Count == 0) return;

        int indiceAleatorio = Random.Range(0, tareaObjetos.objetosRecuperados.Count);
        GameObject objetoA_Devolver = tareaObjetos.objetosRecuperados[indiceAleatorio];

        tareaObjetos.objetosRecuperados.RemoveAt(indiceAleatorio);
        tareaObjetos.objetos.Add(objetoA_Devolver);

        XRGrabInteractable grab = objetoA_Devolver.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

        Rigidbody rb = objetoA_Devolver.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        ObjetoRecogible scriptObjeto = objetoA_Devolver.GetComponent<ObjetoRecogible>();
        if (scriptObjeto != null) scriptObjeto.DevolverAlOrigen();

        tareaObjetos.ActualizarProgresoExterno();

        Debug.Log($"🧠 CerebroFantasma: ¡El fantasma ha robado '{objetoA_Devolver.name}'!");
    }

    void ActivarManos()
    {
        if (manoIzquierda != null) manoIzquierda.SetActive(true);
        if (manoDerecha != null) manoDerecha.SetActive(true);
        manosActivas = true;
        cicloManosIniciado = true;
        temporizador = tiempoManosActivas;
        Debug.Log("🧠 CerebroFantasma: Manos activadas.");
    }

    void DesactivarManos()
    {
        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);
        manosActivas = false;
        Debug.Log("🧠 CerebroFantasma: Manos desactivadas.");
    }
}