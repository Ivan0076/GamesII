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
        if (cerebro != null) cerebro.OnDinoAtrapado += RetirarDino;
        if (faseDos != null) faseDos.OnDinoSeRinde += RetirarDino;
    }

    // Método que llamará el Director
    public void Aparecer()
    {
        if (dinosaurio != null)
        {
            dinosaurio.SetActive(true);
            if (audioSource != null && sonidoAparicion != null)
                audioSource.PlayOneShot(sonidoAparicion);
            Debug.Log("¡El dinosaurio ha aparecido (llamado por el Director)!");
        }
    }

    // Este método se ejecuta si lo atrapan o si se rinde
    private void RetirarDino()
    {
        if (dinosaurio != null) dinosaurio.SetActive(false);

        // Avisamos al Director que el dino ya no está en juego
        OnDinoRetirado?.Invoke();
        Debug.Log("El dinosaurio se ha retirado. Slot liberado.");
    }
}