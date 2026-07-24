using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TansiyonGorevi : MonoBehaviour
{
    [Header("Görev 1 Kapanýþ Objeleri")]
    [SerializeField] private GameObject task1;
    [SerializeField] private GameObject task1_UI;

    [Header("Görev 2 UI Panelleri")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private CanvasGroup quizUIGroup;

    [Header("Görev 3 Açýlýþ Objesi")]
    [SerializeField] private GameObject task3;

    [Header("Geçiþ Efekti")]
    [SerializeField] private Image fadeScreen;
    [SerializeField] private float fadeSpeed = 1.5f;

    [Header("Geri Bildirim (Juice) Ayarlarý")]
    [SerializeField] private TextMeshProUGUI soruText; 
    [SerializeField] private RectTransform shakePanel;

    private void OnEnable()
    {
        GameEvents.OnTaskCompleted += StartMission;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskCompleted -= StartMission;
    }

    private void StartMission(int taskId)
    {
        
        if (taskId == 1)
        {
            StartCoroutine(CinematicTransition());
        }
    }
    private IEnumerator CinematicTransition()
    {
        fadeScreen.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));

        task1.SetActive(false);
        task1_UI.SetActive(false);

        quizPanel.SetActive(true);
        quizUIGroup.alpha = 0f;
        quizUIGroup.interactable = false;
        quizUIGroup.blocksRaycasts = false;

        yield return StartCoroutine(Fade(1f, 0f));
        fadeScreen.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(FadeUI(0f, 1f));

        
    }

    private IEnumerator Fade(float startAlpha, float finishAlpha)
    {
        float counter = 0f;
        Color renk = fadeScreen.color;

        while (counter < 1f)
        {
            counter += Time.deltaTime * fadeSpeed;
            renk.a = Mathf.Lerp(startAlpha, finishAlpha, counter);
            fadeScreen.color = renk;
            yield return null;
        }
    }

    private IEnumerator FadeUI(float startAlpha,float finishAlpha)
    {
        float counter = 0f;
        while (counter < 1f)
        {
            counter += Time.deltaTime * fadeSpeed;
            quizUIGroup.alpha = Mathf.Lerp(startAlpha, finishAlpha, counter);
            yield return null;
        }
        quizUIGroup.interactable = true;
        quizUIGroup.blocksRaycasts = true;
    }

    public void CorrectAnswer()
    {
        Debug.Log("Tebrikler doðru cevap seçildi");
        GameEvents.OnTaskCompleted?.Invoke(2);
        quizPanel.SetActive(false);
        task3.SetActive(true);
        
    }
    public void WrongAnswer()
    {
        StartCoroutine (WrongAnswerEffect());
    }
    private IEnumerator WrongAnswerEffect()
    {
        quizUIGroup.interactable = false;

        string originalText = soruText.text;
        Color originalColor = soruText.color;

        soruText.text = "Yanlýþ cevap. Bir daha dene!";
        soruText.color = Color.red;

        Vector3 originalPosition = shakePanel.anchoredPosition;
        float shakeDuration = 0.4f;
        float counter = 0f;

        while (counter < shakeDuration)
        {
            counter += Time.deltaTime;
            float xOffset = Mathf.Sin(counter * 50f) * 15f;
            shakePanel.anchoredPosition = originalPosition + new Vector3(xOffset, 0, 0);
            yield return null;
        }
        shakePanel.anchoredPosition = originalPosition;
        yield return new WaitForSeconds(0.2f);
        soruText.text = originalText;
        soruText.color = originalColor;

        quizUIGroup.interactable = true;
    }

}
