using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage5MedicineManager : MonoBehaviour
{
    [Header("Eğitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileşeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanılacak adım kimliği.")]
    [SerializeField]
    private string educationStepId = "asama5_egitim1";

    [Header("Sahne İçeriği")]
    [Tooltip("Eğitim tamamlanana kadar kapalı kalacak ilaç seçimi sahnesi.")]
    public GameObject sceneContent;

    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Medicine Slots")]
    public Image vitaminSlotImage;
    public Image heartMedicineSlotImage;
    public Image syrupSlotImage;

    [Header("Slot Targets")]
    public RectTransform vitaminSlotTarget;
    public RectTransform heartMedicineSlotTarget;
    public RectTransform syrupSlotTarget;

    [Header("Medicine Sprites")]
    public Sprite vitaminSprite;
    public Sprite heartMedicineSprite;
    public Sprite syrupSprite;

    [Header("Game Values")]
    public int score = 0;
    public int requiredCount = 3;
    public int currentRequiredCount = 0;

    [Header("Aşama Geçişi")]
    [SerializeField]
    private int stageNumber = 5;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    private bool vitaminSelected;
    private bool heartMedicineSelected;
    private bool syrupSelected;
    private bool stageCompleted;
    private bool isReturningToMap;
    private bool educationIsOpen;

    private void Start()
    {
        StageProgress.EnterStage(stageNumber);

        HideSlot(vitaminSlotImage);
        HideSlot(heartMedicineSlotImage);
        HideSlot(syrupSlotImage);

        educationIsOpen = true;

        if (sceneContent != null)
        {
            sceneContent.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Stage5MedicineManager: Scene Content alanına ilaç seçimi sahnesi atanmadı."
            );
        }

        StartCoroutine(
            StartEducationRoutine()
        );
    }

    private IEnumerator StartEducationRoutine()
    {
        if (educationPanel != null)
        {
            yield return educationPanel
                .ShowStepAndWaitForClose(
                    educationStepId
                );
        }
        else
        {
            Debug.LogWarning(
                "Stage5MedicineManager: Education Panel atanmadı. " +
                "Eğitim adımı atlanıyor."
            );
        }

        educationIsOpen = false;

        if (sceneContent != null)
        {
            sceneContent.SetActive(true);
        }

        Debug.Log(
            "[Stage5MedicineManager] Eğitim tamamlandı. İlaç seçimi başladı."
        );
    }

    public void SelectMedicine(
        string medicineName,
        Image clickedImage = null
    )
    {
        if (
            educationIsOpen ||
            stageCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        switch (medicineName)
        {
            case "vitamin":

                if (!vitaminSelected)
                {
                    vitaminSelected = true;

                    AddCorrectMedicine(
                        vitaminSlotImage,
                        vitaminSprite,
                        vitaminSlotTarget,
                        clickedImage
                    );
                }

                break;

            case "kalpilaci":

                if (!heartMedicineSelected)
                {
                    heartMedicineSelected = true;

                    AddCorrectMedicine(
                        heartMedicineSlotImage,
                        heartMedicineSprite,
                        heartMedicineSlotTarget,
                        clickedImage
                    );
                }

                break;

            case "surup":

                if (!syrupSelected)
                {
                    syrupSelected = true;

                    AddCorrectMedicine(
                        syrupSlotImage,
                        syrupSprite,
                        syrupSlotTarget,
                        clickedImage
                    );
                }

                break;

            case "baskailac":

                score -= 20;

                ShowWarning(
                    "Bu ilaç sana ait değil.\n" +
                    "Başkasının ilacını kullanmamalısın!"
                );

                break;

            default:

                Debug.LogWarning(
                    "Tanımsız ilaç adı: " +
                    medicineName
                );

                break;
        }

        CheckCompletion();
    }

    private void AddCorrectMedicine(
        Image slotImage,
        Sprite medicineSprite,
        RectTransform slotTarget,
        Image clickedImage
    )
    {
        score += 10;
        currentRequiredCount++;

        if (
            clickedImage != null &&
            flyAnimator != null &&
            slotTarget != null
        )
        {
            flyAnimator.FlyToSlot(
                clickedImage,
                slotTarget,
                () =>
                {
                    ShowSlot(
                        slotImage,
                        medicineSprite
                    );
                }
            );
        }
        else
        {
            ShowSlot(
                slotImage,
                medicineSprite
            );
        }
    }

    private void ShowSlot(
        Image slotImage,
        Sprite sprite
    )
    {
        if (
            slotImage == null ||
            sprite == null
        )
        {
            return;
        }

        slotImage.sprite = sprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;
        slotImage.preserveAspect = true;
    }

    private void HideSlot(
        Image slotImage
    )
    {
        if (slotImage == null)
        {
            return;
        }

        slotImage.enabled = false;
    }

    private void CheckCompletion()
    {
        if (
            stageCompleted ||
            currentRequiredCount <
            requiredCount
        )
        {
            return;
        }

        stageCompleted = true;

        StartCoroutine(
            CompleteStageRoutine()
        );
    }

    private IEnumerator CompleteStageRoutine()
    {
        yield return new WaitForSecondsRealtime(
            0.7f
        );

        if (warningPopup != null)
        {
            yield return warningPopup
                .ShowAndWaitForClose(
                    "Harika!\n" +
                    "Doğru ilaçları ilaç kutuna yerleştirdin."
                );
        }
        else
        {
            Debug.LogWarning(
                "Harika! Doğru ilaçları ilaç kutuna yerleştirdin."
            );
        }

        Debug.Log(
            "[Stage5MedicineManager] Aşama 5 tamamlandı. Skor: " +
            score
        );

        CompleteStageAndReturnToMap();
    }

    private void CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            return;
        }

        isReturningToMap = true;

        StageProgress.CompleteStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    isReturningToMap = false;
                    stageCompleted = false;

                    ShowWarning(
                        "Aşama ilerlemesi kaydedilemedi.\n" +
                        "Lütfen internet bağlantını kontrol edip tekrar dene."
                    );

                    return;
                }

                StartCoroutine(
                    ReturnToMapRoutine()
                );
            }
        );
    }

    private IEnumerator ReturnToMapRoutine()
    {
        yield return new WaitForSecondsRealtime(
            mapReturnDelay
        );

        if (
            string.IsNullOrWhiteSpace(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                "[Stage5MedicineManager] Map Scene Name alanı boş."
            );

            isReturningToMap = false;
            yield break;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                $"[Stage5MedicineManager] '{mapSceneName}' sahnesi " +
                "Build Profiles içindeki Scene List'te bulunamadı."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    private void ShowWarning(
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
}