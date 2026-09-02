using UnityEngine;
using System.Collections;

public class DinoCerebro : MonoBehaviour
{
    [Header("Ruta del dinosaurio")]
    public PuntosDino[] puntos;

    [Header("Altura sobre los puntos")]
    public float altura = 2f;

    [Header("Tiempo de espera tras ser atrapado")]
    public float tiempoAtrapado = 2f;

    // Referencia al Animator del modelo
    private Animator animator;
    private int puntoActual = 0;
    private bool atrapado = false;      // Controla si el dino está atrapado

    // Mapeo de tipos de punto a valores del parámetro "EstadoPose"
    private const int POSE_INICIO = 0;
    private const int POSE_PELIGRO = 1;
    private const int POSE_SEGURO = 2;
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

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("¡No se encontró un Animator en el dinosaurio o sus hijos!");
            return;
        }

        if (puntos.Length == 0)
        {
            Debug.LogWarning("El dinosaurio no tiene puntos asignados.");
            return;
        }

        // Iniciar la ruta
        StartCoroutine(RecorrerRuta());
    }

    private IEnumerator RecorrerRuta()
    {
        for (puntoActual = 0; puntoActual < puntos.Length; puntoActual++)
        {
            // Si el dinosaurio fue atrapado, salir del bucle (detener la ruta)
            if (atrapado)
            {
                Debug.Log("Dino atrapado, deteniendo ruta...");
                yield break; // Sale de la corrutina
            }

            PuntosDino punto = puntos[puntoActual];

            // Cambiar la pose
            CambiarPose(punto.tipo);

            // Mover al punto
            Vector3 nuevaPosicion = punto.transform.position;
            nuevaPosicion.y += altura;
            transform.position = nuevaPosicion;

            Debug.Log($"Dino en punto {puntoActual} - Tipo: {punto.tipo}");

            // Esperar el tiempo del punto, pero comprobando si es atrapado durante la espera
            float tiempoEsperado = punto.tiempoEspera;
            float tiempoTranscurrido = 0f;
            while (tiempoTranscurrido < tiempoEsperado)
            {
                if (atrapado)
                {
                    Debug.Log("Dino atrapado durante la espera, saliendo...");
                    yield break; // Sale de la corrutina
                }
                yield return null; // Espera un frame
                tiempoTranscurrido += Time.deltaTime;
            }
        }

        Debug.Log("¡El dinosaurio terminó su recorrido!");
        // Opcional: al terminar, reiniciar automáticamente
        // StartCoroutine(ReiniciarRuta());
    }

    /// <summary>
    /// Cambia la pose usando el Animator.
    /// </summary>
    private void CambiarPose(PuntosDino.TipoPunto tipo)
    {
        if (animator == null) return;

        int valorPose;
        switch (tipo)
        {
            case PuntosDino.TipoPunto.Inicio:
                valorPose = POSE_INICIO;
                break;
            case PuntosDino.TipoPunto.Peligro:
                valorPose = POSE_PELIGRO;
                break;
            case PuntosDino.TipoPunto.Seguro:
                valorPose = POSE_INICIO;
                break;
            default:
                valorPose = POSE_INICIO;
                break;
        }

        animator.SetInteger("EstadoPose", valorPose);
    }

    /// <summary>
    /// Método llamado cuando el dinosaurio es atrapado.
    /// </summary>
    public void Atrapar()
    {
        if (atrapado) return; // Evita llamadas múltiples

        Debug.Log("¡Dinosaurio atrapado!");
        atrapado = true;

        // Cambiar a la pose de atrapado
        if (animator != null)
            animator.SetInteger("EstadoPose", POSE_ATRAPADO);

        // Detener todas las corrutinas (incluyendo RecorrerRuta)
        StopAllCoroutines();

        // Iniciar la secuencia de reinicio después del tiempo de atrapado
        StartCoroutine(ReiniciarDespuesDeAtrapado());
    }

    private IEnumerator ReiniciarDespuesDeAtrapado()
    {
        yield return new WaitForSeconds(tiempoAtrapado);

        // Restablecer el estado
        atrapado = false;

        // Reiniciar la ruta desde el principio
        ReiniciarRuta();
    }

    /// Reinicia la ruta desde el primer punto.
    public void ReiniciarRuta()
    {
        // Detener cualquier corrutina en curso
        StopAllCoroutines();

        // Resetear índices y estado
        puntoActual = 0;
        atrapado = false;

        // Restaurar la pose de inicio
        CambiarPose(PuntosDino.TipoPunto.Inicio);

        // Mover al primer punto
        if (puntos.Length > 0)
        {
            Vector3 nuevaPosicion = puntos[0].transform.position;
            nuevaPosicion.y += altura;
            transform.position = nuevaPosicion;
        }

        // Iniciar de nuevo la ruta
        StartCoroutine(RecorrerRuta());
    }
}