using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapManager : MonoBehaviour
{
    [Header("Mevcut Açýk Aþama")]
    [SerializeField] private int currentStage = 1;

    [Header("Panel Yazýlarý")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text taskSubTitleText;

    [Header("Uyarý Penceresi")]
    [SerializeField] private WarningPopupUI warningPopup;

    private StageMapItem[] stageItems;

    private void Start()
    {
        stageItems = GetComponentsInChildren<StageMapItem>(true)
            .OrderBy(x => x.StageNumber)
            .ToArray();

        RefreshStages();
    }

    public void StageClicked(int clickedStage)
    {
        StageMapItem clickedItem = stageItems
            .FirstOrDefault(x => x.StageNumber == clickedStage);

        if (clickedItem == null)
        {
            Debug.LogError(
                $"{clickedStage}. aþama için StageMapItem bulunamadý."
            );

            return;
        }

        // Kilitli aþama.
        if (clickedStage > currentStage)
        {
            ShowLockedStageWarning(clickedStage);
            return;
        }

        // Aktif veya daha önce tamamlanan aþamayý aç.
        LoadStage(clickedItem);
    }

    private void LoadStage(StageMapItem stage)
    {
        if (string.IsNullOrWhiteSpace(stage.SceneName))
        {
            ShowMessage(
                "Bu aþamanýn oyun sahnesi henüz hazýrlanmadý."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(stage.SceneName))
        {
            ShowMessage(
                $"{stage.SceneName} sahnesi yüklenemiyor.\n" +
                "Sahneyi Build Profiles içindeki Scene List alanýna ekleyin."
            );

            Debug.LogError(
                $"'{stage.SceneName}' sahnesi Scene List içerisinde bulunamadý."
            );

            return;
        }

        SceneManager.LoadScene(stage.SceneName);
    }

    private void ShowLockedStageWarning(int clickedStage)
    {
        string warningMessage;

        if (clickedStage == currentStage + 1)
        {
            warningMessage =
                "Bu aþama henüz hazýr deðil.\n" +
                $"Önce {currentStage}. aþamayý tamamlayalým.";
        }
        else
        {
            warningMessage =
                "Bu aþama henüz hazýr deðil.\n" +
                "Aþamalarý sýrayla tamamlayalým.";
        }

        ShowMessage(warningMessage);
    }

    private void ShowMessage(string message)
    {
        if (warningPopup != null)
        {
            warningPopup.Show(message);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }

    private void RefreshStages()
    {
        foreach (StageMapItem stage in stageItems)
        {
            bool isCompleted =
                stage.StageNumber < currentStage;

            bool isCurrent =
                stage.StageNumber == currentStage;

            bool isLocked =
                stage.StageNumber > currentStage;

            if (currentStage > stageItems.Length)
            {
                isCompleted = true;
                isCurrent = false;
                isLocked = false;
            }

            stage.SetVisualState(
                isLocked,
                isCurrent,
                isCompleted
            );
        }

        UpdatePanelTexts();
    }

    private void UpdatePanelTexts()
    {
        if (currentStage > stageItems.Length)
        {
            if (messageText != null)
            {
                messageText.text =
                    "Harika! Tüm aþamalarý tamamladýn. " +
                    "Ýyileþme yolculuðunu baþarýyla bitirdin.";
            }

            if (taskSubTitleText != null)
            {
                taskSubTitleText.text =
                    "Tüm aþamalar tamamlandý!";
            }

            return;
        }

        if (messageText != null)
        {
            messageText.text =
                GetInformationMessage(currentStage);
        }

        if (taskSubTitleText != null)
        {
            taskSubTitleText.text =
                $"{currentStage}. aþamayý tamamla.";
        }
    }

    private string GetInformationMessage(int stageNumber)
    {
        switch (stageNumber)
        {
            case 1:
                return
                    "Ýlk aþamamýz hazýr! " +
                    "Ameliyat Öncesi Hazýrlýk bölümüne " +
                    "dokunarak baþlayalým.";

            case 2:
                return
                    "Harika gidiyorsun! " +
                    "Þimdi Ameliyat Günü aþamasýna geçebilirsin.";

            case 3:
                return
                    "Çok güzel! Þimdi ameliyat sonrasý " +
                    "fiziksel aktiviteleri öðrenelim.";

            case 4:
                return
                    "Þimdi kalbimizi koruyan saðlýklý " +
                    "besinleri keþfedelim.";

            case 5:
                return
                    "Ýlaçlarý doðru zamanda ve doðru þekilde " +
                    "kullanmayý öðrenelim.";

            case 6:
                return
                    "Acil bir durumda ne yapman gerektiðini " +
                    "birlikte öðrenelim.";

            case 7:
                return
                    "Son aþamadayýz! Saðlýðýný kendi baþýna " +
                    "yönetmenin önemini öðrenelim.";

            default:
                return "";
        }
    }

    [ContextMenu("Haritayý Yenile")]
    public void RefreshMap()
    {
        RefreshStages();
    }
}