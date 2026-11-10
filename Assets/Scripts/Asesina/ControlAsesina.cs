using UnityEngine;
using UnityEngine.AI;

public class ControlAsesina : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject asesina; // Arrastra el GameObject "Asesina" aquí
    public ControlLuces controlLuces;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoAparecer;
    public AudioClip sonidoDesaparecer;

    public System.Action OnAsesinaRetirada;
    private bool asesinaActiva = false;
    private NavMeshAgent agenteAsesina;
    private GestorSpawns gestorSpawns;

    void Start()
    {
        if (asesina != null)
        {
            asesina.SetActive(false);

            // Buscamos el agente y el gestor de spawns DENTRO del GameObject de la asesina
            agenteAsesina = asesina.GetComponent<NavMeshAgent>();
            gestorSpawns = asesina.GetComponent<GestorSpawns>();

            if (agenteAsesina == null)
                Debug.LogError("No se encontró NavMeshAgent en el objeto Asesina.");
            if (gestorSpawns == null)
                Debug.LogError("No se encontró GestorSpawns en el objeto Asesina.");
        }
    }

    public void Aparecer()
    {
        if (asesinaActiva) return;
        asesinaActiva = true;

        // 1. Activamos la asesina primero
        if (asesina != null)
        {
            asesina.SetActive(true);
        }

        // 2. Teletransportamos usando el GestorSpawns de la asesina
        if (gestorSpawns != null && agenteAsesina != null)
        {
            Vector3 puntoAleatorio = gestorSpawns.ObtenerPuntoAleatorio();

            if (NavMesh.SamplePosition(puntoAleatorio, out NavMeshHit hit, 20f, NavMesh.AllAreas))
            {
                // Apagamos el agente, movemos el Transform y lo volvemos a encender
                agenteAsesina.enabled = false;
                asesina.transform.position = hit.position;
                agenteAsesina.enabled = true;
                agenteAsesina.Warp(hit.position);

                Debug.Log("Asesina teletransportada a: " + hit.position);
            }
        }

        if (controlLuces != null) controlLuces.ApagarLuces();
        if (audioSource != null && sonidoAparecer != null) audioSource.PlayOneShot(sonidoAparecer);
    }

    void Update()
    {
        if (asesinaActiva && controlLuces != null && controlLuces.LucesEncendidas)
        {
            DesaparecerAsesina();
        }
    }

    public void DesaparecerAsesina()
    {
        if (!asesinaActiva) return;

        asesinaActiva = false;
        if (asesina != null) asesina.SetActive(false);
        if (audioSource != null && sonidoDesaparecer != null) audioSource.PlayOneShot(sonidoDesaparecer);

        OnAsesinaRetirada?.Invoke();
    }
}