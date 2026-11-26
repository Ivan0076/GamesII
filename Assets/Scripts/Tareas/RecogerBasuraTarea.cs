using System.Collections.Generic;
using UnityEngine;

public class RecogerBasuraTarea : TareaBase
{
    [Header("Objetos de basura en la escena (opcional si se auto-registran)")]
    public List<GameObject> basuraPendiente = new List<GameObject>();

    public int Restantes => basuraPendiente.Count;

    public override void IniciarTarea()
    {
        base.IniciarTarea();
        if (EstaIniciada)
        {
            NotificarProgreso();
        }
    }

    public void RegistrarBasura(GameObject objetoBasura)
    {
        if (objetoBasura == null || completada) return;
        if (!basuraPendiente.Contains(objetoBasura))
        {
            basuraPendiente.Add(objetoBasura);
            if (EstaIniciada)
            {
                NotificarProgreso();
            }
        }
    }

    public bool DepositarBasura(GameObject objetoBasura)
    {
        if (!EstaIniciada || completada) return false;
        if (objetoBasura == null) return false;

        if (!basuraPendiente.Contains(objetoBasura)) return false;

        basuraPendiente.Remove(objetoBasura);
        NotificarProgreso();

        if (basuraPendiente.Count <= 0)
        {
            basuraPendiente.Clear();
            CompletarTarea();
        }

        return true;
    }

    public override string ObtenerProgreso()
    {
        return $"{basuraPendiente.Count} restantes";
    }
}