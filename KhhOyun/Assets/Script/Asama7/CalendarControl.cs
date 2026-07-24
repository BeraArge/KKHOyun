using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CalendarControl : MonoBehaviour
{
    [Header("Görev Ayarlarý")]
    public int currentDay = 3;
    public int targetDay = 10;
    [SerializeField] private GameObject task5_UI;
    public TextMeshProUGUI feedbackText;

    private bool isMissionFinished = false;
    private Coroutine resetCoroutine;
    private Color originalColor;
    private string originalText;

    private void Start()
    {
        if (feedbackText != null)
        {
            originalColor = feedbackText.color;
            originalText = feedbackText.text;
        }
    }

    public void OnDayClicked(int clickedDay)
    {
        if (isMissionFinished) return;

        if (clickedDay == targetDay)
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);

            feedbackText.color = Color.green;
            feedbackText.text = "Doktor kontrollerini aksatmamak hastalýðýný yönetmeni saðlar!";
            Debug.Log("Doðru gün! Randevu iþaretlendi.");
            isMissionFinished = true;
            GameEvents.OnTaskCompleted?.Invoke(5);
        }
        else
        {
            if (resetCoroutine != null) StopCoroutine(resetCoroutine);

            feedbackText.color = Color.red;
            feedbackText.text = "Yanlýþ günü seçtin! Tekrar dene.";
            Debug.Log("Yanlýþ gün seçildi");

            resetCoroutine = StartCoroutine(ResetFeedback());
        }
    }

    private IEnumerator ResetFeedback()
    {
        yield return new WaitForSeconds(1f);
        feedbackText.color = originalColor;
        feedbackText.text = originalText;
        resetCoroutine = null;
    }
}