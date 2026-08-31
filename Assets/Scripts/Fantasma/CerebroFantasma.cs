using UnityEngine;
using System.Collections.Generic;

public enum EstadoFantasma { Ausente, Agarrando, Liberando, EsperandoJumpscare, Jumpscare }

public class CerebroFantasma : MonoBehaviour
{
    public GameObject objetoFantasma;
    public List<GameObject> objetosCubitos;
    public float tiempoAusente = 5f;
    public float tiempoEsperaLiberar = 3f;
    public float rangoLiberacion = 5f;
    public float tiempoDelayJumpscare = 4f; // ⏱ tiempo entre soltar y jumpscare
    public Transform jugador;

    private float temporizador;
    private int indiceActual = 0;
    private EstadoFantasma estadoActual = EstadoFantasma.Ausente;

    void Start()
    {
        temporizador = tiempoAusente;
        //objetoFantasma.SetActive(false);
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
                    temporizador = tiempoDelayJumpscare; // ⏱ esperar antes del jumpscare
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
                Vector3 posicionRandom = objetoFantasma.transform.position +
                    new Vector3(Random.Range(-rangoLiberacion, rangoLiberacion), 0,
                                Random.Range(-rangoLiberacion, rangoLiberacion));
                objeto.transform.position = posicionRandom;
                Debug.Log("Fantasma soltó: " + objeto.name);
            }
        }
    }

    void AparecerFrenteJugador()
    {
        if (jugador != null)
        {
            Vector3 frenteJugador = jugador.position + jugador.forward * 3f;
            objetoFantasma.transform.position = frenteJugador;
            objetoFantasma.SetActive(true);
            Debug.Log("¡Jumpscare frente al jugador!");
        }
    }
}
