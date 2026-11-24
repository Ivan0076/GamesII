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

        // Quitamos la línea que apagaba el objeto aquí
        cerebro.gameObject.SetActive(true);

        cerebro.IniciarCiclo();

        if (audioSource != null && sonidoAparicion != null)
            audioSource.PlayOneShot(sonidoAparicion);
    }

    private void RetirarFantasma()
    {
        if (fantasma != null) fantasma.SetActive(false);
        OnFantasmaRetirado?.Invoke();
        Debug.Log("ControlFantasma: El fantasma ha terminado su ciclo (RetirarFantasma).");
    }

    public void DesaparecerFantasma()
    {
        if (cerebro != null) cerebro.DetenerYLimpiar();
        if (fantasma != null) fantasma.SetActive(false);
    }
}