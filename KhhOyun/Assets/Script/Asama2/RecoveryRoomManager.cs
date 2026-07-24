using TMPro;
using UnityEngine;

public class RecoveryRoomManager : MonoBehaviour
{
    [Header("Stage Reference")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;

    [Header("Güvenli Seri Sayacı")]
    public TMP_Text safeStreakText;

    private bool cougBreathDone = false;
    private bool nurseToldDone = false;
    private bool positionChangedDone = false;
    private bool recoveryComplete = false;

    private float safeStreakSeconds = 0f;
    private bool phaseActive = false;

    public void StartPhase()
    {
        phaseActive = true;
        safeStreakSeconds = 0f;
        UpdateSafeStreakText();
    }

    private void Update()
    {
        if (!phaseActive) return;

        safeStreakSeconds += Time.deltaTime;
        UpdateSafeStreakText();
    }

    private void UpdateSafeStreakText()
    {
        if (safeStreakText != null)
            safeStreakText.text = Mathf.FloorToInt(safeStreakSeconds).ToString();
    }

    public void SelectItem(string itemName, GameObject source)
    {
        Debug.Log("[RRM] SelectItem: " + itemName);
        if (recoveryComplete) return;

        switch (itemName)
        {
            case "oksur":
                if (!cougBreathDone)
                {
                    cougBreathDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowMessage("Akciğerlerini açık tut!");
                }
                break;

            case "hemsireyesoyle":
                if (!nurseToldDone)
                {
                    nurseToldDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowMessage("Ağrın varsa söylemek çok önemli!");
                }
                break;

            case "pozisyondegistir":
                if (!positionChangedDone)
                {
                    positionChangedDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowMessage("Harika! Pozisyonunu değiştirdin.");
                }
                break;

            case "tup":
                safeStreakSeconds = 0f;
                UpdateSafeStreakText();
                if (stage2Manager != null) stage2Manager.AddScore(-10);
                ShowMessage("Tüplerine dokunma, incinebilirsin!");
                break;

            case "kalk":
                if (stage2Manager != null) stage2Manager.AddScore(-15);
                ShowMessage("Kendiliğinden kalkma, yardım iste!");
                break;

            default:
                Debug.LogWarning("[RRM] Tanınmayan itemName: " + itemName);
                break;
        }

        CheckCompletion();
    }

    private void ShowMessage(string message)
    {
        if (warningPopup != null) warningPopup.Show(message);
        else Debug.LogError("[RRM] warningPopup NULL — Inspector'da bağla!");
    }

    private void CheckCompletion()
    {
        if (!recoveryComplete && cougBreathDone && nurseToldDone && positionChangedDone)
        {
            recoveryComplete = true;
            phaseActive = false;
            if (stage2Manager != null) stage2Manager.OnPhaseEComplete();
        }
    }
}
