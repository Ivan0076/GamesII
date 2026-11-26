using UnityEngine;

public class BasureroZona : MonoBehaviour
{
    public RecogerBasuraTarea tarea;

    void Start()
    {
        if (tarea == null) tarea = FindAnyObjectByType<RecogerBasuraTarea>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tarea == null || !tarea.EstaIniciada || tarea.EstaCompletada) return;

        BasuraRecogible basura = other.GetComponentInParent<BasuraRecogible>();
        if (basura == null) return;

        if (tarea.DepositarBasura(basura.gameObject))
        {
            basura.SoltarPorMouse();
            Destroy(basura.gameObject);
        }
    }
}