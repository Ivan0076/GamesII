using UnityEngine;

/// Controla las animaciones de la asesina basándose en el estado de AsesinaCerebro.
/// Asume que el Animator tiene los parámetros: "Velocidad" (float) y "Persiguiendo" (bool).
[RequireComponent(typeof(Animator))]

public class AnimacionesAsesina : MonoBehaviour
{
    public AsesinaCerebro cerebro;
    private Animator animator;

    private readonly string PARAM_VELOCIDAD = "Velocidad";
    private readonly string PARAM_PERSIGUIENDO = "Persiguiendo";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (cerebro == null) cerebro = GetComponent<AsesinaCerebro>();
    }

    private void Update()
    {
        if (cerebro == null || animator == null) return;

        // Obtener velocidad real del agente
        float velocidad = cerebro.agente != null ? cerebro.agente.velocity.magnitude : 0f;
        bool persiguiendo = cerebro.EstaPersiguiendo;

        // Actualizar parámetros
        animator.SetFloat(PARAM_VELOCIDAD, velocidad);
        animator.SetBool(PARAM_PERSIGUIENDO, persiguiendo);

        // Depuración (puedes comentar después)
        Debug.Log($"Velocidad: {velocidad:F2}, Persiguiendo: {persiguiendo}");
    }
}
