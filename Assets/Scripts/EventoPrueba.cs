using UnityEngine;
using UnityEngine.Events;

public class EventoPrueba : MonoBehaviour
{
    public UnityEvent eventoPrueba;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eventoPrueba.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
