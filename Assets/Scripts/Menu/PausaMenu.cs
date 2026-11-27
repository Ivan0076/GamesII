using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PausaMenu : MonoBehaviour
{
    [Header("UI de Pausa")]
    public GameObject menuPausa;

    [Header("Control VR")]
    public InputActionProperty botonPausa;

    private bool juegoPausado = false;

    private void OnEnable()
    {
        if (botonPausa.action != null)
        {
            botonPausa.action.Enable();
            botonPausa.action.performed += OnPausaPresionada;
        }
    }

    private void OnDisable()
    {
        if (botonPausa.action != null)
        {
            botonPausa.action.performed -= OnPausaPresionada;
            botonPausa.action.Disable();
        }
    }

    private void OnPausaPresionada(InputAction.CallbackContext context)
    {
        if (juegoPausado) Reanudar();
        else Pausar();
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f;
        juegoPausado = true;
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f;
        juegoPausado = false;
    }

    public void SalirJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}