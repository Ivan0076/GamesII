using System.Collections.Generic;
using UnityEngine;

public class BotonFlashSeguridad : MonoBehaviour, IInteractuable
{
    [Header("Sistema de cámaras (arrastra cualquiera de los dos)")]
    public SistemaCamaras sistemaCamaras;   // Arrastra el GameObject con SistemaCamaras (botón siguiente o anterior)

    [Header("Cámaras permitidas (índices en la lista)")]
    public List<int> indicesPermitidos = new List<int> { 0, 1 }; // Cámara 1 y 2

    [Header("Audio (opcional)")]
    public AudioSource audioSource;
    public AudioClip sonidoBoton;
    public AudioClip sonidoError; // Sonido cuando no está permitido

    public void Interactuar()
    {
        if (sistemaCamaras == null)
        {
            Debug.LogWarning("Botón flash sin referencia a SistemaCamaras");
            return;
        }

        // Obtener el índice de la cámara activa (la que se muestra en la pantalla)
        int indiceActual = sistemaCamaras.camaraSeleccionada;

        // Verificar si este índice está permitido
        if (!indicesPermitidos.Contains(indiceActual))
        {
            Debug.Log($"Cámara {indiceActual + 1} no permite flash. Solo índices: {string.Join(", ", indicesPermitidos)}");
            if (audioSource != null && sonidoError != null)
            {
                // audioSource.PlayOneShot(sonidoError);
            }
            return;
        }

        // Obtener la cámara activa (GameObject) de la lista del mismo sistema
        if (sistemaCamaras.camaras == null || indiceActual >= sistemaCamaras.camaras.Count)
        {
            Debug.LogWarning("Lista de cámaras vacía o índice fuera de rango");
            return;
        }

        GameObject camaraActual = sistemaCamaras.camaras[indiceActual];
        if (camaraActual == null)
        {
            Debug.LogWarning("Cámara actual es nula");
            return;
        }

        // Obtener el componente ControlCamaraSeguridad de esa cámara
        ControlCamaraSeguridad control = camaraActual.GetComponent<ControlCamaraSeguridad>();
        if (control == null)
        {
            Debug.LogWarning($"La cámara {camaraActual.name} no tiene ControlCamaraSeguridad");
            return;
        }

        // Reproducir sonido del botón (si existe)
        if (audioSource != null && sonidoBoton != null)
        {
            // audioSource.PlayOneShot(sonidoBoton);
            Debug.Log("Botón flash presionado");
        }

        // Activar el flash en la cámara actual
        control.ActivarFlash();
    }
}