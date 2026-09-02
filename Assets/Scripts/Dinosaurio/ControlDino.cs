using UnityEngine;

public class ControlDino : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dinosaurio;               // El GameObject del dinosaurio (con DinoCerebro)

    [Header("Tiempo de aparición")]
    public float tiempoAntesDeAparecer = 15f;

    private float temporizador;
    private bool dinoActivo = false;
    private bool enemigoHabilitado = false;

    void Start()
    {
        temporizador = tiempoAntesDeAparecer;

        if (dinosaurio != null)
        {
            dinosaurio.SetActive(false);
        }

        enemigoHabilitado = false;
    }

    void Update()
    {
        if (!enemigoHabilitado) return;

        if (!dinoActivo)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                AparecerDino();
            }
        }
    }

    public void IniciarEnemigo()
    {
        enemigoHabilitado = true;
        Debug.Log("Dinosaurio habilitado (requisitos cumplidos). Iniciando temporizador...");
    }

    void AparecerDino()
    {
        dinoActivo = true;

        if (dinosaurio != null)
        {
            dinosaurio.SetActive(true);
            Debug.Log("¡El dinosaurio ha aparecido!");
        }
    }
}