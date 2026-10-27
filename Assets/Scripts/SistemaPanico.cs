using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SistemaPanico : MonoBehaviour
{

    [Header("Activacion")]
    public float tiempoActivacionPanico = 60f; // 1 minutos
    private float tiempoTranscurrido = 0f;
    private bool panicoActivo = false;

    [Header("Panico")]
    public float panico = 0f;
    public float panicoMaximo = 100f;
    public TMP_Text textoPorcentaje;

    [Header("Jugador")]
    public Transform jugador;

    [Header("Monstruos")]
    public Transform[] monstruos;

    [Header("Configuracion")]
    public float distanciaDeteccion = 10f;
    public float velocidadAumentoPanico = 15f;
    public float velocidadDisminucionPanico = 5f;

    [Header("Interfaz")]
    public Image barraPanico;

    [Header("Game Over")]
    public GameObject panelGameOver;

    private bool juegoTerminado = false;

    void Update()
    {

        Debug.Log("Update funcionando");

        if (juegoTerminado)
            return;


        tiempoTranscurrido += Time.deltaTime;

        Debug.Log("Tiempo: " + tiempoTranscurrido);

        if (!panicoActivo && tiempoTranscurrido >= tiempoActivacionPanico)
        {
            panicoActivo = true;
            Debug.Log("Sistema de pánico ACTIVADO");
        }

        if (!panicoActivo)
        {
            return;
        }

        bool monstruoCerca = false;

        foreach (Transform monstruo in monstruos)
        {
            float distancia = Vector3.Distance(
                jugador.position,
                monstruo.position);

            if (distancia <= distanciaDeteccion)
            {
                monstruoCerca = true;

                panico += velocidadAumentoPanico * Time.deltaTime;
            }
        }

        if (!monstruoCerca)
        {
            panico -= velocidadDisminucionPanico * Time.deltaTime;
        }

        panico = Mathf.Clamp(
            panico,
            0f,
            panicoMaximo);

        barraPanico.fillAmount = panico / panicoMaximo;

        if (textoPorcentaje != null)
        {
            textoPorcentaje.text = "Pánico: " + Mathf.RoundToInt(panico) + "%";
        }

        barraPanico.color = Color.Lerp(
        Color.green,
        Color.red,
        panico / panicoMaximo);

        if (panico >= panicoMaximo)
        {
            ActivarGameOver();
        }
    }

    void ActivarGameOver()
    {
        juegoTerminado = true;

        panelGameOver.SetActive(true);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;


        SceneManager.LoadScene("MenuPrincipal");
    }
}