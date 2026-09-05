using UnityEngine;

public class ControlLuces : MonoBehaviour
{
    [Header("Luces que controla")]
    public Light[] luces;

    [Header("Sonidos")]
    public AudioSource audioSource;          // Fuente de audio (puede ser en el mismo objeto)
    public AudioClip sonidoEncender;         // Sonido al encender luces
    public AudioClip sonidoApagar;           // Sonido al apagar luces

    public bool LucesEncendidas { get; private set; } = true;

    [ContextMenu("Apagar luces")]
    public void ApagarLuces()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
                luz.enabled = false;
        }

        LucesEncendidas = false;

        // Reproducir sonido de apagar
        if (audioSource != null && sonidoApagar != null)
        {
            audioSource.PlayOneShot(sonidoApagar);
        }

        Debug.Log("Luces apagadas.");
    }

    [ContextMenu("Encender luces")]
    public void EncenderLuces()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
                luz.enabled = true;
        }

        LucesEncendidas = true;

        // Reproducir sonido de encender
        if (audioSource != null && sonidoEncender != null)
        {
            audioSource.PlayOneShot(sonidoEncender);
        }

        Debug.Log("Luces encendidas.");
    }
}