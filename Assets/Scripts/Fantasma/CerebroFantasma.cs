using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;

public class CerebroFantasma : MonoBehaviour
{
    [Header("Fantasma")]
    public GameObject objetoFantasma;
    public Animator animatorFantasma;

    [Header("Conexión con Tarea")]
    public RecogerObjetoPerdidos tareaObjetos;

    [Header("Probabilidad de Robo")]
    [Range(0, 100)] public int probabilidadRobar = 100;

    [Header("Sistema de Pánico")]
    public SistemaPanico sistemaPanico;
    public float panicoQueAumenta = 25f;

    [Header("Manos (Tapar Vista)")]
    public GameObject manoIzquierda;
    public GameObject manoDerecha;
    public float tiempoManosActivas = 3f; // Cuánto tiempo te tapa la vista

    [Header("Tiempos de la Secuencia")]
    public float tiempoEsperaInicial = 4f;
    public float tiempoEntreRoboYJumpscare = 3f;
    public float duracionJumpscare = 1.5f;

    [Header("Jumpscare")]
    public float distanciaJumpscare = 1.2f;
    public AudioSource audioSourceJumpscare;
    public AudioClip sonidoJumpscare;

    public System.Action OnFantasmaTermino;

    private bool enCiclo = false; // 🔥 Candado para evitar ejecuciones dobles

    void Awake()
    {
        DetenerYLimpiar();
    }

    public void IniciarCiclo()
    {
        if (enCiclo)
        {
            Debug.LogWarning("⚠️ El fantasma ya está en ciclo. Ignorando llamada doble.");
            return;
        }

        enCiclo = true;
        StopAllCoroutines();

        if (objetoFantasma != null) objetoFantasma.SetActive(false);
        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);

        StartCoroutine(SecuenciaCicloCompleto());
    }

    public void DetenerYLimpiar()
    {
        StopAllCoroutines();
        enCiclo = false;

        if (objetoFantasma != null) objetoFantasma.SetActive(false);
        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);
    }

    IEnumerator SecuenciaCicloCompleto()
    {
        yield return new WaitForSeconds(tiempoEsperaInicial);

        // 1. ROBAR
        DeterminarSiRobara();

        // 2. ESPERAR 3 SEGUNDOS
        yield return new WaitForSeconds(tiempoEntreRoboYJumpscare);

        // 3. JUMPSCARE
        EjecutarJumpscare();
        yield return new WaitForSeconds(duracionJumpscare);

        // 4. TAPAR LA VISIÓN (UNA SOLA VEZ)
        if (objetoFantasma != null) objetoFantasma.SetActive(false);

        if (manoIzquierda != null) manoIzquierda.SetActive(true);
        if (manoDerecha != null) manoDerecha.SetActive(true);

        yield return new WaitForSeconds(tiempoManosActivas);

        if (manoIzquierda != null) manoIzquierda.SetActive(false);
        if (manoDerecha != null) manoDerecha.SetActive(false);

        // 5. FIN DEL CICLO
        enCiclo = false;
        OnFantasmaTermino?.Invoke();
        Debug.Log("Fantasma: Ciclo terminado.");
    }

    void EjecutarJumpscare()
    {
        if (sistemaPanico != null) sistemaPanico.AumentarPanico(panicoQueAumenta);

        if (objetoFantasma != null)
        {
            objetoFantasma.transform.localPosition = new Vector3(0, 0, distanciaJumpscare);
            objetoFantasma.transform.localRotation = Quaternion.identity;
            objetoFantasma.SetActive(true);
        }

        if (audioSourceJumpscare != null && sonidoJumpscare != null) audioSourceJumpscare.PlayOneShot(sonidoJumpscare);
        if (animatorFantasma != null) animatorFantasma.SetTrigger("Jumpscare");
    }

    void DeterminarSiRobara()
    {
        if (tareaObjetos == null || tareaObjetos.objetosRecuperados.Count == 0) return;

        int dado = Random.Range(0, 100);
        if (dado < probabilidadRobar)
        {
            EjecutarRobo();
        }
    }

    void EjecutarRobo()
    {
        int indiceAleatorio = Random.Range(0, tareaObjetos.objetosRecuperados.Count);
        GameObject objetoA_Devolver = tareaObjetos.objetosRecuperados[indiceAleatorio];

        tareaObjetos.objetosRecuperados.RemoveAt(indiceAleatorio);
        tareaObjetos.objetos.Add(objetoA_Devolver);

        ObjetoRecogible scriptObjeto = objetoA_Devolver.GetComponent<ObjetoRecogible>();
        if (scriptObjeto != null) scriptObjeto.DevolverAlOrigen();

        XRGrabInteractable grab = objetoA_Devolver.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = true;

        tareaObjetos.ActualizarProgresoExterno();
        Debug.Log($"Fantasma: ¡Ha robado '{objetoA_Devolver.name}'!");
    }
}