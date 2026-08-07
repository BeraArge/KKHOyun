using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ResponsiveScreenScaler : MonoBehaviour
{
    [Header("Tasarým Çözünürlüðü")]
    [SerializeField]
    private Vector2 referenceResolution =
        new Vector2(2560f, 1440f);

    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    private Vector2 lastParentSize;

    private void Awake()
    {
        Initialize();
        ApplyScale();
    }

    private void OnEnable()
    {
        Initialize();
        ApplyScale();
    }

    private void Update()
    {
        if (rectTransform == null ||
            parentRectTransform == null)
        {
            Initialize();
        }

        if (parentRectTransform == null)
        {
            return;
        }

        Vector2 currentParentSize =
            parentRectTransform.rect.size;

        if (currentParentSize != lastParentSize)
        {
            ApplyScale();
        }
    }

    private void Initialize()
    {
        rectTransform =
            GetComponent<RectTransform>();

        parentRectTransform =
            transform.parent as RectTransform;
    }

    private void ApplyScale()
    {
        if (rectTransform == null ||
            parentRectTransform == null)
        {
            return;
        }

        Vector2 parentSize =
            parentRectTransform.rect.size;

        if (parentSize.x <= 0f ||
            parentSize.y <= 0f)
        {
            return;
        }

        float scaleX =
            parentSize.x /
            referenceResolution.x;

        float scaleY =
            parentSize.y /
            referenceResolution.y;

        rectTransform.localScale =
            new Vector3(
                scaleX,
                scaleY,
                1f
            );

        rectTransform.anchorMin =
            new Vector2(0.5f, 0.5f);

        rectTransform.anchorMax =
            new Vector2(0.5f, 0.5f);

        rectTransform.pivot =
            new Vector2(0.5f, 0.5f);

        rectTransform.anchoredPosition =
            Vector2.zero;

        rectTransform.sizeDelta =
            referenceResolution;

        lastParentSize =
            parentSize;
    }
}