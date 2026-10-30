using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class JumpscareController : MonoBehaviour
{
    [Header("Configuración 2D")]
    public GameObject canvasJumpscare;
    public Transform camaraJugador;

    [Header("Video y Tiempos")]
    public VideoPlayer videoJumpscare;
    public float duracionJumpscare = 3f;

    [Header("Audio y Game Over")]
    public AudioSource audioSource;
    public AudioClip sonidoJumpscare;
    public AudioClip sonidoGameOver;
    public GameObject pantallaGameOver;

    public GameObject modeloPerseguidor;

    private bool yaActivado = false;

    private void Start()
    {
        if (canvasJumpscare != null) canvasJumpscare.SetActive(false);
        if (pantallaGameOver != null) pantallaGameOver.SetActive(false);

        if (camaraJugador == null && Camera.main != null)
            camaraJugador = Camera.main.transform;
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

        // 🔥 PAUSAR EL JUEGO (Detiene al jugador y a todos los enemigos)
        Time.timeScale = 0f;
        // Nota: Si usas XR Interaction Toolkit, esto también detendrá el movimiento.

        if (audioSource != null && sonidoJumpscare != null)
            audioSource.PlayOneShot(sonidoJumpscare);

        if (canvasJumpscare != null && camaraJugador != null)
        {
            // Lo activamos
            canvasJumpscare.SetActive(true);

            // Lo emparentamos a la cámara (sin conservar la escala mundial)
            canvasJumpscare.transform.SetParent(camaraJugador, false);

            // Lo ponemos a 1 metro de la cara (evita el recorte de la cámara)
            canvasJumpscare.transform.localPosition = new Vector3(0, 0, 1f);

            // Lo rotamos para que mire al frente
            canvasJumpscare.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // Forzamos una escala segura para VR
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
        // 🔥 Usamos Realtime porque el juego está pausado (timeScale = 0)
        yield return new WaitForSecondsRealtime(duracionJumpscare);

        if (videoJumpscare != null) videoJumpscare.Stop();
        if (canvasJumpscare != null) canvasJumpscare.SetActive(false);

        if (pantallaGameOver != null)
            pantallaGameOver.SetActive(true);

        if (audioSource != null && sonidoGameOver != null)
            audioSource.PlayOneShot(sonidoGameOver);

        // El juego se queda pausado en el Game Over
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}