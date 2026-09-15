using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CambiarColorTexto : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMP_Text texto; // referencia al texto TMP
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    public Color clickColor = Color.red;

    void Start()
    {
        texto.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        texto.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        texto.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        texto.color = clickColor;
    }
}
