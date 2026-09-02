using UnityEngine;

public class CerrarCine : TareaBase, IInteractuable
{
    [Header("Configuración del letrero")]
    public GameObject letrero;
    public Material materialCerrado;

    private bool letreroCambiado = false;
    private Renderer rendLetrero;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        if (letrero != null)
            rendLetrero = letrero.GetComponent<Renderer>();
        else
            Debug.LogWarning("Letrero no asignado en CerrarCine");
    }

    public void Interactuar()
    {
        if (letreroCambiado || EstaCompletada) return;

        letreroCambiado = true;

        if (rendLetrero != null)
        {
            rendLetrero.material = materialCerrado;
            Debug.Log("Letrero cambiado a 'Cerrado'");
        }

        CompletarTarea();
    }
}