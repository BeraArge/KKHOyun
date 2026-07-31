using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMapManager : MonoBehaviour
{
    [Header("Loading Sahnesi")]
    [SerializeField]
    private string loadingSceneName = "LoadingScene";

    [Header("Panel Yazıları")]
    [SerializeField]
    private TMP_Text messageText;

    [SerializeField]
    private TMP_Text taskSubTitleText;

    [Header("Uyarı Penceresi")]
    [SerializeField]
    private WarningPopupUI warningPopup;

    private StageMapItem[] stageItems;
    private int currentStage;

    private bool isLoadingScene;
    private bool progressLoaded;

    private void Start()
    {
        stageItems = GetComponentsInChildren<StageMapItem>(true)
            .OrderBy(x => x.StageNumber)
            .ToArray();

        progressLoaded = false;
        isLoadingScene = false;

        // Sunucu cevabı gelene kadar bütün aşamalar
        // kilitli ve pasif görünür.
        ShowLoadingState();

        StageProgress.Initialize(
            onReady: () =>
            {
                currentStage =
                    StageProgress.CurrentStage;

                progressLoaded = true;

                RefreshStages();
            },
            onError: message =>
            {
                /*
                 * Sunucuya ulaşılamazsa StageProgress içindeki
                 * mevcut güvenli değer kullanılır.
                 */
                currentStage =
                    StageProgress.CurrentStage;

                progressLoaded = true;

                RefreshStages();
                ShowMessage(message);
            }
        );
    }

    private void ShowLoadingState()
    {
        if (messageText != null)
        {
            messageText.text =
                "İlerleme bilgilerin yükleniyor...";
        }

        if (taskSubTitleText != null)
        {
            taskSubTitleText.text =
                "Lütfen bekle";
        }

        if (stageItems == null)
        {
            return;
        }

        foreach (StageMapItem stage in stageItems)
        {
            if (stage == null)
            {
                continue;
            }

            stage.SetVisualState(
                isLocked: true,
                isCurrent: false,
                isCompleted: false
            );
        }
    }

    public void StageClicked(int clickedStage)
    {
        // Sunucu bilgisi gelmeden aşamalara giriş yapılmaz.
        if (!progressLoaded)
        {
            ShowMessage(
                "İlerleme bilgilerin henüz yükleniyor.\n" +
                "Lütfen kısa bir süre bekle."
            );

            return;
        }

        // Peş peşe tıklamaları engeller.
        if (isLoadingScene)
        {
            return;
        }

        StageMapItem clickedItem =
            stageItems.FirstOrDefault(
                x => x.StageNumber == clickedStage
            );

        if (clickedItem == null)
        {
            Debug.LogError(
                $"{clickedStage}. aşama için " +
                "StageMapItem bulunamadı."
            );

            return;
        }

        // Henüz açılmamış aşama.
        if (clickedStage > currentStage)
        {
            ShowLockedStageWarning(
                clickedStage
            );

            return;
        }

        /*
         * Aktif veya daha önce tamamlanmış aşama
         * tekrar açılabilir.
         */
        LoadStageWithLoadingScreen(
            clickedItem
        );
    }

    private void LoadStageWithLoadingScreen(
        StageMapItem stage
    )
    {
        if (stage == null)
        {
            return;
        }

        if (
            string.IsNullOrWhiteSpace(
                stage.SceneName
            )
        )
        {
            ShowMessage(
                "Bu aşamanın oyun sahnesi henüz hazırlanmadı."
            );

            return;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                stage.SceneName
            )
        )
        {
            ShowMessage(
                "Bu aşamanın sahnesi henüz kullanıma hazır değil."
            );

            Debug.LogError(
                $"'{stage.SceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadı."
            );

            return;
        }

        if (
            string.IsNullOrWhiteSpace(
                loadingSceneName
            )
        )
        {
            ShowMessage(
                "Loading sahnesinin adı ayarlanmamış."
            );

            Debug.LogError(
                "StageMapManager üzerindeki " +
                "Loading Scene Name alanı boş."
            );

            return;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                loadingSceneName
            )
        )
        {
            ShowMessage(
                "Yükleme ekranı henüz kullanıma hazır değil."
            );

            Debug.LogError(
                $"'{loadingSceneName}' sahnesi Build Profiles " +
                "içindeki Scene List'te bulunamadı."
            );

            return;
        }

        isLoadingScene = true;

        LoadingSceneData.SetTargetScene(
            stage.SceneName
        );

        Debug.Log(
            $"LoadingScene açılıyor. " +
            $"Hedef aşama: {stage.SceneName}"
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
                "Bu aşama henüz hazır değil.\n" +
                $"Önce {currentStage}. aşamayı tamamlayalım.";
        }
        else
        {
            warningMessage =
                "Bu aşama henüz hazır değil.\n" +
                "Aşamaları sırayla tamamlayalım.";
        }

        ShowMessage(
            warningMessage
        );
    }

    private void ShowMessage(
        string message
    )
    {
        if (warningPopup != null)
        {
            warningPopup.Show(
                message
            );
        }
        else
        {
            Debug.LogWarning(
                message
            );
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
            if (stage == null)
            {
                continue;
            }

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
                    "Harika! Tüm aşamaları tamamladın. " +
                    "İyileşme yolculuğunu başarıyla bitirdin.";
            }

            if (taskSubTitleText != null)
            {
                taskSubTitleText.text =
                    "Tüm aşamalar tamamlandı!";
            }

            return;
        }

        if (messageText != null)
        {
            messageText.text =
                GetInformationMessage(
                    currentStage
                );
        }

        if (taskSubTitleText != null)
        {
            taskSubTitleText.text =
                $"{currentStage}. aşamayı tamamla.";
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
                    "İlk aşamamız hazır! " +
                    "Ameliyat Öncesi Hazırlık bölümüne " +
                    "dokunarak başlayalım.";

            case 2:
                return
                    "Harika gidiyorsun! " +
                    "Şimdi Ameliyat Günü aşamasına geçebilirsin.";

            case 3:
                return
                    "Çok güzel! Şimdi ameliyat sonrası " +
                    "fiziksel aktiviteleri öğrenelim.";

            case 4:
                return
                    "Şimdi kalbimizi koruyan sağlıklı " +
                    "besinleri keşfedelim.";

            case 5:
                return
                    "İlaçları doğru zamanda ve doğru şekilde " +
                    "kullanmayı öğrenelim.";

            case 6:
                return
                    "Acil bir durumda ne yapman gerektiğini " +
                    "birlikte öğrenelim.";

            case 7:
                return
                    "Son aşamadayız! Sağlığını kendi başına " +
                    "yönetmenin önemini öğrenelim.";

            default:
                return "";
        }
    }

    [ContextMenu("Haritayı Yenile")]
    public void RefreshMap()
    {
        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;
        progressLoaded = true;

        RefreshStages();
    }

    [ContextMenu("İlerlemeyi Sıfırla")]
    public void ResetSavedProgress()
    {
        StageProgress.ResetProgress();

        currentStage =
            StageProgress.CurrentStage;

        isLoadingScene = false;
        progressLoaded = true;

        RefreshStages();
    }
}