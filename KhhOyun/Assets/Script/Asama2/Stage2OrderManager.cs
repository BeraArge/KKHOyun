using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage2OrderManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;
    public WarningPopupUI warningPopup;
    public DialoguePromptUI dialoguePrompt;

    [Header("Phase Containers")]
    public GameObject phaseAContainer;
    public GameObject phaseBContainer;
    public GameObject phaseCContainer;

    [Header("Phase A - Oda Sırası")]
    public string[] correctRoomOrder = { "kayit", "doktor", "bekleme", "ameliyat" };
    public Image[] roomSlotImages;
    public Sprite[] roomSlotSprites;

    [Header("Phase C")]
    public PreparationRoomManager prepRoomManager;

    [Header("Scoring")]
    public int score = 0;

    private int currentStep = 0;
    private Phase currentPhase = Phase.A;
    private bool stageComplete = false;

    private enum Phase { A, B, C }

    private void Start()
    {
        if (phaseAContainer != null) phaseAContainer.SetActive(true);
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(false);

        if (messageText != null)
            messageText.text = "Hastanede doğru sırayı takip etmelisin!";

        foreach (var img in roomSlotImages)
            if (img != null) img.enabled = false;
    }

    public void HandleClick(string itemName, GameObject source = null)
    {
        if (stageComplete) return;

        switch (currentPhase)
        {
            case Phase.A: HandlePhaseA(itemName); break;
            case Phase.B: HandlePhaseB(itemName); break;
            case Phase.C:
                if (prepRoomManager != null) prepRoomManager.SelectItem(itemName, source);
                else Debug.LogError("[S2OM] prepRoomManager NULL — Inspector'da bağla!");
                break;
        }
    }

    // ─── BÖLÜM A ─────────────────────────────────────────────────────────────

    private void HandlePhaseA(string itemName)
    {
        if (currentStep >= correctRoomOrder.Length) return;

        if (itemName == correctRoomOrder[currentStep])
        {
            score += 10;
            int step = currentStep++;
            RevealRoomSlot(step);
            if (currentStep >= correctRoomOrder.Length)
                StartCoroutine(TransitionToPhaseB());
        }
        else
        {
            score -= 5;
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
            dialoguePrompt.ShowTwoButton(
                "Merhaba! Seni ameliyata hazırlayacağız.",
                "Tamam", () => StartCoroutine(TransitionToPhaseC()),
                "Ne olacak?", ShowExplanationDialogue
            );
        }
        else
        {
            score -= 5;
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
        yield return new WaitForSeconds(0.3f);
        currentPhase = Phase.C;
        if (phaseBContainer != null) phaseBContainer.SetActive(false);
        if (phaseCContainer != null) phaseCContainer.SetActive(true);
        if (messageText != null)
            messageText.text = "Hazırlık odasındasın! Ameliyata hazırlanmak için gereken her şeyi yap.";
    }

    // ─── BÖLÜM C callback ────────────────────────────────────────────────────

    public void OnPhaseCComplete()
    {
        StartCoroutine(CompleteStageRoutine());
    }

    private IEnumerator CompleteStageRoutine()
    {
        stageComplete = true;
        yield return warningPopup.ShowAndWaitForClose("Harika! Şimdi hazırlanıyorsun!");
        Debug.Log("Asama 2 tamamlandı. Toplam skor: " + score);
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
}
