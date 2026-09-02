using UnityEngine;

public class ControlAsesina : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject asesina;
    public ControlLuces controlLuces;

    [Header("Tiempo de aparición")]
    public float tiempoAntesDeAparecer = 10f;

    private float temporizador;
    private bool asesinaActiva = false;
    private bool enemigoHabilitado = false; // ← Nuevo: controla si puede aparecer

    void Start()
    {
        temporizador = tiempoAntesDeAparecer;

        if (asesina != null)
        {
            asesina.SetActive(false);
        }

        // El enemigo se habilita mediante EnemigoRequisitos
        enemigoHabilitado = false;
    }

    void Update()
    {
        // Solo procesar si el enemigo está habilitado
        if (!enemigoHabilitado) return;

        if (!asesinaActiva)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                AparecerAsesina();
            }
        }
        else
        {
            ComprobarLuces();
        }
    }

    /// <summary>
    /// Método llamado por EnemigoRequisitos cuando se cumplen los requisitos.
    /// </summary>
    public void IniciarEnemigo()
    {
        enemigoHabilitado = true;
        Debug.Log("Asesina habilitada (requisitos cumplidos). Iniciando temporizador...");
    }

    void AparecerAsesina()
    {
        asesinaActiva = true;

        if (asesina != null)
        {
            asesina.SetActive(true);
        }

        if (controlLuces != null)
        {
            controlLuces.ApagarLuces();
        }

        Debug.Log("La asesina apareció. ¡Las luces se apagaron!");
    }

    void ComprobarLuces()
    {
        if (controlLuces != null && controlLuces.LucesEncendidas)
        {
            DesaparecerAsesina();
        }
    }

    public void DesaparecerAsesina()
    {
        asesinaActiva = false;
        temporizador = tiempoAntesDeAparecer;

        if (asesina != null)
        {
            asesina.SetActive(false);
        }

        Debug.Log("Las luces volvieron. ¡La asesina desapareció!");
    }
}