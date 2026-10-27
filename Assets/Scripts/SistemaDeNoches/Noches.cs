using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int nocheActual = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("GameManager creado");
        }
        else
        {
            Destroy(gameObject);
        }

    }
}