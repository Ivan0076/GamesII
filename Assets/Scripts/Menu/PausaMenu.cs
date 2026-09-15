using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaMenu : MonoBehaviour
{
    public GameObject menuPausa; // Panel del menú de pausa
    private bool juegoPausado = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape presionado");
            if (juegoPausado) Reanudar();
            else Pausar();
        }
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;

        Cursor.visible = true;                  // Mostrar cursor
        Cursor.lockState = CursorLockMode.None; // Liberar cursor
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;

        Cursor.visible = false;                 // Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked; // Bloquear cursor al centro
    }


    public void SalirJuego()
    {
        Debug.Log("Botón presionado, cargando escena...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
