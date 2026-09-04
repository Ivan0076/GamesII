using UnityEngine;

public class PortapapelesObjeto : MonoBehaviour, IInteractuable
{
    [Header("Referencias")]
    public PortapapelesUI uiPortapapeles;

    private bool recogido = false;

    void Start()
    {
        if (uiPortapapeles == null)
            uiPortapapeles = FindFirstObjectByType<PortapapelesUI>();

        if (uiPortapapeles == null)
            Debug.LogWarning("No se encontró PortapapelesUI.");
    }

    public void Interactuar()
    {
        if (recogido) return;
        RecogerPortapapeles();
    }

    private void RecogerPortapapeles()
    {
        recogido = true;
        gameObject.SetActive(false);

        if (uiPortapapeles != null)
            uiPortapapeles.MostrarPortapapeles(true);

        Debug.Log("Portapapeles recogido. Presiona E para soltarlo.");
    }

    // Método público para que lo llame PortapapelesUI
    public void Soltar()
    {
        if (!recogido) return;

        recogido = false;

        if (uiPortapapeles != null)
            uiPortapapeles.MostrarPortapapeles(false);

        Transform jugador = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (jugador != null)
        {
            transform.position = jugador.position + jugador.forward * 1.5f + Vector3.up * 0.5f;
            transform.rotation = Quaternion.identity;
        }

        gameObject.SetActive(true);
        Debug.Log("Portapapeles soltado.");
    }
}