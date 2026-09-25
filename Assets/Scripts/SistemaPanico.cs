using UnityEngine;
using UnityEngine.UI;

public class SistemaPanico : MonoBehaviour
{
    [Header("Panico")]
    public float panico = 0f;
    public float panicoMaximo = 100f;

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
        if (juegoTerminado)
            return;

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
    }
}