using UnityEngine;
using System.Collections.Generic;

public enum EstadoFantasma { Ausente, Agarrando, Liberando, EsperandoJumpscare, Jumpscare, Manos }

public class CerebroFantasma : MonoBehaviour
{
    public GameObject objetoFantasma;
    public List<GameObject> objetosCubitos;
    public GameObject manoIzquierda;
    public GameObject manoDerecha;
    public float tiempoAusente = 5f;
    public float tiempoEsperaLiberar = 3f;
    public float rangoLiberacion = 5f;
    public float tiempoDelayJumpscare = 4f;
    public float distanciaJumpscare = 4f; // más distancia para evitar empuje
    public Transform jugador;
    public bool regresarAlOrigen = true;
    public float tiempoDelayManos = 2f; // ⏱ tiempo entre jumpscare y manos


    private float temporizador;
    private int indiceActual = 0;
    private EstadoFantasma estadoActual = EstadoFantasma.Ausente;

    private Dictionary<GameObject, Vector3> posicionesOriginales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        temporizador = tiempoAusente;

        foreach (GameObject obj in objetosCubitos)
        {
            if (obj != null && !posicionesOriginales.ContainsKey(obj))
            {
                posicionesOriginales.Add(obj, obj.transform.position);
            }
        }

        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoFantasma.Ausente:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0)
                {
                    objetoFantasma.SetActive(true);
                    AgarrarObjeto(objetosCubitos[indiceActual]);
                    indiceActual++;
                    temporizador = tiempoAusente;
                    estadoActual = EstadoFantasma.Agarrando;
                }
                break;

            case EstadoFantasma.Agarrando:
                if (indiceActual < objetosCubitos.Count)
                {
                    temporizador -= Time.deltaTime;
                    if (temporizador <= 0)
                    {
                        AgarrarObjeto(objetosCubitos[indiceActual]);
                        indiceActual++;
                        temporizador = tiempoAusente;
                    }
                }
                else
                {
                    estadoActual = EstadoFantasma.Liberando;
                    temporizador = tiempoEsperaLiberar;
                }
                break;

            case EstadoFantasma.Liberando:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0)
                {
                    SoltarObjetos();
                    estadoActual = EstadoFantasma.EsperandoJumpscare;
                    temporizador = tiempoDelayJumpscare;
                }
                break;

            case EstadoFantasma.EsperandoJumpscare:
                temporizador -= Time.deltaTime;
                if (temporizador <= 0)
                {
                    estadoActual = EstadoFantasma.Jumpscare;
                }
                break;

            case EstadoFantasma.Jumpscare:
                AparecerFrenteJugador();
                Invoke("MoverFantasma", 1f); // se mueve rápido después de aparecer
                Invoke("ActivarManos", tiempoDelayManos); // ⏱ manos aparecen después del jumpscare
                estadoActual = EstadoFantasma.Manos;
                break;


            case EstadoFantasma.Manos:
                // Aquí ya no llamamos a ActivarManos directamente
                break;

        }
    }

    void AgarrarObjeto(GameObject objeto)
    {
        if (objeto != null)
        {
            objeto.transform.SetParent(objetoFantasma.transform);
            objeto.transform.localPosition = Vector3.zero;
            Debug.Log("Fantasma agarró: " + objeto.name);
        }
    }

    void SoltarObjetos()
    {
        foreach (GameObject objeto in objetosCubitos)
        {
            if (objeto != null)
            {
                objeto.transform.SetParent(null);

                if (regresarAlOrigen && posicionesOriginales.ContainsKey(objeto))
                {
                    objeto.transform.position = posicionesOriginales[objeto];
                    Debug.Log("Fantasma soltó (origen): " + objeto.name);
                }
                else
                {
                    Vector3 posicionRandom = objetoFantasma.transform.position +
                        new Vector3(Random.Range(-rangoLiberacion, rangoLiberacion), 0,
                                    Random.Range(-rangoLiberacion, rangoLiberacion));
                    objeto.transform.position = posicionRandom;
                    Debug.Log("Fantasma soltó (random): " + objeto.name);
                }
            }
        }
    }

    void AparecerFrenteJugador()
    {
        if (jugador != null)
        {
            Vector3 frenteJugador = jugador.position + jugador.forward * distanciaJumpscare + Vector3.up * 1f;
            objetoFantasma.transform.position = frenteJugador;
            objetoFantasma.SetActive(true);
            Debug.Log("¡Jumpscare frente al jugador!");
        }
    }

    void MoverFantasma()
    {
        objetoFantasma.SetActive(false);
        objetoFantasma.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        Debug.Log("Fantasma se movió a otra posición");
    }

    
    void ActivarManos()
    {
        if (jugador != null)
        {
            if (manoIzquierda != null)
            {
                manoIzquierda.SetActive(true);
                manoIzquierda.transform.SetParent(jugador);
                manoIzquierda.transform.localPosition = new Vector3(-0.4f, 0.2f, 1f);
            }
            if (manoDerecha != null)
            {
                manoDerecha.SetActive(true);
                manoDerecha.transform.SetParent(jugador);
                manoDerecha.transform.localPosition = new Vector3(0.4f, 0.2f, 1f);
            }
            Debug.Log("Manos aparecieron frente al jugador con delay");
        }
    }

}
