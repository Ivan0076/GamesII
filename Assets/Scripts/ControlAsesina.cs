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

    void Start()
    {
        temporizador = tiempoAntesDeAparecer;

        if (asesina != null)
        {
            asesina.SetActive(false);
        }
    }

    void Update()
    {
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