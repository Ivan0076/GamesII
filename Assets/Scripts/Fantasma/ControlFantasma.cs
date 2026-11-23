using UnityEngine;

public class ControlFantasma : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject fantasma;
    public CerebroFantasma cerebro;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoAparicion;

    // Evento para avisar al Director que el Fantasma se fue
    public System.Action OnFantasmaRetirado;

    void Awake()
    {
        // Si no se asignó manualmente en el Inspector, lo busca en hijos o en toda la escena
        if (cerebro == null) cerebro = GetComponentInChildren<CerebroFantasma>(true);
        if (cerebro == null) cerebro = FindAnyObjectByType<CerebroFantasma>(FindObjectsInactive.Include);

        if (fantasma == null && cerebro != null) fantasma = cerebro.objetoFantasma;

        if (fantasma != null) fantasma.SetActive(false);

        if (cerebro != null)
        {
            if (cerebro.manoIzquierda != null) cerebro.manoIzquierda.SetActive(false);
            if (cerebro.manoDerecha != null) cerebro.manoDerecha.SetActive(false);
        }
    }

    void Start()
    {
        if (cerebro != null)
        {
            cerebro.OnFantasmaTermino += RetirarFantasma;
        }
        else
        {
            Debug.LogError("ControlFantasma: ¡No se encontró ningún CerebroFantasma en la escena!");
        }
    }

    // Método que llamará el Director
    public void Aparecer()
    {
        if (cerebro == null)
        {
            Debug.LogError("ControlFantasma: No puede iniciar porque 'cerebro' es null.");
            return;
        }

        // Aseguramos que el objeto donde vive CerebroFantasma esté activo en la jerarquía
        cerebro.gameObject.SetActive(true);

        if (!cerebro.gameObject.activeInHierarchy)
        {
            Debug.LogError($"ControlFantasma: El objeto '{cerebro.gameObject.name}' tiene un PADRE desactivado en la jerarquía, por eso su Update no corre.");
        }

        cerebro.IniciarCiclo();

        if (audioSource != null && sonidoAparicion != null)
            audioSource.PlayOneShot(sonidoAparicion);

        Debug.Log("¡El fantasma ha entrado en juego (llamado por el Director)!");
    }

    private void RetirarFantasma()
    {
        if (fantasma != null) fantasma.SetActive(false);

        OnFantasmaRetirado?.Invoke();
        Debug.Log("El fantasma se ha retirado. Slot liberado.");
    }

    public void DesaparecerFantasma()
    {
        if (cerebro != null) cerebro.DetenerYLimpiar();
        if (fantasma != null) fantasma.SetActive(false);
    }
}