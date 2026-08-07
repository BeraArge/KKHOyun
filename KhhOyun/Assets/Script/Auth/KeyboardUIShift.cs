using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class KeyboardUIShift : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private RectTransform content;
    [SerializeField] private Canvas canvas;

    [Header("Ayarlar")]
    [SerializeField] private float extraPadding = 40f;
    [SerializeField] private float smoothSpeed = 12f;

    private Vector2 originalPosition;

    private void Awake()
    {
        if (content == null)
            content = GetComponent<RectTransform>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        originalPosition = content.anchoredPosition;
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS

        if (TouchScreenKeyboard.visible)
        {
            MoveForSelectedInput();
        }
        else
        {
            ReturnToOriginalPosition();
        }

#endif
    }

    private void MoveForSelectedInput()
    {
        if (EventSystem.current == null)
            return;

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            return;

        TMP_InputField inputField =
            selectedObject.GetComponent<TMP_InputField>();

        if (inputField == null)
            inputField = selectedObject.GetComponentInParent<TMP_InputField>();

        if (inputField == null)
            return;

        RectTransform inputRect =
            inputField.GetComponent<RectTransform>();

        // Klavyenin ekranda kapladýðý alan
        float keyboardHeight =
            TouchScreenKeyboard.area.height;

        if (keyboardHeight <= 0)
            return;

        Camera uiCamera = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            uiCamera = canvas.worldCamera;

        Vector3[] corners = new Vector3[4];
        inputRect.GetWorldCorners(corners);

        // Input'un ekran üzerindeki alt noktasý
        Vector2 inputBottom =
            RectTransformUtility.WorldToScreenPoint(
                uiCamera,
                corners[0]
            );

        float currentOffset =
            content.anchoredPosition.y -
            originalPosition.y;

        /*
         * UI þu anda yukarý taþýnmýþ olabileceðinden
         * input'un orijinal ekran konumunu hesaplýyoruz.
         */
        float originalInputBottom =
            inputBottom.y -
            (currentOffset * canvas.scaleFactor);

        float requiredScreenY =
            keyboardHeight + extraPadding;

        float overlap =
            requiredScreenY - originalInputBottom;

        float desiredShift = 0f;

        if (overlap > 0)
        {
            desiredShift =
                overlap / canvas.scaleFactor;
        }

        Vector2 targetPosition =
            originalPosition +
            new Vector2(0, desiredShift);

        content.anchoredPosition =
            Vector2.Lerp(
                content.anchoredPosition,
                targetPosition,
                Time.unscaledDeltaTime * smoothSpeed
            );
    }

    private void ReturnToOriginalPosition()
    {
        content.anchoredPosition =
            Vector2.Lerp(
                content.anchoredPosition,
                originalPosition,
                Time.unscaledDeltaTime * smoothSpeed
            );

        if (Vector2.Distance(
                content.anchoredPosition,
                originalPosition) < 0.5f)
        {
            content.anchoredPosition =
                originalPosition;
        }
    }
}