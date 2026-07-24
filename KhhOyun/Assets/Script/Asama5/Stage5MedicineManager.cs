using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage5MedicineManager : MonoBehaviour
{
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

    [Header("Aþama Geçiþi")]
    [SerializeField]
    private int stageNumber = 5;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    private bool vitaminSelected = false;
    private bool heartMedicineSelected = false;
    private bool syrupSelected = false;
    private bool stageCompleted = false;
    private bool isReturningToMap = false;

    private void Start()
    {
        HideSlot(vitaminSlotImage);
        HideSlot(heartMedicineSlotImage);
        HideSlot(syrupSlotImage);
    }

    public void SelectMedicine(
        string medicineName,
        Image clickedImage = null
    )
    {
        if (
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
                    "Bu ilaç sana ait deðil.\n" +
                    "Baþkasýnýn ilacýný kullanmamalýsýn!"
                );

                break;

            default:

                Debug.LogWarning(
                    "Tanýmsýz ilaç adý: " +
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
                    "Doðru ilaçlarý ilaç kutuna yerleþtirdin."
                );
        }
        else
        {
            Debug.LogWarning(
                "Harika! Doðru ilaçlarý ilaç kutuna yerleþtirdin."
            );
        }

        Debug.Log(
            "[Stage5MedicineManager] Aþama 5 tamamlandý. Skor: " +
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
            stageNumber
        );

        StartCoroutine(
            ReturnToMapRoutine()
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
                "[Stage5MedicineManager] Map Scene Name alaný boþ."
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
                "Build Profiles içindeki Scene List'te bulunamadý."
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