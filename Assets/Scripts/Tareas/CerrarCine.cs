using UnityEngine;

public class CerrarCine : TareaBase, IInteractuable
{
    [Header("Configuración del letrero")]
    public GameObject letrero;
    public string nombreTrigger = "Cerrar"; // Nombre del Trigger en el Animator

    private bool letreroCambiado = false;
    private Animator animLetrero;

    public override void IniciarTarea()
    {
        base.IniciarTarea();

        if (letrero != null)
        {
            animLetrero = letrero.GetComponent<Animator>();
            if (animLetrero == null)
                Debug.LogError("El letrero no tiene un componente Animator asignado.");
            else
                Debug.Log("Animator encontrado correctamente en el letrero.");
        }
        else
        {
            Debug.LogError("Letrero no asignado en CerrarCine");
        }
    }

    public void Interactuar()
    {
        if (letreroCambiado || EstaCompletada) return;

        letreroCambiado = true;
        Debug.Log("Interacción detectada con el letrero.");

        if (animLetrero != null)
        {
            // Usamos SetTrigger en lugar de Play para mayor seguridad
            animLetrero.SetTrigger(nombreTrigger);
            Debug.Log($"Trigger '{nombreTrigger}' activado en el Animator.");
        }

        CompletarTarea();
    }
}