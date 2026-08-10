using UnityEngine;
using TMPro;

public class KeyboardUIShift : MonoBehaviour
{
    [Header("Kaydýrýlacak UI")]
    [SerializeField] private RectTransform content;

    [Header("Ayarlar")]
    [SerializeField] private float moveUpAmount = 300f;
    [SerializeField] private float smoothSpeed = 12f;

    private Vector2 originalPosition;
    private Vector2 targetPosition;

    private TMP_InputField[] inputFields;

    private void Start()
    {
        if (content == null)
            content = GetComponent<RectTransform>();

        originalPosition = content.anchoredPosition;
        targetPosition = originalPosition;

        // ResponsiveRoot altýndaki TÜM TMP InputField'larý bul
        inputFields = GetComponentsInChildren<TMP_InputField>(true);

        foreach (TMP_InputField input in inputFields)
        {
            input.onSelect.AddListener(OnInputSelected);
            input.onDeselect.AddListener(OnInputDeselected);
            input.onEndEdit.AddListener(OnInputEndEdit);
        }

        Debug.Log("KeyboardUIShift hazýr. Bulunan input sayýsý: " + inputFields.Length);
    }

    private void Update()
    {
        content.anchoredPosition = Vector2.Lerp(
            content.anchoredPosition,
            targetPosition,
            Time.unscaledDeltaTime * smoothSpeed
        );
    }

    private void OnInputSelected(string value)
    {
        Debug.Log("Input seçildi - UI yukarý çýkýyor");

        targetPosition =
            originalPosition + new Vector2(0f, moveUpAmount);
    }

    private void OnInputDeselected(string value)
    {
        Debug.Log("Input býrakýldý - UI geri dönüyor");

        targetPosition = originalPosition;
    }

    private void OnInputEndEdit(string value)
    {
        targetPosition = originalPosition;
    }

    private void OnDestroy()
    {
        if (inputFields == null)
            return;

        foreach (TMP_InputField input in inputFields)
        {
            if (input == null)
                continue;

            input.onSelect.RemoveListener(OnInputSelected);
            input.onDeselect.RemoveListener(OnInputDeselected);
            input.onEndEdit.RemoveListener(OnInputEndEdit);
        }
    }
}