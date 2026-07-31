using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage2OrderManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public WarningPopupUI warningPopup;
    public DialoguePromptUI dialoguePrompt;

    [Header("Eğitim Paneli (adımlar panelin kendi Inspector listesinde tanımlanır)")]
    public EducationPanelUI educationPanel;

    [Header("Phase B - Karakter Seçimi")]
    public Image characterDisplay;
    public Sprite doktorSprite;
    public Sprite hemsireSprite;

    [Header("Phase Containers")]
    public GameObject phaseAContainer;
    public GameObject phaseBContainer;
    public GameObject phaseCContainer;
    public GameObject phaseDContainer;
    public GameObject phaseEContainer;
    public GameObject phaseFContainer;
    public GameObject phaseHContainer;

    [Header("Phase A - Oda Sırası")]
    public string[] correctRoomOrder = { "kayit", "doktor", "bekleme", "ameliyat" };
    public Image[] roomSlotImages;
    public Sprite[] roomSlotSprites;

    [Header("Phase C")]
    public PreparationRoomManager prepRoomManager;

    [Header("Phase D - Sedye")]
    public StretcherPhaseManager stretcherPhaseManager;

    [Header("Phase E - İyileşme Odası")]
    public RecoveryRoomManager recoveryRoomManager;

    [Header("Phase F - Yürüyüş")]
    public WalkingPhaseManager walkingPhaseManager;

    [Header("Phase H - Beslenme")]
    public FeedingRoomManager feedingRoomManager;

    [Header("Kalp Göstergesi")]
    public HeartDisplayUI heartDisplay;

    [Header("Scoring")]
    public int score = 0;

    [Header("Aşama Geçişi")]
    [SerializeField]
    private int stageNumber = 2;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    [Header("Debug")]
    [SerializeField] private bool debugSkipEnabled = false;
    [SerializeField] private Phase debugStartPhase = Phase.A;

    private int currentStep = 0;
    private Phase currentPhase = Phase.A;
    private bool stageComplete = false;
    private bool isReturningToMap = false;

    private enum Phase { A, B, C, D, E, F, H }

    private void Start()
    {
        StageProgress.EnterStage(stageNumber);

        if (debugSkipEnabled)
        {
            DebugJumpToPhase(debugStartPhase);
            return;
        }

        if (phaseAContainer != null) phaseAContainer.SetActive(false);
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(false);
        if (phaseDContainer != null) phaseDContainer.SetActive(false);
        if (phaseEContainer != null) phaseEContainer.SetActive(false);
        if (phaseFContainer != null) phaseFContainer.SetActive(false);
        if (phaseHContainer != null) phaseHContainer.SetActive(false);

        foreach (var img in roomSlotImages)
            if (img != null) img.enabled = false;

        if (heartDisplay != null) heartDisplay.UpdateHearts(score);

        StartCoroutine(IntroThenBeginPhaseA());
    }

    private IEnumerator IntroThenBeginPhaseA()
    {
        stageComplete = true;
        yield return ShowEducation("intro");
        stageComplete = false;

        if (phaseAContainer != null) phaseAContainer.SetActive(true);

        if (messageText != null)
            messageText.text = "Hastanede doğru sırayı takip etmelisin!";
    }

    private IEnumerator ShowEducation(string stepId)
    {
        if (educationPanel == null)
        {
            Debug.LogWarning("[S2OM] educationPanel atanmadı, '" + stepId + "' adımı atlanıyor.");
            yield break;
        }

        yield return educationPanel.ShowStepAndWaitForClose(stepId);
    }

    private void DebugJumpToPhase(Phase target)
    {
        score = 70;
        stageComplete = false;
        currentPhase = target;

        if (phaseAContainer != null) phaseAContainer.SetActive(target == Phase.A);
        if (phaseBContainer != null) phaseBContainer.SetActive(target == Phase.B);
        if (phaseCContainer != null) phaseCContainer.SetActive(target == Phase.C);
        if (phaseDContainer != null) phaseDContainer.SetActive(target == Phase.D);
        if (phaseEContainer != null) phaseEContainer.SetActive(target == Phase.E);
        if (phaseFContainer != null) phaseFContainer.SetActive(target == Phase.F);
        if (phaseHContainer != null) phaseHContainer.SetActive(target == Phase.H);

        if (heartDisplay != null) heartDisplay.UpdateHearts(score);

        switch (target)
        {
            case Phase.D:
                if (stretcherPhaseManager != null) stretcherPhaseManager.StartPhase();
                else Debug.LogError("[S2OM] stretcherPhaseManager NULL — Inspector'da bağla!");
                break;
            case Phase.E:
                if (recoveryRoomManager != null) recoveryRoomManager.StartPhase();
                else Debug.LogError("[S2OM] recoveryRoomManager NULL — Inspector'da bağla!");
                break;
            case Phase.F:
                if (walkingPhaseManager != null) walkingPhaseManager.StartPhase();
                else Debug.LogError("[S2OM] walkingPhaseManager NULL — Inspector'da bağla!");
                break;
        }
    }

    public void HandleClick(string itemName, GameObject source = null)
    {
        if (stageComplete || isReturningToMap)
        {
            return;
        }

        switch (currentPhase)
        {
            case Phase.A: HandlePhaseA(itemName); break;
            case Phase.B: HandlePhaseB(itemName); break;
            case Phase.C:
                if (prepRoomManager != null) prepRoomManager.SelectItem(itemName, source);
                else Debug.LogError("[S2OM] prepRoomManager NULL — Inspector'da bağla!");
                break;
            case Phase.E:
                if (recoveryRoomManager != null) recoveryRoomManager.SelectItem(itemName, source);
                else Debug.LogError("[S2OM] recoveryRoomManager NULL — Inspector'da bağla!");
                break;
            case Phase.H:
                if (feedingRoomManager != null) feedingRoomManager.SelectItem(itemName, source);
                else Debug.LogError("[S2OM] feedingRoomManager NULL — Inspector'da bağla!");
                break;
        }
    }

    // ─── BÖLÜM A ─────────────────────────────────────────────────────────────

    private void HandlePhaseA(string itemName)
    {
        if (currentStep >= correctRoomOrder.Length) return;

        if (itemName == correctRoomOrder[currentStep])
        {
            AddScore(10);
            int step = currentStep++;
            RevealRoomSlot(step);
            if (currentStep >= correctRoomOrder.Length)
                StartCoroutine(TransitionToPhaseB());
        }
        else
        {
            AddScore(-5);
            warningPopup.Show("Bu sıra doğru değil! Daha dikkatli düşün.");
        }
    }

    private void RevealRoomSlot(int index)
    {
        if (index >= roomSlotImages.Length || roomSlotImages[index] == null) return;
        if (index >= roomSlotSprites.Length || roomSlotSprites[index] == null) return;
        roomSlotImages[index].sprite = roomSlotSprites[index];
        roomSlotImages[index].color = Color.white;
        roomSlotImages[index].enabled = true;
    }

    private IEnumerator TransitionToPhaseB()
    {
        yield return new WaitForSeconds(0.5f);
        yield return warningPopup.ShowAndWaitForClose(
            "Harika! Tüm yerleri sırayla gezdin.\nŞimdi sana yardım edecek birini bul."
        );
        currentPhase = Phase.B;
        if (phaseAContainer != null) phaseAContainer.SetActive(false);
        if (phaseBContainer != null) phaseBContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Sana yardım edecek doğru kişiyi seç!";
    }

    // ─── BÖLÜM B ─────────────────────────────────────────────────────────────

    private void HandlePhaseB(string characterType)
    {
        Debug.Log("Phase B tıklandı: " + characterType + " | dialoguePrompt: " + (dialoguePrompt == null ? "NULL" : "OK"));
        if (characterType == "doktor" || characterType == "hemsire")
        {
            if (phaseBContainer != null) phaseBContainer.SetActive(false);
            if (characterDisplay != null)
            {
                characterDisplay.sprite = characterType == "doktor" ? doktorSprite : hemsireSprite;
                characterDisplay.gameObject.SetActive(true);
            }

            dialoguePrompt.ShowTwoButton(
                "Merhaba! Seni ameliyata hazırlayacağız.",
                "Tamam", () => StartCoroutine(TransitionToPhaseC()),
                "Ne olacak?", ShowExplanationDialogue
            );
        }
        else
        {
            AddScore(-5);
            string message = characterType == "yabanci"
                ? "Bu kişiyi tanımıyorsun, güvenli değil!"
                : "Temizlik görevlisi sana yardım edemez.";
            warningPopup.Show(message);
        }
    }

    private void ShowExplanationDialogue()
    {
        dialoguePrompt.ShowOneButton(
            "Seni uyutacağız ve hiçbir şey hissetmeyeceksin.",
            "Devam Et", () => StartCoroutine(TransitionToPhaseC())
        );
    }

    private IEnumerator TransitionToPhaseC()
    {
        stageComplete = true;
        if (characterDisplay != null) characterDisplay.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        yield return ShowEducation("afterB");
        stageComplete = false;
        currentPhase = Phase.C;
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Hazırlık odasındasın! Ameliyata hazırlanmak için gereken her şeyi yap.";
    }

    // ─── BÖLÜM C callback ────────────────────────────────────────────────────

    public void OnPhaseCComplete()
    {
        StartCoroutine(TransitionToPhaseD());
    }

    private IEnumerator TransitionToPhaseD()
    {
        stageComplete = true;
        yield return warningPopup.ShowAndWaitForClose("Harika! Şimdi hazırlanıyorsun!");
        stageComplete = false;
        currentPhase = Phase.D;
        if (phaseCContainer != null) phaseCContainer.SetActive(false);
        if (phaseDContainer != null) phaseDContainer.SetActive(true);
        if (stretcherPhaseManager != null) stretcherPhaseManager.StartPhase();
        else Debug.LogError("[S2OM] stretcherPhaseManager NULL — Inspector'da bağla!");
    }

    // ─── BÖLÜM D callback ────────────────────────────────────────────────────

    public void OnPhaseDComplete()
    {
        StartCoroutine(TransitionToPhaseE());
    }

    private IEnumerator TransitionToPhaseE()
    {
        stageComplete = true;
        if (phaseDContainer != null) phaseDContainer.SetActive(false);
        yield return ShowEducation("afterD");
        stageComplete = false;
        currentPhase = Phase.E;
        if (phaseEContainer != null) phaseEContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "İyileşme odasındasın! Doğru davranışları seç.";
        if (recoveryRoomManager != null) recoveryRoomManager.StartPhase();
        else Debug.LogError("[S2OM] recoveryRoomManager NULL — Inspector'da bağla!");
    }

    // ─── BÖLÜM E callback ────────────────────────────────────────────────────

    public void OnPhaseEComplete()
    {
        StartCoroutine(TransitionToPhaseF());
    }

    private IEnumerator TransitionToPhaseF()
    {
        stageComplete = true;
        if (phaseEContainer != null) phaseEContainer.SetActive(false);
        yield return ShowEducation("afterE");
        stageComplete = false;
        currentPhase = Phase.F;
        if (phaseFContainer != null) phaseFContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Şimdi biraz yürüyelim!";
        if (walkingPhaseManager != null) walkingPhaseManager.StartPhase();
        else Debug.LogError("[S2OM] walkingPhaseManager NULL — Inspector'da bağla!");
    }

    // ─── BÖLÜM F callback ────────────────────────────────────────────────────

    public void OnPhaseFComplete()
    {
        currentPhase = Phase.H;
        if (phaseFContainer != null) phaseFContainer.SetActive(false);
        if (phaseHContainer != null) phaseHContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Şimdi ağızdan beslenmeye başlayabilirsin!";
    }

    // ─── BÖLÜM H callback ────────────────────────────────────────────────────

    public void OnPhaseHComplete()
    {
        StartCoroutine(FinishStageRoutine());
    }

    private IEnumerator FinishStageRoutine()
    {
        if (stageComplete || isReturningToMap)
        {
            yield break;
        }

        stageComplete = true;

        if (warningPopup != null)
        {
            yield return warningPopup.ShowAndWaitForClose(
                "Tebrikler, odana geçeceksin!"
            );
        }

        Debug.Log(
            "[S2OM] Aşama 2 tamamen tamamlandı. Skor: " +
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
            stageNumber,
            success =>
            {
                if (!success)
                {
                    isReturningToMap = false;
                    stageComplete = false;

                    if (warningPopup != null)
                    {
                        warningPopup.Show(
                            "Aşama ilerlemesi kaydedilemedi.\n" +
                            "Lütfen internet bağlantını kontrol edip tekrar dene."
                        );
                    }
                    else
                    {
                        Debug.LogError(
                            "[S2OM] Aşama 2 ilerlemesi sunucuya kaydedilemedi."
                        );
                    }

                    return;
                }

                StartCoroutine(
                    ReturnToMapRoutine()
                );
            }
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
                "[S2OM] Map Scene Name alanı boş."
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
                $"[S2OM] '{mapSceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadı."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    public void AddScore(int amount)
    {
        score += amount;
        if (heartDisplay != null) heartDisplay.UpdateHearts(score);
    }
}