using UnityEngine;

public class RoomMovementManager : MonoBehaviour
{
    public Asama3StageManager stageManager;
    public WarningPopupUI warningPopup;

    private bool suDone = false;
    private bool dinlenmekDone = false;
    private bool nefesDone = false;

    public void SelectItem(string itemName, GameObject source)
    {
        switch (itemName)
        {
            case "su":
                if (suDone)
                {
                    break;
                }

                suDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Su içmek iyileşmene yardımcı olur!");
                break;

            case "dinlenmek":
                if (dinlenmekDone)
                {
                    break;
                }

                dinlenmekDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Dinlenmek vücudunun güçlenmesine yardımcı olur.");
                break;

            case "nefesegzersizi":
                if (nefesDone)
                {
                    break;
                }

                nefesDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Nefes egzersizi akciğerlerin için çok iyi.");
                break;

            case "kosmak":
            case "zipla":
                stageManager.AddScore(-10);
                ShowMessage("Acele etme… yavaş yavaş güçleniyorsun.");
                break;

            default:
                Debug.LogWarning("Tanımsız öğe adı: " + itemName);
                break;
        }

        CheckCompletion();
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

    private void CheckCompletion()
    {
        if (suDone && dinlenmekDone && nefesDone)
        {
            stageManager.OnPhaseBComplete();
        }
    }
}
