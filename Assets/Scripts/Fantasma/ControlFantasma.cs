using UnityEngine;

public class ControlFantasma : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject fantasma;
    public CerebroFantasma cerebro;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoAparicion;

    public System.Action OnFantasmaRetirado;
    private bool fantasmaOcupado = false; // 🔥 Nueva bandera de control

    void Awake()
    {
        if (cerebro == null) cerebro = GetComponentInChildren<CerebroFantasma>(true);
        if (cerebro == null) cerebro = FindAnyObjectByType<CerebroFantasma>(FindObjectsInactive.Include);
        if (fantasma == null && cerebro != null) fantasma = cerebro.objetoFantasma;
    }

    void Start()
    {
        if (cerebro != null) cerebro.OnFantasmaTermino += RetirarFantasma;
    }

    public void Aparecer()
    {
        if (cerebro == null) return;

        // Si ya está en medio de un ataque, ignorar la orden del Director
        if (fantasmaOcupado)
        {
            Debug.Log("ControlFantasma: Ya está en ciclo. Ignorando orden.");
            return;
        }

        // Marcamos como ocupado INMEDIATAMENTE
        fantasmaOcupado = true;

        cerebro.gameObject.SetActive(true);
        cerebro.IniciarCiclo();

        if (audioSource != null && sonidoAparicion != null)
            audioSource.PlayOneShot(sonidoAparicion);
    }

    private void RetirarFantasma()
    {
        fantasmaOcupado = false; // 🔥 Liberamos la bandera para el próximo ciclo
        if (fantasma != null) fantasma.SetActive(false);

        OnFantasmaRetirado?.Invoke();
        Debug.Log("ControlFantasma: Ciclo finalizado. Listo para el cooldown.");
    }

    public void DesaparecerFantasma()
    {
        fantasmaOcupado = false;
        if (cerebro != null) cerebro.DetenerYLimpiar();
        if (fantasma != null) fantasma.SetActive(false);
    }
}