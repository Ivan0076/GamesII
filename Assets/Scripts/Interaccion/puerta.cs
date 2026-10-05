using UnityEngine;

public class Puerta : MonoBehaviour, IInteractuable
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    private bool estaAbierta = false;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Interactuar()
    {
        if (!estaAbierta)
        {
            animator.SetTrigger("abrir");
            estaAbierta = true;
        }
        else
        {
            animator.SetTrigger("cerrar");
            estaAbierta = false;
        }
    }
}