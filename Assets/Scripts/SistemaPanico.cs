using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SistemaPanico : MonoBehaviour
{
    [Header("Condición de Inicio")]
    public TareaBase tareaCerrarCine;
    public TareaBase tareaApagarProyectores;
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
        if (juegoTerminado) return;

        if (!panicoActivo)
        {
            if (tareaCerrarCine != null && tareaApagarProyectores != null)
            {
                if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
                {
                    panicoActivo = true;
                }
            }
            return;
        }

        bool monstruoCerca = false;

        foreach (Transform monstruo in monstruos)
        {
            if (monstruo == null || !monstruo.gameObject.activeInHierarchy) continue;

            float distancia = Vector3.Distance(jugador.position, monstruo.position);

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

        panico = Mathf.Clamp(panico, 0f, panicoMaximo);

        if (barraPanico != null)
        {
            barraPanico.fillAmount = panico / panicoMaximo;
            barraPanico.color = Color.Lerp(Color.green, Color.red, panico / panicoMaximo);
        }

        if (textoPorcentaje != null)
        {
            textoPorcentaje.text = "Pánico: " + Mathf.RoundToInt(panico) + "%";
        }

        if (panico >= panicoMaximo)
        {
            ActivarGameOver();
        }
    }

    // MÉTODO PÚBLICO PARA QUE EL FANTASMA SUME PÁNICO
    public void AumentarPanico(float cantidad)
    {
        panico += cantidad;
        Debug.Log($"¡Pánico aumentado por susto! +{cantidad}");
    }

    void ActivarGameOver()
    {
        juegoTerminado = true;
        if (panelGameOver != null) panelGameOver.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerPrefs.SetInt("AsesinoMuerte", 0); // 0 = Default / Pánico
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("EscenaMuerte");
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}