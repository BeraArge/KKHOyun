using System.Collections;
using TMPro;
using UnityEngine;

public class Asama3StageManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public WarningPopupUI warningPopup;
    public HeartDisplayUI heartDisplay;

    [Header("Eğitim Paneli (adımlar panelin kendi Inspector listesinde tanımlanır)")]
    public EducationPanelUI educationPanel;

    [Header("Phase Containers")]
    public GameObject phaseAContainer;
    public GameObject phaseBContainer;
    public GameObject phaseCContainer;

    [Header("Faz A - Ameliyat Sonrası Aktiviteler")]
    public PostOpActivityManager postOpActivityManager;

    [Header("Faz B - Oda İçi Hareket")]
    public RoomMovementManager roomMovementManager;

    [Header("Faz C - Aktivite Seçimi")]
    public ActivitySelectionManager activitySelectionManager;

    [Header("Scoring")]
    public int score = 0;

    [Header("Debug")]
    [SerializeField] private bool debugSkipEnabled = false;
    [SerializeField] private DebugPhase debugStartPhase = DebugPhase.Egitim;

    private enum DebugPhase { Egitim, A, B, C }
    private enum Phase { A, B, C }
    private Phase currentPhase = Phase.A;
    private bool stageComplete = false;

    private void Start()
    {
        if (phaseAContainer != null) phaseAContainer.SetActive(false);
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(false);

        if (heartDisplay != null) heartDisplay.UpdateHearts(score);

        if (debugSkipEnabled && debugStartPhase != DebugPhase.Egitim)
        {
            DebugJumpToPhase(debugStartPhase);
            return;
        }

        StartCoroutine(IntroThenBeginPhaseA());
    }

    private void DebugJumpToPhase(DebugPhase target)
    {
        score = 70;
        stageComplete = false;

        if (heartDisplay != null) heartDisplay.UpdateHearts(score);

        switch (target)
        {
            case DebugPhase.A:
                currentPhase = Phase.A;
                if (phaseAContainer != null) phaseAContainer.SetActive(true);
                if (messageText != null)
                    messageText.text = "Ameliyat sonrası iyileşmene yardımcı olacak aktiviteleri tamamlayalım!";
                break;
            case DebugPhase.B:
                currentPhase = Phase.B;
                if (phaseBContainer != null) phaseBContainer.SetActive(true);
                if (messageText != null)
                    messageText.text = "Yavaş yavaş hareket etmeye başlayalım, acele etme.";
                break;
            case DebugPhase.C:
                currentPhase = Phase.C;
                if (phaseCContainer != null) phaseCContainer.SetActive(true);
                if (messageText != null)
                    messageText.text = "Sana uygun bir aktivite seç.";
                break;
        }
    }

    private IEnumerator IntroThenBeginPhaseA()
    {
        yield return ShowEducation("introA");

        if (phaseAContainer != null) phaseAContainer.SetActive(true);

        if (messageText != null)
            messageText.text = "Ameliyat sonrası iyileşmene yardımcı olacak aktiviteleri tamamlayalım!";
    }

    private IEnumerator ShowEducation(string stepId)
    {
        if (educationPanel == null)
        {
            Debug.LogWarning("[A3SM] educationPanel atanmadı, '" + stepId + "' adımı atlanıyor.");
            yield break;
        }

        yield return educationPanel.ShowStepAndWaitForClose(stepId);
    }

    public void HandleClick(string itemName, GameObject source)
    {
        if (stageComplete) return;

        switch (currentPhase)
        {
            case Phase.A:
                if (postOpActivityManager != null) postOpActivityManager.SelectItem(itemName, source);
                else Debug.LogError("[A3SM] postOpActivityManager NULL — Inspector'da bağla!");
                break;
            case Phase.B:
                if (roomMovementManager != null) roomMovementManager.SelectItem(itemName, source);
                else Debug.LogError("[A3SM] roomMovementManager NULL — Inspector'da bağla!");
                break;
            case Phase.C:
                if (activitySelectionManager != null) activitySelectionManager.SelectItem(itemName, source);
                else Debug.LogError("[A3SM] activitySelectionManager NULL — Inspector'da bağla!");
                break;
        }
    }

    public void OnPhaseAComplete()
    {
        StartCoroutine(TransitionToPhaseB());
    }

    private IEnumerator TransitionToPhaseB()
    {
        stageComplete = true;
        yield return ShowEducation("toPhaseB");
        stageComplete = false;
        currentPhase = Phase.B;
        if (phaseAContainer != null) phaseAContainer.SetActive(false);
        if (phaseBContainer != null) phaseBContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Yavaş yavaş hareket etmeye başlayalım, acele etme.";
    }

    public void OnPhaseBComplete()
    {
        StartCoroutine(TransitionToPhaseC());
    }

    private IEnumerator TransitionToPhaseC()
    {
        stageComplete = true;
        yield return ShowEducation("toPhaseC");
        stageComplete = false;
        currentPhase = Phase.C;
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Sana uygun bir aktivite seç.";
    }

    public void OnPhaseCComplete()
    {
        StartCoroutine(FinishStageRoutine());
    }

    private IEnumerator FinishStageRoutine()
    {
        stageComplete = true;
        yield return ShowEducation("finish");
        Debug.Log("[A3SM] Asama 3 tamamlandı. Skor: " + score);
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (heartDisplay != null) heartDisplay.UpdateHearts(score);
    }
}
