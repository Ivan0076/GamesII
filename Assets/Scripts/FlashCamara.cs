using UnityEngine;
using UnityEngine.InputSystem;

public class FlashCamara : MonoBehaviour
{
    [Header("Dinosaurio")]
    public DinoCerebro dinosaurio;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            ActivarFlash();
        }
    }

    private void ActivarFlash()
    {
        Debug.Log("¡FLASH DE CÁMARA!");

        if (dinosaurio.EstadoActual == PuntosDino.TipoPunto.Peligro)
        {
            DinosaurioAtrapado();
        }
        else
        {
            Debug.Log("Flash desperdiciado. El dinosaurio está en zona segura.");
        }
    }

    private void DinosaurioAtrapado()
    {
        Debug.Log("¡Dinosaurio atrapado!");

        dinosaurio.ReiniciarRuta();
    }
}