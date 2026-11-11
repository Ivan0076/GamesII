using UnityEngine;
using System.Collections;

public class DinoCerebro : MonoBehaviour
{
    [Header("Ruta del dinosaurio")]
    public PuntosDino[] puntos;

    [Header("Altura sobre los puntos")]
    public float altura = 2f;

    [Header("Tiempo tras ser atrapado")]
    public float tiempoAntesDeDesaparecer = 2f;

    [Header("Fase 2")]
    public GameObject dinosaurioFase2;

    [Header("Jumpscare (sonido)")]
    public AudioSource audioSource;
    public AudioClip sonidoJumpscare;
    public float duracionJumpscare = 2f;

    // EVENTOS para el ControlDino
    public System.Action OnDinoAtrapado;
    public System.Action OnDinoLlegoAlFinal;

    private Animator animator;
    private int puntoActual = 0;
    private bool atrapado = false;
    private Coroutine rutinaActual;

    private const int POSE_INICIO = 0;
    private const int POSE_PELIGRO = 1;
    private const int POSE_ATRAPADO = 3;

    public PuntosDino.TipoPunto EstadoActual
    {
        get
        {
            if (puntos == null || puntos.Length == 0) return PuntosDino.TipoPunto.Inicio;
            return puntos[puntoActual].tipo;
        }
    }

    private void OnEnable()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        ReiniciarRecorrido();
    }

    private void OnDisable()
    {
        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }
    }

    /// <summary>
    /// Reinicia el estado del dino: lo devuelve al punto inicial,
    /// limpia el flag de atrapado y (si el objeto está activo) arranca la ruta.
    /// Es seguro llamarlo con el objeto inactivo; en ese caso OnEnable
    /// arrancará la corrutina al activarse.
    /// </summary>
    public void ReiniciarRecorrido()
    {
        atrapado = false;
        puntoActual = 0;

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }

        if (puntos == null || puntos.Length == 0) return;

        // Colocar visualmente en el punto inicial
        Vector3 pos = puntos[0].transform.position;
        pos.y += altura;
        transform.position = pos;

        // Asegurar pose de inicio
        CambiarPose(puntos[0].tipo);

        // Solo arrancar corrutina si el objeto está activo.
        // Si está inactivo, OnEnable la arrancará al activarse.
        if (isActiveAndEnabled)
            rutinaActual = StartCoroutine(RecorrerRuta());
    }

    private IEnumerator RecorrerRuta()
    {
        for (puntoActual = 0; puntoActual < puntos.Length; puntoActual++)
        {
            if (atrapado) yield break;

            PuntosDino punto = puntos[puntoActual];

            if (punto.tipo == PuntosDino.TipoPunto.Final)
            {
                yield return StartCoroutine(SecuenciaFinal());
                yield break;
            }

            CambiarPose(punto.tipo);

            Vector3 pos = punto.transform.position;
            pos.y += altura;
            transform.position = pos;

            float t = 0f;
            while (t < punto.tiempoEspera)
            {
                if (atrapado) yield break;
                yield return null;
                t += Time.deltaTime;
            }
        }
    }

    private IEnumerator SecuenciaFinal()
    {
        if (audioSource != null && sonidoJumpscare != null) audioSource.PlayOneShot(sonidoJumpscare);
        yield return new WaitForSeconds(duracionJumpscare);

        // IMPORTANTE: activar fase 2 ANTES de desactivar este GameObject.
        // Si desactivamos primero, la corrutina se detiene y la fase 2 nunca se activaría.
        if (dinosaurioFase2 != null)
        {
            dinosaurioFase2.SetActive(true);
            DinoFaseDos fase2 = dinosaurioFase2.GetComponent<DinoFaseDos>();
            if (fase2 != null) fase2.IniciarFase();
        }

        // Avisar al ControlDino que llegó al final (libera el slot en el Director)
        OnDinoLlegoAlFinal?.Invoke();

        // Desactivar el dino original
        gameObject.SetActive(false);
    }

    private void CambiarPose(PuntosDino.TipoPunto tipo)
    {
        if (animator == null) return;
        int valor = (tipo == PuntosDino.TipoPunto.Peligro) ? POSE_PELIGRO : POSE_INICIO;
        animator.SetInteger("EstadoPose", valor);
    }

    public void Atrapar()
    {
        if (atrapado) return;
        atrapado = true;

        // Avisamos al ControlDino que fuimos atrapados
        OnDinoAtrapado?.Invoke();

        if (animator != null) animator.SetInteger("EstadoPose", POSE_ATRAPADO);

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        StartCoroutine(DesaparecerTrasAtrapado());
    }

    private IEnumerator DesaparecerTrasAtrapado()
    {
        yield return new WaitForSeconds(tiempoAntesDeDesaparecer);
        gameObject.SetActive(false);
    }
}