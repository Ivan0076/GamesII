using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum EstadoFantasma
{
    Inactivo,
    Ausente,
    EsperandoJumpscare,
    Jumpscare,
    Manos
}

public class CerebroFantasma : MonoBehaviour
{
    [Header("Fantasma")]
    public GameObject objetoFantasma;
    public Animator animatorFantasma;

    [Header("Conexión con Tarea")]
    public RecogerObjetoPerdidos tareaObjetos;

    [Header("Manos")]
    public GameObject manoIzquierda;
    public GameObject manoDerecha;
    public float tiempoManosActivas = 4f;
    public float tiempoManosInactivas = 6f;
    public int ciclosManosMaximos = 2;

    [Header("Tiempos")]
    public float tiempoAusente = 5f;
    public float tiempoDelayJumpscare = 4f;
    public float tiempoDelayManos = 2f;

    [Header("Jumpscare")]
    public Transform jugador; // Asigna aquí la Main Camera del jugador (VR)
    public float distanciaJumpscare = 2f; // Más cerca para que no quede detrás de paredes
    public float ajusteAlturaJumpscare = -0.3f; // Ajuste vertical respecto a los ojos
    public AudioSource audioSourceJumpscare;
    public AudioClip sonidoJumpscare;

    // Evento para avisar a ControlFantasma que terminó toda su secuencia
    public System.Action OnFantasmaTermino;

    private float temporizador;
    private bool manosActivas = false;
    private bool cicloManosIniciado = false;
    private int ciclosCompletados = 0;
    private EstadoFantasma estadoActual = EstadoFantasma.Inactivo;

    void Awake()
    {
        DetenerYLimpiar();
    }

    public void IniciarCiclo()
    {
        DetenerYLimpiar();
        temporizador = tiempoAusente;
        estadoActual = EstadoFantasma.Ausente;
        Debug.Log("CerebroFantasma: Ciclo iniciado en estado Ausente.");
    }

    public void DetenerYLimpiar()
    {
        CancelInvoke();
        estadoActual = EstadoFantasma.Inactivo;
        manosActivas = false;
        cicloManosIniciado = false;
        ciclosCompletados = 0;

        if (objetoFantasma != null)
            objetoFantasma.SetActive(false);

        if (manoIzquierda != null)
            manoIzquierda.SetActive(false);

        if (manoDerecha != null)
            manoDerecha.SetActive(false);
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoFantasma.Inactivo:
                break;

            case EstadoFantasma.Ausente:

                if (tareaObjetos != null && tareaObjetos.objetosRecuperados.Count > 0)
                {
                    temporizador -= Time.deltaTime;

                    if (temporizador <= 0)
                    {
                        DevolverUnObjetoAlAzar();

                        temporizador = tiempoDelayJumpscare;
                        estadoActual = EstadoFantasma.EsperandoJumpscare;
                    }
                }

                break;

            case EstadoFantasma.EsperandoJumpscare:

                temporizador -= Time.deltaTime;

                if (temporizador <= 0)
                {
                    estadoActual = EstadoFantasma.Jumpscare;
                }

                break;

            case EstadoFantasma.Jumpscare:

                AparecerFrenteJugador();

                if (animatorFantasma != null)
                {
                    animatorFantasma.SetTrigger("Jumpscare");
                }

                if (audioSourceJumpscare != null && sonidoJumpscare != null)
                {
                    audioSourceJumpscare.PlayOneShot(sonidoJumpscare);
                }

                Invoke(nameof(MoverFantasma), 1.5f);
                Invoke(nameof(ActivarManos), tiempoDelayManos);

                cicloManosIniciado = false;
                ciclosCompletados = 0;
                estadoActual = EstadoFantasma.Manos;

                break;

            case EstadoFantasma.Manos:

                if (!cicloManosIniciado) break;

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
                            Debug.Log("El fantasma terminó su ataque y se retiró.");
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

                break;
        }
    }

    public void DevolverUnObjetoAlAzar()
    {
        if (tareaObjetos == null)
        {
            Debug.LogError("CerebroFantasma: No está asignada la referencia 'tareaObjetos' en el Inspector.");
            return;
        }

        if (tareaObjetos.objetosRecuperados.Count == 0)
        {
            Debug.Log("El fantasma intentó devolver un objeto, pero la lista está vacía.");
            return;
        }

        int indiceAleatorio = Random.Range(0, tareaObjetos.objetosRecuperados.Count);
        GameObject objetoA_Devolver = tareaObjetos.objetosRecuperados[indiceAleatorio];

        // 1. Lo sacamos de recuperados y lo regresamos a pendientes ANTES de moverlo
        tareaObjetos.objetosRecuperados.RemoveAt(indiceAleatorio);
        tareaObjetos.objetos.Add(objetoA_Devolver);

        // 2. Reactivamos el agarre VR que ZonaRecoleccion había apagado
        XRGrabInteractable grab = objetoA_Devolver.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

        // 3. Restauramos la física por si estaba congelado
        Rigidbody rb = objetoA_Devolver.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 4. Lo regresamos a su punto inicial (Punto X)
        ObjetoRecogible scriptObjeto = objetoA_Devolver.GetComponent<ObjetoRecogible>();
        if (scriptObjeto != null)
        {
            scriptObjeto.DevolverAlOrigen();
        }

        // 5. Actualizamos el progreso de la tarea (portapapeles)
        tareaObjetos.ActualizarProgresoExterno();

        Debug.Log($"¡El fantasma devolvió el objeto '{objetoA_Devolver.name}' a su lugar!");
    }

    void AparecerFrenteJugador()
    {
        if (jugador == null)
        {
            if (Camera.main != null) jugador = Camera.main.transform;
            else
            {
                Debug.LogError("CerebroFantasma: No hay 'jugador' asignado para el Jumpscare.");
                return;
            }
        }

        if (objetoFantasma == null)
        {
            Debug.LogError("CerebroFantasma: No está asignado 'objetoFantasma' en el Inspector.");
            return;
        }

        Vector3 frenteHorizontal = Vector3.ProjectOnPlane(jugador.forward, Vector3.up).normalized;
        if (frenteHorizontal == Vector3.zero) frenteHorizontal = jugador.forward;

        Vector3 posicionJumpscare =
            jugador.position +
            frenteHorizontal * distanciaJumpscare +
            Vector3.up * ajusteAlturaJumpscare;

        objetoFantasma.transform.position = posicionJumpscare;

        objetoFantasma.transform.LookAt(
            new Vector3(
                jugador.position.x,
                objetoFantasma.transform.position.y,
                jugador.position.z
            )
        );

        objetoFantasma.SetActive(true);
        Debug.Log("¡Jumpscare frente al jugador!");
    }

    void MoverFantasma()
    {
        if (objetoFantasma != null)
        {
            objetoFantasma.SetActive(false);

            objetoFantasma.transform.position =
                new Vector3(
                    Random.Range(-10, 10),
                    0,
                    Random.Range(-10, 10)
                );
        }

        Debug.Log("Fantasma se movió a otra posición");
    }

    void ActivarManos()
    {
        if (manoIzquierda != null)
            manoIzquierda.SetActive(true);

        if (manoDerecha != null)
            manoDerecha.SetActive(true);

        manosActivas = true;
        cicloManosIniciado = true;
        temporizador = tiempoManosActivas;

        Debug.Log("Manos activadas");
    }

    void DesactivarManos()
    {
        if (manoIzquierda != null)
            manoIzquierda.SetActive(false);

        if (manoDerecha != null)
            manoDerecha.SetActive(false);

        manosActivas = false;

        Debug.Log("Manos desactivadas");
    }
}