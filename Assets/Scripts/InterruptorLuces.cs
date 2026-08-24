using UnityEngine;
using UnityEngine.InputSystem;

public class InterruptorLuz : MonoBehaviour
{
    [Header("Control de luces")]
    public ControlLuces controlLuces;

    [Header("Control de la asesina")]
    public ControlAsesina controlAsesina;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivarInterruptor();
        }
    }

    private void ActivarInterruptor()
    {
        if (controlLuces != null)
        {
            controlLuces.EncenderLuces();
            Debug.Log("Interruptor activado.");
        }

        if (controlAsesina != null)
        {
            controlAsesina.DesaparecerAsesina();
        }
    }
}