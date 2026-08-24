using UnityEngine;

public class ControlLuces : MonoBehaviour
{
    [Header("Luces que controla")]
    public Light[] luces;

    public bool LucesEncendidas { get; private set; } = true;

    [ContextMenu("Apagar luces")]
    public void ApagarLuces()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
                luz.enabled = false;
        }

        LucesEncendidas = false;

        Debug.Log("Luces apagadas.");
    }

    [ContextMenu("Encender luces")]
    public void EncenderLuces()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
                luz.enabled = true;
        }

        LucesEncendidas = true;

        Debug.Log("Luces encendidas.");
    }
}