using UnityEngine;

public class ActivitySelectionManager : MonoBehaviour
{
    public Asama3StageManager stageManager;
    public WarningPopupUI warningPopup;

    private bool yuruyusDone = false;
    private bool tenisDone = false;

    public void SelectItem(string itemName, GameObject source)
    {
        switch (itemName)
        {
            case "yuruyus":
                if (yuruyusDone) break;
                yuruyusDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Harika seçim! Yürüyüş iyileşmene yardımcı olur.");
                break;

            case "tenis":
                if (tenisDone) break;
                tenisDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Hafif tenis de güzel bir seçim, dikkatli ol yeter!");
                break;

            case "futbol":
                stageManager.AddScore(-10);
                ShowMessage("Bu aktivite şu an için çok yorucu olabilir, biraz daha bekleyelim.");
                break;

            case "agirlikkaldirma":
                stageManager.AddScore(-10);
                ShowMessage("Ağırlık kaldırmak şu an vücuduna zarar verebilir.");
                break;

            default:
                Debug.LogWarning("Tanımsız öğe adı: " + itemName);
                break;
        }

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (yuruyusDone && tenisDone)
            stageManager.OnPhaseCComplete();
    }

    private void ShowMessage(string message)
    {
        if (warningPopup == null)
        {
            Debug.LogError("warningPopup atanmadı.");
            return;
        }

        warningPopup.Show(message);
    }
}
