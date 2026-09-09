using UnityEngine;


/// Controla el flash de una cámara de seguridad individual.
/// Se coloca en cada cámara (GameObject dentro de la lista de SistemaCamaras).

public class ControlCamaraSeguridad : MonoBehaviour
{
    [Header("Referencias")]
    public DinoCerebro dinosaurio;       // El dinosaurio que vigila esta cámara

    [Header("Audio (opcional)")]
    public AudioSource audioSource;      // Fuente de audio para efectos
    public AudioClip sonidoFlash;        // Sonido al activar el flash
    public AudioClip sonidoAtrapado;     // Sonido cuando atrapa al dinosaurio


    /// Activa el flash de esta cámara.
    /// Si el dinosaurio está en zona peligro, lo atrapa.

    public void ActivarFlash()
    {
        // Reproducir sonido de flash (si existe)
        if (audioSource != null && sonidoFlash != null)
        {
            audioSource.PlayOneShot(sonidoFlash);
            Debug.Log("Sonido de flash reproducido");
        }

        Debug.Log($"Flash activado desde cámara {gameObject.name}");

        // Verificar si el dinosaurio está en zona de peligro
        if (dinosaurio != null && dinosaurio.EstadoActual == PuntosDino.TipoPunto.Peligro)
        {
            DinosaurioAtrapado();
        }
        else
        {
            Debug.Log("Dinosaurio no está en zona de peligro. Flash desperdiciado.");
        }
    }


    /// Atrapa al dinosaurio y reproduce sonido.

    private void DinosaurioAtrapado()
    {
        Debug.Log($"¡Dinosaurio atrapado desde cámara {gameObject.name}!");

        // Reproducir sonido de atrapado (si existe)
        if (audioSource != null && sonidoAtrapado != null)
        {
            audioSource.PlayOneShot(sonidoAtrapado);
            Debug.Log("Sonido de atrapado reproducido");
        }

        // Atrapar al dinosaurio
        if (dinosaurio != null)
        {
            dinosaurio.Atrapar();
        }
    }
}