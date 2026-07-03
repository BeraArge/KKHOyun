using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePromptUI : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupRect;
    public TMP_Text messageText;
    public Button button1;
    public TMP_Text button1Label;
    public Button button2;
    public TMP_Text button2Label;

    [Header("Feedback Layer")]
    public CanvasGroup feedbackCanvasGroup;
    public float feedbackMaxAlpha = 0.45f;

    [Header("Animation")]
    public float openDuration = 0.22f;
    public float closeDuration = 0.16f;

    private Coroutine activeRoutine;
    private bool isOpen = false;

    private void Awake()
    {
        HideInstant();
    }

    public void ShowTwoButton(string message, string btn1Text, Action onBtn1, string btn2Text, Action onBtn2)
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        button1Label.text = btn1Text;
        button2Label.text = btn2Text;

        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(() =>
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(CloseAndInvoke(onBtn1));
        });

        button2.onClick.RemoveAllListeners();
        button2.onClick.AddListener(() =>
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(CloseAndInvoke(onBtn2));
        });

        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(OpenRoutine(message));
    }

    public void ShowOneButton(string message, string btnText, Action onBtn)
    {
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(false);
        button1Label.text = btnText;

        button1.onClick.RemoveAllListeners();
        button1.onClick.AddListener(() =>
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = StartCoroutine(CloseAndInvoke(onBtn));
        });

        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(OpenRoutine(message));
    }

    private IEnumerator CloseAndInvoke(Action callback)
    {
        yield return CloseRoutine();
        callback?.Invoke();
    }

    private IEnumerator OpenRoutine(string message)
    {
        if (isOpen)
            yield return CloseRoutine();

        messageText.text = message;
        popupCanvasGroup.blocksRaycasts = true;
        popupCanvasGroup.interactable = true;
        SetFeedback(true, 0f);

        float t = 0f;
        while (t < openDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / openDuration);
            popupCanvasGroup.alpha = p;
            popupRect.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, EaseOutBack(p));
            SetFeedbackAlpha(Mathf.Lerp(0f, feedbackMaxAlpha, p));
            yield return null;
        }

        popupCanvasGroup.alpha = 1f;
        popupRect.localScale = Vector3.one;
        SetFeedbackAlpha(feedbackMaxAlpha);
        isOpen = true;
    }

    private IEnumerator CloseRoutine()
    {
        isOpen = false;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;
        SetFeedback(false, 0f);

        Vector3 startScale = popupRect.localScale;
        float startAlpha = popupCanvasGroup.alpha;
        float startFeedbackAlpha = feedbackCanvasGroup != null ? feedbackCanvasGroup.alpha : 0f;

        float t = 0f;
        while (t < closeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / closeDuration);
            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, p);
            popupRect.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, p));
            SetFeedbackAlpha(Mathf.Lerp(startFeedbackAlpha, 0f, p));
            yield return null;
        }

        HideInstant();
    }

    private void HideInstant()
    {
        popupCanvasGroup.alpha = 0f;
        popupCanvasGroup.blocksRaycasts = false;
        popupCanvasGroup.interactable = false;
        popupRect.localScale = Vector3.zero;
        SetFeedback(false, 0f);
        isOpen = false;
    }

    private void SetFeedback(bool blocks, float alpha)
    {
        if (feedbackCanvasGroup == null) return;
        feedbackCanvasGroup.blocksRaycasts = blocks;
        feedbackCanvasGroup.interactable = blocks;
        feedbackCanvasGroup.alpha = alpha;
    }

    private void SetFeedbackAlpha(float alpha)
    {
        if (feedbackCanvasGroup != null)
            feedbackCanvasGroup.alpha = alpha;
    }

    private float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
