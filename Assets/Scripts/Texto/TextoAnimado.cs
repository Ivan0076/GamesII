using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverTextoAnimado : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text texto; // referencia al texto TMP
    public Color normalColor = Color.white;
    public Color hoverColor = Color.cyan;
    public float normalScale = 1f;
    public float hoverScale = 1.2f;
    public float transitionSpeed = 8f;

    private bool isHovering = false;

    void Start()
    {
        texto.color = normalColor;
        texto.transform.localScale = Vector3.one * normalScale;
    }

    void Update()
    {
        // Transición suave entre escalas
        float targetScale = isHovering ? hoverScale : normalScale;
        texto.transform.localScale = Vector3.Lerp(
            texto.transform.localScale,
            Vector3.one * targetScale,
            Time.deltaTime * transitionSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        texto.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        texto.color = normalColor;
    }
}
