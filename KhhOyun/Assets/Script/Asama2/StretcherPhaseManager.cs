using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StretcherPhaseManager : MonoBehaviour
{
    [Header("References")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;
    public DialoguePromptUI dialoguePrompt;

    [Header("Karakter")]
    public GameObject characterDefault;
    public GameObject characterPhaseD;

    [Header("Arka Plan")]
    public GameObject backgroundDefault;
    public GameObject backgroundPhaseD;
    public GameObject backgroundSurgery;

    [Header("UI")]
    public GameObject messageObject;
    public GameObject breathingPanel;
    public RectTransform breathingCircle;
    public TMP_Text breathingCountdownText;
    public Image screenOverlay;

    [Header("Circle Boyutları")]
    public float circleMinScale = 0.6f;
    public float circleMaxScale = 1.0f;

    public void StartPhase()
    {
        if (characterDefault != null) characterDefault.SetActive(false);
        if (characterPhaseD != null) characterPhaseD.SetActive(true);
        if (backgroundDefault != null) backgroundDefault.SetActive(false);
        if (backgroundPhaseD != null) backgroundPhaseD.SetActive(true);
        StartCoroutine(PhaseDRoutine());
    }

    private IEnumerator PhaseDRoutine()
    {
        if (messageObject != null)
        {
            messageObject.SetActive(true);
            var tmp = messageObject.GetComponent<TMP_Text>();
            if (tmp != null) tmp.text = "Sedyeyle ilerlerken nefes egzersizini yapmayı unutma.";
        }

        yield return new WaitForSeconds(2f);
        yield return LightsTransitionRoutine();

        ShowBreathingChoice();
    }

    private void ShowBreathingChoice()
    {
        dialoguePrompt.ShowTwoButton(
            "Sakin kalmak için ne yaparsın?",
            "Derin nefes al", OnDeepBreathSelected,
            "Panikle", OnPanicSelected
        );
    }

    private void OnDeepBreathSelected()
    {
        StartCoroutine(BreathingAndSleepRoutine());
    }

    private void OnPanicSelected()
    {
        if (stage2Manager != null) stage2Manager.AddScore(-15);
        StartCoroutine(PanicRoutine());
    }

    private IEnumerator PanicRoutine()
    {
        yield return warningPopup.ShowAndWaitForClose("Paniklemek yardımcı olmaz! Sakin kalmayı dene.");
        ShowBreathingChoice();
    }

    private IEnumerator BreathingAndSleepRoutine()
    {
        if (breathingPanel != null) breathingPanel.SetActive(true);

        Vector3 small = Vector3.one * circleMinScale;
        Vector3 large = Vector3.one * circleMaxScale;

        if (breathingCircle != null) breathingCircle.localScale = small;

        yield return BreathingStep(4, small, large);   // Nefes al: büyü
        yield return BreathingStep(5, large, large);   // Tut: sabit
        yield return BreathingStep(8, large, small);   // Ver: küçül

        if (breathingPanel != null) breathingPanel.SetActive(false);

        yield return warningPopup.ShowAndWaitForClose("Çok iyi! Sakin kalıyorsun.");
        yield return SleepFadeRoutine();
        yield return warningPopup.ShowAndWaitForClose("Şimdi biraz uyuyacaksın… Uyandığında her şey bitmiş olacak.");

        yield return FadeOverlay(Color.black, 1f, 0f, 1f);
        if (backgroundPhaseD != null) backgroundPhaseD.SetActive(false);
        if (backgroundSurgery != null) backgroundSurgery.SetActive(true);
        if (characterPhaseD != null) characterPhaseD.SetActive(false);
        if (messageObject != null) messageObject.SetActive(false);

        yield return warningPopup.ShowAndWaitForClose("Ameliyat başladı…");

        int finalScore = stage2Manager != null ? stage2Manager.score : 0;
        if (finalScore < 70)
        {
            yield return warningPopup.ShowAndWaitForClose(
                $"Puanın: {finalScore}. En az 70 puan gerekiyor. Tekrar deneyelim!");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        else
        {
            yield return warningPopup.ShowAndWaitForClose("Harika iş çıkardın! Çok cesurdun. ❤️");
            if (stage2Manager != null) stage2Manager.OnPhaseDComplete();
        }
    }

    private IEnumerator BreathingStep(int seconds, Vector3 fromScale, Vector3 toScale)
    {
        float duration = seconds;
        float elapsed = 0f;
        int lastShown = -1;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            if (breathingCircle != null)
                breathingCircle.localScale = Vector3.Lerp(fromScale, toScale, Mathf.SmoothStep(0f, 1f, t));

            int remaining = Mathf.CeilToInt(duration - elapsed);
            if (remaining != lastShown)
            {
                lastShown = remaining;
                if (breathingCountdownText != null)
                    breathingCountdownText.text = remaining.ToString();
            }

            yield return null;
        }

        if (breathingCircle != null) breathingCircle.localScale = toScale;
    }

    private IEnumerator LightsTransitionRoutine()
    {
        yield return FadeOverlay(Color.white, 0f, 0.85f, 0.4f);
        yield return new WaitForSeconds(0.2f);
        yield return FadeOverlay(Color.white, 0.85f, 0f, 0.5f);
    }

    private IEnumerator SleepFadeRoutine()
    {
        yield return FadeOverlay(Color.black, 0f, 1f, 2.5f);
        yield return new WaitForSeconds(0.8f);
    }

    private IEnumerator FadeOverlay(Color baseColor, float fromAlpha, float toAlpha, float duration)
    {
        if (screenOverlay == null) yield break;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            screenOverlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, Mathf.Lerp(fromAlpha, toAlpha, p));
            yield return null;
        }
        screenOverlay.color = new Color(baseColor.r, baseColor.g, baseColor.b, toAlpha);
    }
}
