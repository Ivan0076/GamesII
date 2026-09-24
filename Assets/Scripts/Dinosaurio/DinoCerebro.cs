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
            if (puntos == null || puntos.Length == 0)
                return PuntosDino.TipoPunto.Inicio;
            return puntos[puntoActual].tipo;
        }
    }

    // ====================================================================
    // ACTIVAR / DESACTIVAR
    // ====================================================================

    private void OnEnable()
    {
        // Referencias
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (puntos == null || puntos.Length == 0)
        {
            Debug.LogWarning("El dinosaurio no tiene puntos asignados.");
            return;
        }

        // Reset estado y arrancar ruta
        atrapado = false;
        puntoActual = 0;

        if (rutinaActual != null)
            StopCoroutine(rutinaActual);

        rutinaActual = StartCoroutine(RecorrerRuta());
        Debug.Log("DinoCerebro activado. Iniciando ruta.");
    }

    private void OnDisable()
    {
        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }
    }

    // ====================================================================
    // RECORRIDO
    // ====================================================================

    private IEnumerator RecorrerRuta()
    {
        for (puntoActual = 0; puntoActual < puntos.Length; puntoActual++)
        {
            if (atrapado) yield break;

            PuntosDino punto = puntos[puntoActual];

            // Punto final → jumpscare y pasar a fase 2
            if (punto.tipo == PuntosDino.TipoPunto.Final)
            {
                yield return StartCoroutine(SecuenciaFinal());
                yield break;
            }

            CambiarPose(punto.tipo);

            Vector3 pos = punto.transform.position;
            pos.y += altura;
            transform.position = pos;

            Debug.Log($"Dino en punto {puntoActual} - Tipo: {punto.tipo}");

            float t = 0f;
            while (t < punto.tiempoEspera)
            {
                if (atrapado) yield break;
                yield return null;
                t += Time.deltaTime;
            }
        }

        Debug.Log("¡El dinosaurio terminó su recorrido!");
    }

    // ====================================================================
    // SECUENCIA FINAL → FASE 2
    // ====================================================================

    private IEnumerator SecuenciaFinal()
    {
        Debug.Log("Dino llegó al FINAL!");

        if (audioSource != null && sonidoJumpscare != null)
            audioSource.PlayOneShot(sonidoJumpscare);

        yield return new WaitForSeconds(duracionJumpscare);

        // Desactivar fase 1
        gameObject.SetActive(false);

        // Activar fase 2
        if (dinosaurioFase2 != null)
        {
            dinosaurioFase2.SetActive(true);
            DinoFaseDos fase2 = dinosaurioFase2.GetComponent<DinoFaseDos>();
            if (fase2 != null) fase2.IniciarFase();
        }
        else
        {
            Debug.LogError("No se asignó 'dinosaurioFase2' en DinoCerebro.");
        }
    }

    // ====================================================================
    // POSES
    // ====================================================================

    private void CambiarPose(PuntosDino.TipoPunto tipo)
    {
        if (animator == null) return;
        int valor = (tipo == PuntosDino.TipoPunto.Peligro) ? POSE_PELIGRO : POSE_INICIO;
        animator.SetInteger("EstadoPose", valor);
    }

    // ====================================================================
    // ATRAPAR
    // ====================================================================

    public void Atrapar()
    {
        if (atrapado) return;
        atrapado = true;

        Debug.Log("¡Dino atrapado! Desaparecerá en unos segundos...");

        if (animator != null)
            animator.SetInteger("EstadoPose", POSE_ATRAPADO);

        if (rutinaActual != null)
        {
            StopCoroutine(rutinaActual);
            rutinaActual = null;
        }

        StartCoroutine(DesaparecerTrasAtrapado());
    }

    private IEnumerator DesaparecerTrasAtrapado()
    {
        yield return new WaitForSeconds(tiempoAntesDeDesaparecer);
        gameObject.SetActive(false);
        Debug.Log("Dino fase 1 desapareció.");
    }
}