using UnityEngine;

public class ControlDino : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dinosaurio;            // El GameObject del dinosaurio (con DinoCerebro)
    public DinoCerebro dinoCerebro;          // Referencia al script DinoCerebro

    [Header("Tiempo de aparición")]
    public float tiempoAntesDeAparecer = 5f; // Tiempo tras cumplir requisitos

    private float temporizador;
    private bool dinoActivo = false;
    private bool habilitado = false;          // Controla si puede iniciar el temporizador

    void Start()
    {
        // Buscar referencias si no se asignaron
        if (dinosaurio == null)
            dinosaurio = gameObject;

        if (dinoCerebro == null)
            dinoCerebro = GetComponent<DinoCerebro>();

        // Desactivar el dinosaurio al inicio
        if (dinosaurio != null)
            dinosaurio.SetActive(false);

        temporizador = tiempoAntesDeAparecer;
        habilitado = false;
    }

    void Update()
    {
        // Solo procesar si el dinosaurio está habilitado
        if (!habilitado) return;
        if (dinoActivo) return;

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            AparecerDinosaurio();
        }
    }

    /// <summary>
    /// Método llamado por EnemigoRequisitos cuando se cumplen los requisitos.
    /// </summary>
    public void IniciarDino()
    {
        habilitado = true;
        Debug.Log("Dinosaurio habilitado (requisitos cumplidos). Iniciando temporizador...");
    }

    private void AparecerDinosaurio()
    {
        dinoActivo = true;

        if (dinosaurio != null)
        {
            dinosaurio.SetActive(true);
        }

        // Opcional: si el cerebro tiene algún método de inicio, llamarlo
        // if (dinoCerebro != null) dinoCerebro.Iniciar();

        Debug.Log("¡El dinosaurio apareció!");
    }

    // Método para reiniciar (si se necesita)
    public void Reiniciar()
    {
        dinoActivo = false;
        habilitado = false;
        temporizador = tiempoAntesDeAparecer;
        if (dinosaurio != null)
            dinosaurio.SetActive(false);
    }
}
