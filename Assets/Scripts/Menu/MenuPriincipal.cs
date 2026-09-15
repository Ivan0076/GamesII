using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPriincipal : MonoBehaviour
{
    public void NuevaPartida()
    {
        Debug.Log("Botón presionado, cargando escena...");
        SceneManager.LoadScene("CineNivel1"); 
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        // Nota: En el editor de Unity no se cierra la ventana al usar Application.Quit.
        // Solo funciona en el ejecutable compilado (.exe, .apk, etc.).
    }
}
