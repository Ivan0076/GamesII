using UnityEngine;

public class ControlAsesina : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject asesina;
    public ControlLuces controlLuces;

    [Header("Tiempo de aparición")]
    public float tiempoAntesDeAparecer = 10f;

    [Header("Sonidos")]
    public AudioSource audioSource;          // Fuente de audio (puede ser la misma que ControlLuces)
    public AudioClip sonidoAparecer;         // Sonido al aparecer la asesina
    public AudioClip sonidoDesaparecer;      // Sonido al desaparecer la asesina

    private float temporizador;
    private bool asesinaActiva = false;
    private bool enemigoHabilitado = false;

    void Start()
    {
        temporizador = tiempoAntesDeAparecer;

        if (asesina != null)
        {
            asesina.SetActive(false);
        }

        enemigoHabilitado = false;
    }

    void Update()
    {
        if (!enemigoHabilitado) return;

        if (!asesinaActiva)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                AparecerAsesina();
            }
        }
        else
        {
            ComprobarLuces();
        }
    }

    public void IniciarEnemigo()
    {
        enemigoHabilitado = true;
        Debug.Log("Asesina habilitada (requisitos cumplidos). Iniciando temporizador...");
    }

    void AparecerAsesina()
    {
        asesinaActiva = true;

        if (asesina != null)
        {
            asesina.SetActive(true);
        }

        if (controlLuces != null)
        {
            controlLuces.ApagarLuces();
        }

        // Sonido de aparición
        if (audioSource != null && sonidoAparecer != null)
        {
            audioSource.PlayOneShot(sonidoAparecer);
        }

        Debug.Log("La asesina apareció. ¡Las luces se apagaron!");
    }

    void ComprobarLuces()
    {
        if (controlLuces != null && controlLuces.LucesEncendidas)
        {
            DesaparecerAsesina();
        }
    }

    public void DesaparecerAsesina()
    {
        asesinaActiva = false;
        temporizador = tiempoAntesDeAparecer;

        if (asesina != null)
        {
            asesina.SetActive(false);
        }

        // Sonido de desaparición
        if (audioSource != null && sonidoDesaparecer != null)
        {
            audioSource.PlayOneShot(sonidoDesaparecer);
        }

        Debug.Log("Las luces volvieron. ¡La asesina desapareció!");
    }
}