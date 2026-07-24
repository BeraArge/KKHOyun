using UnityEngine;
using UnityEngine.EventSystems;

public class EducationInputBlocker :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        eventData.Use();
    }

    public void OnPointerUp(
        PointerEventData eventData
    )
    {
        eventData.Use();
    }

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        eventData.Use();
    }

    private void OnMouseDown()
    {
        // Physics2D veya OnMouseDown kullanan
        // arka plan nesnelerine týklamanýn geçmesini engeller.
    }
}