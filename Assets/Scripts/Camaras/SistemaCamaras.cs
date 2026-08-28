using System.Collections.Generic;
using UnityEngine;

public class SistemaCamaras : MonoBehaviour
{
    public List<GameObject> camaras;
    public int camaraSeleccionada;
    public SistemaCamaras other_button;
    //public Animator buttonAnim;

    public void siguienteCam()
    {
        camaraSeleccionada = camaraSeleccionada + 1;
        //buttonAnim.Play("buttonpress");
        if (camaraSeleccionada > camaras.Count - 1)
        {
            camaraSeleccionada = 0;
        }
        if (camaraSeleccionada > 0)
        {
            camaras[camaraSeleccionada - 1].SetActive(false);
        }
        if (camaraSeleccionada == 0)
        {
            camaras[camaras.Count - 1].SetActive(false);
        }
        camaras[camaraSeleccionada].SetActive(true);
        other_button.camaraSeleccionada = camaraSeleccionada;
        Debug.Log(camaraSeleccionada);
    }
    public void anteriorCam()
    {
        camaraSeleccionada = camaraSeleccionada - 1;
        //buttonAnim.Play("buttonpress");
        if (camaraSeleccionada < 0)
        {
            camaraSeleccionada = camaras.Count - 1;
        }
        if (camaraSeleccionada == camaras.Count - 1)
        {
            camaras[0].SetActive(false);
        }
        if (camaraSeleccionada < camaras.Count - 1)
        {
            camaras[camaraSeleccionada + 1].SetActive(false);
        }
        camaras[camaraSeleccionada].SetActive(true);
        other_button.camaraSeleccionada = camaraSeleccionada;
        Debug.Log(camaraSeleccionada);
    }
}