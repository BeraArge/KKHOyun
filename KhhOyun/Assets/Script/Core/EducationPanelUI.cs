using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EducationPanelUI : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public string id;
        [TextArea]
        public string message;
        public AudioClip audioClip;
    }

    [Header("References")]
    public CanvasGroup popupCanvasGroup;
    public RectTransform popupRect;
    public TMP_Text messageText;
    public CanvasGroup messageCanvasGroup;
    public AudioSource audioSource;

    [Header("Adımlar (id ile çağrılır — bu sahnede kaç adım gerekiyorsa listeye eklenir)")]
    public Step[] steps;

    [Header("Hemşire Animasyonu (ses çalarken kare kare oynar, ses bitince olduğu karede durur)")]
    public Image nurseImage;
    public Sprite[] nurseFrames;
    public float nurseFrameRate = 10f;

    [Header("Devam Et Butonu (ses bitmeden gizli, ses bitince görünür)")]
    public GameObject continueButton;

    [Header("Feedback Layer")]
    public CanvasGroup feedbackCanvasGroup;
    public float feedbackMaxAlpha = 0.45f;

    [Header("Animation")]
    public float openDuration = 0.22f;
    public float messageFadeDuration = 0.18f;
    public float closeDuration = 0.16f;

    private Coroutine routine;
    private Coroutine nurseAnimationRoutine;
    private Coroutine continueButtonRoutine;
    private bool isOpen = false;

    private void Awake()
    {
        HideInstant();
    }

    public void Show(string message, AudioClip clip = null)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(message, clip));
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

        if (audioSource != null)
            audioSource.Stop();

        StopNurseAnimation();

        if (nurseImage != null && nurseFrames != null && nurseFrames.Length > 0)
            nurseImage.sprite = nurseFrames[0];

        if (continueButton != null)
            continueButton.SetActive(false);

        isOpen = false;
    }

    private void StopNurseAnimation()
    {
        if (nurseAnimationRoutine != null)
        {
            StopCoroutine(nurseAnimationRoutine);
            nurseAnimationRoutine = null;
        }

        if (continueButtonRoutine != null)
        {
            StopCoroutine(continueButtonRoutine);
            continueButtonRoutine = null;
        }
    }

    private IEnumerator ShowRoutine(string message, AudioClip clip)
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

        if (nurseImage != null && nurseFrames != null && nurseFrames.Length > 0)
            nurseImage.sprite = nurseFrames[0];

        if (continueButton != null)
            continueButton.SetActive(false);

        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            nurseAnimationRoutine = StartCoroutine(NurseAnimationRoutine());
            continueButtonRoutine = StartCoroutine(WaitForAudioEndThenShowButton());
        }
        else if (continueButton != null)
        {
            continueButton.SetActive(true);
        }

        isOpen = true;
    }

    private IEnumerator NurseAnimationRoutine()
    {
        if (nurseImage == null || nurseFrames == null || nurseFrames.Length == 0)
            yield break;

        int frameIndex = 0;
        float frameDuration = nurseFrameRate > 0f ? 1f / nurseFrameRate : 0.1f;
        float timer = 0f;

        while (audioSource != null && audioSource.isPlaying)
        {
            timer += Time.deltaTime;

            if (timer >= frameDuration)
            {
                timer -= frameDuration;
                frameIndex = (frameIndex + 1) % nurseFrames.Length;
                nurseImage.sprite = nurseFrames[frameIndex];
            }

            yield return null;
        }
    }

    private IEnumerator WaitForAudioEndThenShowButton()
    {
        yield return null;

        while (audioSource != null && audioSource.isPlaying)
            yield return null;

        if (continueButton != null)
            continueButton.SetActive(true);
    }

    private IEnumerator CloseRoutine()
    {
        isOpen = false;

        if (audioSource != null)
            audioSource.Stop();

        StopNurseAnimation();

        if (continueButton != null)
            continueButton.SetActive(false);

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

    public IEnumerator ShowAndWaitForClose(string message, AudioClip clip = null)
    {
        Show(message, clip);

        while (!isOpen)
            yield return null;

        while (isOpen)
            yield return null;
    }

    public void ShowStep(string id)
    {
        Step step = FindStep(id);
        if (step == null)
            return;

        Show(step.message, step.audioClip);
    }

    public IEnumerator ShowStepAndWaitForClose(string id)
    {
        Step step = FindStep(id);
        if (step == null)
        {
            Debug.LogWarning("EducationPanelUI: '" + id + "' id'li adım bulunamadı.");
            yield break;
        }

        yield return ShowAndWaitForClose(step.message, step.audioClip);
    }

    private Step FindStep(string id)
    {
        if (steps == null)
            return null;

        foreach (Step step in steps)
        {
            if (step != null && step.id == id)
                return step;
        }

        return null;
    }
}
