using UnityEngine;
using UnityEngine.AI;

public class GestorSpawns : MonoBehaviour
{
    [Header("Puntos de Aparición de este Enemigo")]
    public Transform[] puntosSpawn;

    public Vector3 ObtenerPuntoAleatorio()
    {
        if (puntosSpawn == null || puntosSpawn.Length == 0)
            return transform.position;

        int indice = Random.Range(0, puntosSpawn.Length);
        Vector3 posicionElegida = puntosSpawn[indice].position;

        if (NavMesh.SamplePosition(posicionElegida, out NavMeshHit hit, 20f, NavMesh.AllAreas))
            return hit.position;

        return posicionElegida;
    }
}