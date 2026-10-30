using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic; // Necesario para usar List

public class JumpscareController : MonoBehaviour
{
    [Header("Configuración del Jumpscare")]
    public GameObject canvasJumpscare;
    public VideoPlayer videoJumpscare;
    public Transform camaraJugador;

    [Header("UI a Ocultar durante el susto")]
    [Tooltip("Arrastra aquí el crosshair, panel de pausa, etc. Se apagarán y no volverán hasta reiniciar (o hasta que tú decidas).")]
    public List<GameObject> panelesAOcultar = new List<GameObject>();

    [Header("Audio y Game Over")]
    public AudioSource audioSource;
    public AudioClip sonidoJumpscare;
    public AudioClip sonidoGameOver;
    public GameObject pantallaGameOver;

    public GameObject modeloPerseguidor;
    public float duracionJumpscare = 4f;

    private bool yaActivado = false;

    private void Start()
    {
        if (canvasJumpscare != null) canvasJumpscare.SetActive(false);
        if (pantallaGameOver != null) pantallaGameOver.SetActive(false);
        if (camaraJugador == null && Camera.main != null) camaraJugador = Camera.main.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (yaActivado) return;
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            yaActivado = true;
            IniciarJumpscare();
        }
    }

    private void IniciarJumpscare()
    {
        if (modeloPerseguidor != null) modeloPerseguidor.SetActive(false);

        // Pausar el juego
        Time.timeScale = 0f;

        // Ocultar selectivamente los paneles de la UI
        foreach (GameObject panel in panelesAOcultar)
        {
            if (panel != null) panel.SetActive(false);
        }

        if (audioSource != null && sonidoJumpscare != null)
            audioSource.PlayOneShot(sonidoJumpscare);

        if (canvasJumpscare != null && camaraJugador != null)
        {
            canvasJumpscare.SetActive(true);

            // Centrar perfectamente frente a la cámara
            // Aleja el video 0.6 metros y lo baja un poquito (0.15 metros) para centrarlo en los ojos
            canvasJumpscare.transform.position = camaraJugador.position + (camaraJugador.forward * 0.6f) + (Vector3.down * 0.15f);
            canvasJumpscare.transform.rotation = camaraJugador.rotation;
            canvasJumpscare.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }

        if (videoJumpscare != null)
        {
            videoJumpscare.Play();
        }

        StartCoroutine(SecuenciaGameOver());
    }

    private IEnumerator SecuenciaGameOver()
    {
        yield return new WaitForSecondsRealtime(duracionJumpscare);

        if (videoJumpscare != null) videoJumpscare.Stop();
        if (canvasJumpscare != null) canvasJumpscare.SetActive(false);

        if (pantallaGameOver != null)
            pantallaGameOver.SetActive(true);

        if (audioSource != null && sonidoGameOver != null)
            audioSource.PlayOneShot(sonidoGameOver);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}