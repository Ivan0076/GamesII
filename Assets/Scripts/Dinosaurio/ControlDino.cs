using UnityEngine;

public class ControlDino : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dinosaurio;
    public DinoCerebro cerebro;
    public DinoFaseDos faseDos;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoAparicion;

    // Evento para avisar al Director que el Dino se fue
    public System.Action OnDinoRetirado;

    void Start()
    {
        if (dinosaurio != null) dinosaurio.SetActive(false);

        // Nos suscribimos a los eventos del Dino
        if (cerebro != null)
        {
            cerebro.OnDinoAtrapado += RetirarDino;
            cerebro.OnDinoLlegoAlFinal += DinoLlegoAlFinal;
        }
        if (faseDos != null) faseDos.OnDinoSeRinde += RetirarDino;
    }

    // Método que llamará el Director
    public void Aparecer()
    {
        // Resetear estado Y posición ANTES de activar.
        // Funciona aunque el dinosaurio esté inactivo:
        // OnEnable volverá a llamar a ReiniciarRecorrido y arrancará la ruta.
        if (cerebro != null) cerebro.ReiniciarRecorrido();

        if (dinosaurio != null)
        {
            dinosaurio.SetActive(true);
            if (audioSource != null && sonidoAparicion != null)
                audioSource.PlayOneShot(sonidoAparicion);
            Debug.Log("¡El dinosaurio ha aparecido (llamado por el Director)!");
        }
    }

    // Se ejecuta si lo atrapan en Fase 1 o si la Fase 2 se rinde
    private void RetirarDino()
    {
        if (dinosaurio != null) dinosaurio.SetActive(false);

        // Avisamos al Director que el dino ya no está en juego
        OnDinoRetirado?.Invoke();
        Debug.Log("El dinosaurio se ha retirado. Slot liberado.");
    }

    // El dino llegó al punto Final y pasó a Fase 2.
    // La Fase 2 ya se activa desde DinoCerebro.SecuenciaFinal,
    // aquí solo liberamos el slot en el Director.
    private void DinoLlegoAlFinal()
    {
        OnDinoRetirado?.Invoke();
        Debug.Log("El dinosaurio llegó al final. Slot liberado (Fase 2 activa).");
    }
}