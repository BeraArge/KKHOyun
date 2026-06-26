using System.Collections;
using TMPro;
using UnityEngine;


public class WarningPopupUI : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupRect;
    public TMP_Text messageText;
    public CanvasGroup messageCanvasGroup;

    [Header("Feedback Layer")]
    public CanvasGroup feedbackCanvasGroup;
    public float feedbackMaxAlpha = 0.45f;

    [Header("Animation")]
    public float openDuration = 0.22f;
    public float messageFadeDuration = 0.18f;
    public float closeDuration = 0.16f;

    private Coroutine routine;
    private bool isOpen = false;

    private void Awake()
    {
        HideInstant();
    }

    public void Show(string message)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(message));
    }

    public void Close()
    {
        if (!isOpen)
            return;

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(CloseRoutine());
    }

    private void HideInstant()
    {
        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;

        if (feedbackCanvasGroup != null)
        {
            feedbackCanvasGroup.alpha = 0f;
            feedbackCanvasGroup.blocksRaycasts = false;
            feedbackCanvasGroup.interactable = false;
        }

        popupRect.localScale = Vector3.zero;

        messageText.text = "";
        messageCanvasGroup.alpha = 0f;

        isOpen = false;
    }

    private IEnumerator ShowRoutine(string message)
    {
        if (isOpen)
        {
            yield return CloseRoutine();
        }

        messageText.text = "";
        messageCanvasGroup.alpha = 0f;

        popupCanvasGroup.blocksRaycasts = true;
        popupCanvasGroup.interactable = true;

        if (feedbackCanvasGroup != null)
        {
            feedbackCanvasGroup.blocksRaycasts = true;
            feedbackCanvasGroup.interactable = true;
        }

        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / openDuration);
            float eased = EaseOutBack(p);

            popupCanvasGroup.alpha = Mathf.Lerp(0f, 1f, p);
            popupRect.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, eased);

            if (feedbackCanvasGroup != null)
                feedbackCanvasGroup.alpha = Mathf.Lerp(0f, feedbackMaxAlpha, p);

            yield return null;
        }

        popupCanvasGroup.alpha = 1f;
        popupRect.localScale = Vector3.one;

        if (feedbackCanvasGroup != null)
            feedbackCanvasGroup.alpha = feedbackMaxAlpha;

        messageText.text = message;

        yield return FadeMessage(0f, 1f, messageFadeDuration);

        isOpen = true;
    }

    private IEnumerator CloseRoutine()
    {
        isOpen = false;

        messageCanvasGroup.alpha = 0f;

        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;

        if (feedbackCanvasGroup != null)
        {
            feedbackCanvasGroup.blocksRaycasts = false;
            feedbackCanvasGroup.interactable = false;
        }

        float t = 0f;
        Vector3 startScale = popupRect.localScale;

        float startPopupAlpha = popupCanvasGroup.alpha;
        float startFeedbackAlpha = feedbackCanvasGroup != null ? feedbackCanvasGroup.alpha : 0f;

        while (t < closeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / closeDuration);
            float eased = Mathf.SmoothStep(0f, 1f, p);

            popupCanvasGroup.alpha = Mathf.Lerp(startPopupAlpha, 0f, p);
            popupRect.localScale = Vector3.Lerp(startScale, Vector3.zero, eased);

            if (feedbackCanvasGroup != null)
                feedbackCanvasGroup.alpha = Mathf.Lerp(startFeedbackAlpha, 0f, p);

            yield return null;
        }

        HideInstant();
    }

    private IEnumerator FadeMessage(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);

            messageCanvasGroup.alpha = Mathf.Lerp(from, to, p);

            yield return null;
        }

        messageCanvasGroup.alpha = to;
    }

    private float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;

        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
    public IEnumerator ShowAndWaitForClose(string message)
    {
        Show(message);

        while (!isOpen)
            yield return null;

        while (isOpen)
            yield return null;
    }
}