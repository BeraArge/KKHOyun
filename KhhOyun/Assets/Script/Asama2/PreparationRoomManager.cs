using UnityEngine;

public class PreparationRoomManager : MonoBehaviour
{
    [Header("Stage Reference")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;

    private bool outfitDone = false;
    private bool jewelryDone = false;
    private bool layDownDone = false;
    private bool preparationComplete = false;

    public void SelectItem(string itemName, GameObject source)
    {
        Debug.Log("[PRM] SelectItem: " + itemName);
        if (preparationComplete) return;

        switch (itemName)
        {
            case "onluk":
                if (!outfitDone)
                {
                    outfitDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowSuccess("Harika! Önlüğünü çıkardın.");
                }
                break;

            case "taki":
                if (!jewelryDone)
                {
                    jewelryDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowSuccess("Harika! Takılarını çıkardın.");
                }
                break;

            case "yatak":
                if (!layDownDone)
                {
                    layDownDone = true;
                    if (stage2Manager != null) stage2Manager.AddScore(10);
                    if (source != null) source.SetActive(false);
                    ShowSuccess("Harika! Yatağa uzandın.");
                }
                break;

            case "ayakkabi":
                if (stage2Manager != null) stage2Manager.AddScore(-5);
                ShowWarning("Yatağa ayakkabıyla çıkmamalısın!");
                break;

            case "atla":
                if (stage2Manager != null) stage2Manager.AddScore(-10);
                ShowWarning("Kurallara uymak çok önemlidir!");
                break;

            default:
                Debug.LogWarning("[PRM] Tanınmayan itemName: " + itemName);
                break;
        }

        CheckCompletion();
    }

    private void ShowSuccess(string message)
    {
        if (warningPopup != null) warningPopup.Show(message);
        else Debug.LogError("[PRM] warningPopup NULL — Inspector'da bağla!");
    }

    private void ShowWarning(string message)
    {
        if (warningPopup != null) warningPopup.Show(message);
        else Debug.LogError("[PRM] warningPopup NULL — Inspector'da bağla!");
    }

    private void CheckCompletion()
    {
        if (!preparationComplete && outfitDone && jewelryDone && layDownDone)
        {
            preparationComplete = true;
            if (stage2Manager != null) stage2Manager.OnPhaseCComplete();
        }
    }
}
