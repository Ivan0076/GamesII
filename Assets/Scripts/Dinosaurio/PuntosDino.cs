using UnityEngine;

public class PuntosDino : MonoBehaviour
{
    public enum TipoPunto
    {
        Inicio,
        Peligro,
        Seguro,
        Final      
    }

    [Header("Configuración del punto")]
    public TipoPunto tipo = TipoPunto.Seguro;

    [Header("Tiempo de permanencia")]
    public float tiempoEspera = 3f;
}