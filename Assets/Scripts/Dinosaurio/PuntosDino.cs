using UnityEngine;

public class PuntosDino : MonoBehaviour
{
    public enum TipoPunto
    {
        Inicio,
        Peligro,
        Seguro
    }

    [Header("Configuración del punto")]
    public TipoPunto tipo = TipoPunto.Seguro;

    [Header("Tiempo de permanencia")]
    public float tiempoEspera = 3f;
}