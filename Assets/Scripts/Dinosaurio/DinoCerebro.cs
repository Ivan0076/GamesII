using UnityEngine;
using System.Collections;

public class DinoCerebro : MonoBehaviour
{
    [Header("Ruta del dinosaurio")]
    public PuntosDino[] puntos;

    [Header("Altura sobre los puntos")]
    public float altura = 2f;

    [Header("Materiales por estado")]
    public Material materialInicio;
    public Material materialPeligro;
    public Material materialSeguro;

    private int puntoActual = 0;
    private Renderer dinoRenderer;

    public PuntosDino.TipoPunto EstadoActual
    {
        get
        {
            if (puntos == null || puntos.Length == 0)
                return PuntosDino.TipoPunto.Inicio;

            return puntos[puntoActual].tipo;
        }
    }

    private void Start()
    {
        dinoRenderer = GetComponent<Renderer>();

        if (puntos.Length == 0)
        {
            Debug.LogWarning("El dinosaurio no tiene puntos asignados.");
            return;
        }

        StartCoroutine(RecorrerRuta());
    }

    private IEnumerator RecorrerRuta()
    {
        for (puntoActual = 0; puntoActual < puntos.Length; puntoActual++)
        {
            PuntosDino punto = puntos[puntoActual];

            CambiarMaterial(punto.tipo);

            Vector3 nuevaPosicion = punto.transform.position;
            nuevaPosicion.y += altura;

            transform.position = nuevaPosicion;

            yield return new WaitForSeconds(punto.tiempoEspera);
        }

        Debug.Log("¡El dinosaurio terminó su recorrido!");
    }

    private void CambiarMaterial(PuntosDino.TipoPunto tipo)
    {
        switch (tipo)
        {
            case PuntosDino.TipoPunto.Inicio:
                dinoRenderer.material = materialInicio;
                break;

            case PuntosDino.TipoPunto.Peligro:
                dinoRenderer.material = materialPeligro;
                break;

            case PuntosDino.TipoPunto.Seguro:
                dinoRenderer.material = materialSeguro;
                break;
        }
    }

    public void ReiniciarRuta()
    {
        StopAllCoroutines();

        puntoActual = 0;

        PuntosDino punto = puntos[puntoActual];

        CambiarMaterial(punto.tipo);

        Vector3 nuevaPosicion = punto.transform.position;
        nuevaPosicion.y += altura;

        transform.position = nuevaPosicion;

        StartCoroutine(RecorrerRuta());
    }
}