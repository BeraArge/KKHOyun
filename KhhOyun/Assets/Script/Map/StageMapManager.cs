using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapManager : MonoBehaviour
{
    [Header("Loading Sahnesi")]
    [SerializeField] private string loadingSceneName = "LoadingScene";

    [Header("Panel Yaz�lar�")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text taskSubTitleText;

    [Header("Uyar� Penceresi")]
    [SerializeField] private WarningPopupUI warningPopup;

    private StageMapItem[] stageItems;
    private int currentStage;
    private bool isLoadingScene;

    private void Start()
    {
        stageItems = GetComponentsInChildren<StageMapItem>(true)
            .OrderBy(x => x.StageNumber)
            .ToArray();

        if (messageText != null)
        {
            messageText.text = "Yükleniyor...";
        }

        StageProgress.Initialize(
            onReady: () =>
            {
                currentStage = StageProgress.CurrentStage;
                RefreshStages();
            },
            onError: message =>
            {
                // Sunucuya ulaşılamazsa güvenli varsayılan: yalnızca 1. aşama açık.
                currentStage = StageProgress.CurrentStage;
                RefreshStages();
                ShowMessage(message);
            }
        );
    }

    public void StageClicked(int clickedStage)
    {
        // Pe� pe�e t�klamalar� engeller.
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
                $"{clickedStage}. a�ama i�in " +
                "StageMapItem bulunamad�."
            );

            return;
        }

        // Hen�z a��lmam�� a�ama.
        if (clickedStage > currentStage)
        {
            ShowLockedStageWarning(clickedStage);
            return;
        }

        /*
         * Aktif veya daha �nce tamamlanm�� a�ama
         * tekrar a��labilir.
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
                "Bu a�aman�n oyun sahnesi hen�z haz�rlanmad�."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(stage.SceneName))
        {
            ShowMessage(
                "Bu a�aman�n sahnesi hen�z kullan�ma haz�r de�il."
            );

            Debug.LogError(
                $"'{stage.SceneName}' sahnesi Build Profiles " +
                "i�indeki Scene List'te bulunamad�."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(loadingSceneName))
        {
            ShowMessage(
                "Loading sahnesinin ad� ayarlanmam��."
            );

            Debug.LogError(
                "StageMapManager �zerindeki Loading Scene Name alan� bo�."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(loadingSceneName))
        {
            ShowMessage(
                "Y�kleme ekran� hen�z kullan�ma haz�r de�il."
            );

            Debug.LogError(
                $"'{loadingSceneName}' sahnesi Build Profiles " +
                "i�indeki Scene List'te bulunamad�."
            );

            return;
        }

        isLoadingScene = true;

        /*
         * LoadingScene a��lmadan �nce, daha sonra
         * a��lacak ger�ek a�aman�n ad�n� kaydediyoruz.
         */
        LoadingSceneData.SetTargetScene(
            stage.SceneName
        );

        Debug.Log(
            $"LoadingScene a��l�yor. " +
            $"Hedef a�ama: {stage.SceneName}"
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
                "Bu a�ama hen�z haz�r de�il.\n" +
                $"�nce {currentStage}. a�amay� tamamlayal�m.";
        }
        else
        {
            warningMessage =
                "Bu a�ama hen�z haz�r de�il.\n" +
                "A�amalar� s�rayla tamamlayal�m.";
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
                    "Harika! T�m a�amalar� tamamlad�n. " +
                    "�yile�me yolculu�unu ba�ar�yla bitirdin.";
            }

            if (taskSubTitleText != null)
            {
                taskSubTitleText.text =
                    "T�m a�amalar tamamland�!";
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
                $"{currentStage}. a�amay� tamamla.";
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
                    "�lk a�amam�z haz�r! " +
                    "Ameliyat �ncesi Haz�rl�k b�l�m�ne " +
                    "dokunarak ba�layal�m.";

            case 2:
                return
                    "Harika gidiyorsun! " +
                    "�imdi Ameliyat G�n� a�amas�na ge�ebilirsin.";

            case 3:
                return
                    "�ok g�zel! �imdi ameliyat sonras� " +
                    "fiziksel aktiviteleri ��renelim.";

            case 4:
                return
                    "�imdi kalbimizi koruyan sa�l�kl� " +
                    "besinleri ke�fedelim.";

            case 5:
                return
                    "�la�lar� do�ru zamanda ve do�ru �ekilde " +
                    "kullanmay� ��renelim.";

            case 6:
                return
                    "Acil bir durumda ne yapman gerekti�ini " +
                    "birlikte ��renelim.";

            case 7:
                return
                    "Son a�amaday�z! Sa�l���n� kendi ba��na " +
                    "y�netmenin �nemini ��renelim.";

            default:
                return "";
        }
    }

    [ContextMenu("Haritay� Yenile")]
    public void RefreshMap()
    {
        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;

        RefreshStages();
    }

    [ContextMenu("�lerlemeyi S�f�rla")]
    public void ResetSavedProgress()
    {
        StageProgress.ResetProgress();

        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;

        RefreshStages();
    }
}