using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SistemaPanico : MonoBehaviour
{
    [Header("Condición de Inicio")]
    [Tooltip("Arrastra aquí la tarea de Cerrar Cine")]
    public TareaBase tareaCerrarCine;
    [Tooltip("Arrastra aquí la tarea de Apagar Proyectores")]
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

        // 1. COMPROBAR SI LAS TAREAS INICIALES ESTÁN HECHAS
        if (!panicoActivo)
        {
            if (tareaCerrarCine != null && tareaApagarProyectores != null)
            {
                if (tareaCerrarCine.EstaCompletada && tareaApagarProyectores.EstaCompletada)
                {
                    panicoActivo = true;
                    Debug.Log("Sistema de pánico ACTIVADO (Tareas iniciales completadas)");
                }
            }
            return; // Si no están hechas, no hacemos nada más
        }

        bool monstruoCerca = false;

        foreach (Transform monstruo in monstruos)
        {
            // 2. IGNORAR ENEMIGOS DESACTIVADOS O QUE YA NO ESTÁN EN ESCENA
            if (monstruo == null || !monstruo.gameObject.activeInHierarchy)
                continue;

            float distancia = Vector3.Distance(jugador.position, monstruo.position);

            if (distancia <= distanciaDeteccion)
            {
                monstruoCerca = true;
                panico += velocidadAumentoPanico * Time.deltaTime;
            }
        }

        // Si no hay ningún enemigo activo cerca, el pánico baja
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

    void ActivarGameOver()
    {
        juegoTerminado = true;
        if (panelGameOver != null) panelGameOver.SetActive(true);
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