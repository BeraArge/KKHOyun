using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapManager : MonoBehaviour
{
    [Header("Loading Sahnesi")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("Panel Yazýlarý")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text taskSubTitleText;

    [Header("Uyarý Penceresi")]
    [SerializeField] private WarningPopupUI warningPopup;

    private StageMapItem[] stageItems;
    private int currentStage;
    private bool isLoadingScene;

    private void Start()
    {
        stageItems = GetComponentsInChildren<StageMapItem>(true)
            .OrderBy(x => x.StageNumber)
            .ToArray();

        currentStage = StageProgress.CurrentStage;

        RefreshStages();
    }

    public void StageClicked(int clickedStage)
    {
        // Peþ peþe týklamalarý engeller.
        if (isLoadingScene)
        {
            return;
        }

        StageMapItem clickedItem = stageItems
            .FirstOrDefault(
                x => x.StageNumber == clickedStage
            );

        if (clickedItem == null)
        {
            Debug.LogError(
                $"{clickedStage}. aþama için " +
                "StageMapItem bulunamadý."
            );

            return;
        }

        // Henüz açýlmamýþ aþama.
        if (clickedStage > currentStage)
        {
            ShowLockedStageWarning(clickedStage);
            return;
        }

        /*
         * Aktif veya daha önce tamamlanmýþ aþama
         * tekrar açýlabilir.
         */
        LoadStageWithLoadingScreen(clickedItem);
    }

    private void LoadStageWithLoadingScreen(
        StageMapItem stage
    )
    {
        if (stage == null)
        {
            return;
        }

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
                "Bu aþamanýn sahnesi henüz kullanýma hazýr deðil."
            );

            Debug.LogError(
                $"'{stage.SceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadý."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(loadingSceneName))
        {
            ShowMessage(
                "Loading sahnesinin adý ayarlanmamýþ."
            );

            Debug.LogError(
                "StageMapManager üzerindeki Loading Scene Name alaný boþ."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(loadingSceneName))
        {
            ShowMessage(
                "Yükleme ekraný henüz kullanýma hazýr deðil."
            );

            Debug.LogError(
                $"'{loadingSceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadý."
            );

            return;
        }

        isLoadingScene = true;

        /*
         * LoadingScene açýlmadan önce, daha sonra
         * açýlacak gerçek aþamanýn adýný kaydediyoruz.
         */
        LoadingSceneData.SetTargetScene(
            stage.SceneName
        );

        Debug.Log(
            $"LoadingScene açýlýyor. " +
            $"Hedef aþama: {stage.SceneName}"
        );

        SceneManager.LoadScene(
            loadingSceneName
        );
    }

    private void ShowLockedStageWarning(
        int clickedStage
    )
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
        if (stageItems == null)
        {
            return;
        }

        bool allStagesCompleted =
            StageProgress.HighestCompletedStage >=
            StageProgress.TotalStageCount;

        foreach (StageMapItem stage in stageItems)
        {
            bool isCompleted;
            bool isCurrent;
            bool isLocked;

            if (allStagesCompleted)
            {
                isCompleted = true;
                isCurrent = false;
                isLocked = false;
            }
            else
            {
                isCompleted =
                    stage.StageNumber < currentStage;

                isCurrent =
                    stage.StageNumber == currentStage;

                isLocked =
                    stage.StageNumber > currentStage;
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
        bool allStagesCompleted =
            StageProgress.HighestCompletedStage >=
            StageProgress.TotalStageCount;

        if (allStagesCompleted)
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

    private string GetInformationMessage(
        int stageNumber
    )
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
        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;

        RefreshStages();
    }

    [ContextMenu("Ýlerlemeyi Sýfýrla")]
    public void ResetSavedProgress()
    {
        StageProgress.ResetProgress();

        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;

        RefreshStages();
    }
}