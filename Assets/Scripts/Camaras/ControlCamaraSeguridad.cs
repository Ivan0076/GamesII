using System.Collections;
using UnityEngine;

/// Controla el flash de una cámara de seguridad individual.
public class ControlCamaraSeguridad : MonoBehaviour
{
    [Header("Referencias")]
    public DinoCerebro dinosaurio;

    [Header("Audio (opcional)")]
    public AudioSource audioSource;
    public AudioClip sonidoFlash;
    public AudioClip sonidoAtrapado;

    [Header("Flash (luz)")]
    [Tooltip("Luz que simula el flash. Si es null, se crea automáticamente.")]
    public Light luzFlash;
    [Tooltip("Intensidad máxima del flash al disparar.")]
    public float intensidadMaxima = 8f;
    [Tooltip("Velocidad de decaimiento de la intensidad (unidades/segundo).")]
    public float velocidadDecaimiento = 6f;
    [Tooltip("Tiempo mínimo entre usos (segundos).")]
    public float cooldown = 1.5f;

    private bool flashDisponible = true;

    void Awake()
    {
        // Si no hay luz asignada, la creamos como hijo de esta cámara
        if (luzFlash == null)
        {
            GameObject go = new GameObject("LuzFlash");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            luzFlash = go.AddComponent<Light>();
            luzFlash.type = LightType.Spot;
            luzFlash.spotAngle = 60f;
            luzFlash.range = 30f;
            luzFlash.color = Color.white;
            luzFlash.intensity = 0f;
            luzFlash.enabled = false;
        }
        else
        {
            luzFlash.intensity = 0f;
            luzFlash.enabled = false;
        }
    }

    /// Activa el flash de esta cámara.
    public void ActivarFlash()
    {
        if (!flashDisponible)
        {
            Debug.Log("Flash en cooldown, espera un momento.");
            return;
        }

        // Audio
        if (audioSource != null && sonidoFlash != null)
        {
            audioSource.PlayOneShot(sonidoFlash);
            Debug.Log("Sonido de flash reproducido");
        }

        Debug.Log($"Flash activado desde cámara {gameObject.name}");

        // Lógica del dinosaurio
        if (dinosaurio != null && dinosaurio.EstadoActual == PuntosDino.TipoPunto.Peligro)
        {
            DinosaurioAtrapado();
        }
        else
        {
            Debug.Log("Dinosaurio no está en zona de peligro. Flash desperdiciado.");
        }

        // Efecto visual + cooldown
        StartCoroutine(EfectoFlash());
    }

    private IEnumerator EfectoFlash()
    {
        flashDisponible = false;

        luzFlash.enabled = true;
        luzFlash.intensity = intensidadMaxima;

        // Decaimiento progresivo
        while (luzFlash.intensity > 0.01f)
        {
            luzFlash.intensity -= velocidadDecaimiento * Time.deltaTime;
            yield return null;
        }

        luzFlash.intensity = 0f;
        luzFlash.enabled = false;

        // Esperar el resto del cooldown (el tiempo ya consumido en el decaimiento cuenta)
        float tiempoRestante = cooldown - (intensidadMaxima / velocidadDecaimiento);
        if (tiempoRestante > 0f)
            yield return new WaitForSeconds(tiempoRestante);

        flashDisponible = true;
        Debug.Log($"Flash de {gameObject.name} listo de nuevo.");
    }

    private void DinosaurioAtrapado()
    {
        Debug.Log($"¡Dinosaurio atrapado desde cámara {gameObject.name}!");

        if (audioSource != null && sonidoAtrapado != null)
        {
            audioSource.PlayOneShot(sonidoAtrapado);
            Debug.Log("Sonido de atrapado reproducido");
        }

        if (dinosaurio != null)
        {
            dinosaurio.Atrapar();
        }
    }
}