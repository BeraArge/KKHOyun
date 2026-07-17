using UnityEngine;
using UnityEngine.EventSystems;

public class FoodDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Food Info")]
    public string foodName;
    public string foodCategory;

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startAnchoredPosition;

    // Doðru yere yerleþti mi?
    private bool isPlaced = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlaced)
            return;

        startAnchoredPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlaced)
            return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isPlaced)
            return;

        canvasGroup.blocksRaycasts = true;

        // Eðer herhangi bir DropZone'a býrakýlmadýysa
        // eski yerine dön.
        if (transform.parent == null ||
            transform.parent.name.StartsWith("FoodOptionsArea"))
        {
            rectTransform.anchoredPosition = startAnchoredPosition;
        }
    }

    /// <summary>
    /// Doðru DropZone tarafýndan çaðrýlýr.
    /// </summary>
    public void SnapTo(RectTransform target)
    {
        rectTransform.SetParent(target);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;

        // Artýk tekrar sürüklenemez
        isPlaced = true;

        // Raycast kapatýyoruz ki üstüne baþka obje býrakýlabilsin
        canvasGroup.blocksRaycasts = false;
    }
}