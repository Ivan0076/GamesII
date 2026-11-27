using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class JumpscareController : MonoBehaviour
{
    [Header("Configuración del Jumpscare")]
    public GameObject canvasJumpscare;
    public VideoPlayer videoJumpscare;
    public Transform camaraJugador;

    [Header("UI a Ocultar durante el susto")]
    [Tooltip("Arrastra aquí el crosshair, panel de pausa, etc. Se apagarán durante el susto.")]
    public List<GameObject> panelesAOcultar = new List<GameObject>();

    [Header("Audio y Game Over")]
    public AudioSource audioSource; // Añadí esto por si lo tienes asignado
    public AudioClip sonidoGameOver;

    // Ya no usaremos pantallaGameOver local, usaremos la escena, pero lo dejo por si acaso
    public GameObject pantallaGameOver;

    public GameObject modeloPerseguidor;
    public float duracionJumpscare = 4f;

    [Header("Identificador de Muerte")]
    [Tooltip("1 = Dino, 2 = Asesina, 3 = Fantasma")]
    public int idEnemigoMuerte = 1;
    public string nombreEscenaMuerte = "EscenaMuerte";

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
            IniciarJumpscare(); // Llamada única
        }
    }

    // 🔥 AQUÍ ESTÁ EL MÉTODO CORREGIDO (Solo uno)
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

        if (canvasJumpscare != null && camaraJugador != null)
        {
            canvasJumpscare.SetActive(true);

            // Centrar perfectamente frente a la cámara
            canvasJumpscare.transform.position = camaraJugador.position + (camaraJugador.forward * 0.6f) + (Vector3.down * 0.15f);
            canvasJumpscare.transform.rotation = camaraJugador.rotation;
            canvasJumpscare.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        }

        if (videoJumpscare != null) videoJumpscare.Play();
        if (audioSource != null && sonidoGameOver != null) audioSource.PlayOneShot(sonidoGameOver);

        // Iniciamos la secuencia que carga la nueva escena
        StartCoroutine(SecuenciaCargarMuerte());
    }

    // 🔥 ESTA ES LA NUEVA CORRUTINA
    private IEnumerator SecuenciaCargarMuerte()
    {
        // Esperamos a que termine el video usando Realtime (porque Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(duracionJumpscare);

        if (videoJumpscare != null) videoJumpscare.Stop();
        if (canvasJumpscare != null) canvasJumpscare.SetActive(false);

        // Guardamos quién nos mató para que la siguiente escena lo lea
        PlayerPrefs.SetInt("AsesinoMuerte", idEnemigoMuerte);
        PlayerPrefs.Save();

        // Restauramos el tiempo y liberamos el cursor
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cargamos la escena de muerte
        SceneManager.LoadScene(nombreEscenaMuerte);
    }
}