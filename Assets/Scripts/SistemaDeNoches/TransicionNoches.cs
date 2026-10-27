using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransicionNoches : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(
            "CineNivel" + GameManager.Instance.nocheActual
        );
    }
}