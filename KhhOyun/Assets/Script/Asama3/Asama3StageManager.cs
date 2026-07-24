using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Aşama Geçişi")]
    [SerializeField]
    private int stageNumber = 3;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    [Header("Debug")]
    [SerializeField]
    private bool debugSkipEnabled = false;

    [SerializeField]
    private DebugPhase debugStartPhase =
        DebugPhase.Egitim;

    private enum DebugPhase
    {
        Egitim,
        A,
        B,
        C
    }

    private enum Phase
    {
        A,
        B,
        C
    }

    private Phase currentPhase = Phase.A;
    private bool stageComplete = false;
    private bool isReturningToMap = false;

    private void Start()
    {
        if (phaseAContainer != null)
        {
            phaseAContainer.SetActive(false);
        }

        if (phaseBContainer != null)
        {
            phaseBContainer.SetActive(false);
        }

        if (phaseCContainer != null)
        {
            phaseCContainer.SetActive(false);
        }

        if (heartDisplay != null)
        {
            heartDisplay.UpdateHearts(score);
        }

        if (
            debugSkipEnabled &&
            debugStartPhase != DebugPhase.Egitim
        )
        {
            DebugJumpToPhase(
                debugStartPhase
            );

            return;
        }

        StartCoroutine(
            IntroThenBeginPhaseA()
        );
    }

    private void DebugJumpToPhase(
        DebugPhase target
    )
    {
        score = 70;
        stageComplete = false;
        isReturningToMap = false;

        if (heartDisplay != null)
        {
            heartDisplay.UpdateHearts(score);
        }

        switch (target)
        {
            case DebugPhase.A:

                currentPhase = Phase.A;

                if (phaseAContainer != null)
                {
                    phaseAContainer.SetActive(true);
                }

                if (messageText != null)
                {
                    messageText.text =
                        "Ameliyat sonrası iyileşmene yardımcı olacak aktiviteleri tamamlayalım!";
                }

                break;

            case DebugPhase.B:

                currentPhase = Phase.B;

                if (phaseBContainer != null)
                {
                    phaseBContainer.SetActive(true);
                }

                if (messageText != null)
                {
                    messageText.text =
                        "Yavaş yavaş hareket etmeye başlayalım, acele etme.";
                }

                break;

            case DebugPhase.C:

                currentPhase = Phase.C;

                if (phaseCContainer != null)
                {
                    phaseCContainer.SetActive(true);
                }

                if (messageText != null)
                {
                    messageText.text =
                        "Sana uygun bir aktivite seç.";
                }

                break;
        }
    }

    private IEnumerator IntroThenBeginPhaseA()
    {
        stageComplete = true;

        yield return ShowEducation(
            "introA"
        );

        stageComplete = false;

        if (phaseAContainer != null)
        {
            phaseAContainer.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text =
                "Ameliyat sonrası iyileşmene yardımcı olacak aktiviteleri tamamlayalım!";
        }
    }

    private IEnumerator ShowEducation(
        string stepId
    )
    {
        if (educationPanel == null)
        {
            Debug.LogWarning(
                "[A3SM] educationPanel atanmadı, '" +
                stepId +
                "' adımı atlanıyor."
            );

            yield break;
        }

        yield return educationPanel
            .ShowStepAndWaitForClose(
                stepId
            );
    }

    public void HandleClick(
        string itemName,
        GameObject source
    )
    {
        if (
            stageComplete ||
            isReturningToMap
        )
        {
            return;
        }

        switch (currentPhase)
        {
            case Phase.A:

                if (postOpActivityManager != null)
                {
                    postOpActivityManager.SelectItem(
                        itemName,
                        source
                    );
                }
                else
                {
                    Debug.LogError(
                        "[A3SM] postOpActivityManager NULL — Inspector'da bağla!"
                    );
                }

                break;

            case Phase.B:

                if (roomMovementManager != null)
                {
                    roomMovementManager.SelectItem(
                        itemName,
                        source
                    );
                }
                else
                {
                    Debug.LogError(
                        "[A3SM] roomMovementManager NULL — Inspector'da bağla!"
                    );
                }

                break;

            case Phase.C:

                if (activitySelectionManager != null)
                {
                    activitySelectionManager.SelectItem(
                        itemName,
                        source
                    );
                }
                else
                {
                    Debug.LogError(
                        "[A3SM] activitySelectionManager NULL — Inspector'da bağla!"
                    );
                }

                break;
        }
    }

    public void OnPhaseAComplete()
    {
        if (
            stageComplete ||
            isReturningToMap
        )
        {
            return;
        }

        StartCoroutine(
            TransitionToPhaseB()
        );
    }

    private IEnumerator TransitionToPhaseB()
    {
        stageComplete = true;

        yield return ShowEducation(
            "toPhaseB"
        );

        currentPhase = Phase.B;

        if (phaseAContainer != null)
        {
            phaseAContainer.SetActive(false);
        }

        if (phaseBContainer != null)
        {
            phaseBContainer.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text =
                "Yavaş yavaş hareket etmeye başlayalım, acele etme.";
        }

        stageComplete = false;
    }

    public void OnPhaseBComplete()
    {
        if (
            stageComplete ||
            isReturningToMap
        )
        {
            return;
        }

        StartCoroutine(
            TransitionToPhaseC()
        );
    }

    private IEnumerator TransitionToPhaseC()
    {
        stageComplete = true;

        yield return ShowEducation(
            "toPhaseC"
        );

        currentPhase = Phase.C;

        if (phaseBContainer != null)
        {
            phaseBContainer.SetActive(false);
        }

        if (phaseCContainer != null)
        {
            phaseCContainer.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text =
                "Sana uygun bir aktivite seç.";
        }

        stageComplete = false;
    }

    public void OnPhaseCComplete()
    {
        if (
            stageComplete ||
            isReturningToMap
        )
        {
            return;
        }

        StartCoroutine(
            FinishStageRoutine()
        );
    }

    private IEnumerator FinishStageRoutine()
    {
        stageComplete = true;

        yield return ShowEducation(
            "finish"
        );

        Debug.Log(
            "[A3SM] Aşama 3 tamamlandı. Skor: " +
            score
        );

        CompleteStageAndReturnToMap();
    }

    private void CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            return;
        }

        isReturningToMap = true;

        StageProgress.CompleteStage(
            stageNumber
        );

        StartCoroutine(
            ReturnToMapRoutine()
        );
    }

    private IEnumerator ReturnToMapRoutine()
    {
        yield return new WaitForSecondsRealtime(
            mapReturnDelay
        );

        if (
            string.IsNullOrWhiteSpace(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                "[A3SM] Map Scene Name alanı boş."
            );

            isReturningToMap = false;
            yield break;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                $"[A3SM] '{mapSceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadı."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    public void AddScore(
        int amount
    )
    {
        score += amount;

        if (heartDisplay != null)
        {
            heartDisplay.UpdateHearts(score);
        }
    }
}