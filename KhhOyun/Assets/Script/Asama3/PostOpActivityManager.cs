using UnityEngine;

public class PostOpActivityManager : MonoBehaviour
{
    public Asama3StageManager stageManager;
    public WarningPopupUI warningPopup;

    private bool otur45Done = false;
    private bool oksurNefesDone = false;
    private bool triflowDone = false;

    public void SelectItem(string itemName, GameObject source)
    {
        switch (itemName)
        {
            case "otur45":
                if (otur45Done)
                {
                    break;
                }

                otur45Done = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Bu pozisyon nefes almana yardımcı olur!");
                break;

            case "oksurnefes":
                if (oksurNefesDone)
                {
                    break;
                }

                oksurNefesDone = true;
                stageManager.AddScore(10);
                source.SetActive(false);
                ShowMessage("Aferin! Derin nefes almak iyileşmene yardımcı olur.");
                break;

            case "oyunodasi":
                stageManager.AddScore(-15);
                ShowMessage("Henüz oyun odası yasak, mikroplardan korunmalısın!");
                break;

            default:
                Debug.LogWarning("Tanımsız öğe adı: " + itemName);
                break;
        }

        CheckCompletion();
    }

    public void OnTriflowComplete()
    {
        if (triflowDone)
        {
            return;
        }

        triflowDone = true;
        stageManager.AddScore(20);
        ShowMessage("Harika! Akciğerlerin için çok önemli!");
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
        if (triflowDone && otur45Done && oksurNefesDone)
        {
            stageManager.OnPhaseAComplete();
        }
    }
}
