using UnityEngine;

public class InterruptorLuces : MonoBehaviour, IInteractuable
{
    [Header("Control de luces")]
    public ControlLuces controlLuces;

    private bool yaActivado = false;

    public void Interactuar()
    {
        // Si ya se usó una vez o no hay referencia, salir
        if (yaActivado) return;
        if (controlLuces == null)
        {
            Debug.LogWarning("ControlLuces no asignado en InterruptorLuces.");
            return;
        }

        // SOLO permitir encender si están apagadas
        if (!controlLuces.LucesEncendidas)
        {
            controlLuces.EncenderLuces();
            Debug.Log("Luces ENCENDIDAS por el jugador.");

            // Opcional: marcar como ya usado si quieres que solo funcione una vez
            // yaActivado = true; 
        }
        else
        {
            Debug.Log("Las luces ya están encendidas. No puedes apagarlas.");
        }
    }
}