using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestorTareas : MonoBehaviour
{
    [Header("Lista de tareas (asignar en el Inspector)")]
    public List<TareaBase> tareas;

    [Header("UI de Victoria")]
    public GameObject panelVictoria;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoVictoria;

    private int tareasCompletadas = 0;

    public event System.Action<ITarea> TareaCompletada;
    public event System.Action TodasLasTareasCompletadas;

    void Start()
    {
        if (panelVictoria != null)
            panelVictoria.SetActive(false);

        EvaluarTareas();
    }

    public void EvaluarTareas()
    {
        foreach (var t in tareas)
        {
            if (t != null && !t.EstaCompletada && t.PuedeIniciarse())
            {
                t.IniciarTarea();
            }
        }
    }

    public void RegistrarTareaCompletada(ITarea tarea)
    {
        tareasCompletadas++;
        Debug.Log($"Tarea completada: {tarea.nombreTarea} ({tareasCompletadas}/{tareas.Count})");

        TareaCompletada?.Invoke(tarea);

        EvaluarTareas();

        if (tareasCompletadas >= tareas.Count)
        {
            Debug.Log("¡Todas las tareas completadas! Mostrando victoria.");
            MostrarPantallaVictoria();
        }
    }

    private void MostrarPantallaVictoria()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        if (audioSource != null && sonidoVictoria != null)
            audioSource.PlayOneShot(sonidoVictoria);

        TodasLasTareasCompletadas?.Invoke();
    }

    // ====================================================================
    // MÉTODOS PARA LOS BOTONES
    // ====================================================================

    /// <summary>
    /// Asigna este método al botón "Siguiente Noche" de tu panel de victoria.
    /// </summary>
    public void IrSiguienteNoche()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.nocheActual++;
            Debug.Log("Siguiente noche: " + GameManager.Instance.nocheActual);
        }

        SceneManager.LoadScene("TransicionNoche");
    }

    /// <summary>
    /// Asigna este método al botón "Salir al Menú" de tu panel de victoria.
    /// </summary>
    public void SalirAlMenuPrincipal()
    {
        // Es vital restaurar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // Reemplaza "MenuPrincipal" por el nombre exacto de tu escena de menú
        SceneManager.LoadScene("MenuPrincipal");
    }
}