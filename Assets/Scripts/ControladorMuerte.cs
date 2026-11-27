using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorMuerte : MonoBehaviour
{
    [Header("UI de las Muertes (Arrastrar los Paneles)")]
    public GameObject panelDino;
    public GameObject panelAsesina;
    public GameObject panelFantasma;
    public GameObject panelDefecto; // Para el pánico o daño general

    [Header("Botones")]
    public string escenaMenu = "MenuPrincipal";
    public string escenaNivel = "CineNivel1"; // Para el botón de reintentar

    void Start()
    {
        // Ocultamos todos primero
        panelDino.SetActive(false);
        panelAsesina.SetActive(false);
        panelFantasma.SetActive(false);
        panelDefecto.SetActive(false);

        // Leemos quién nos mató (Por defecto es 0, que es el pánico)
        int idAsesino = PlayerPrefs.GetInt("AsesinoMuerte", 0);

        // Mostramos el panel correspondiente
        switch (idAsesino)
        {
            case 1:
                panelDino.SetActive(true);
                break;
            case 2:
                panelAsesina.SetActive(true);
                break;
            case 3:
                panelFantasma.SetActive(true);
                break;
            default:
                panelDefecto.SetActive(true);
                break;
        }

        // Restauramos el tiempo y el cursor
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BotonReintentar()
    {
        // Vuelve a cargar el nivel del cine
        SceneManager.LoadScene(escenaNivel);
    }

    public void BotonMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }
}