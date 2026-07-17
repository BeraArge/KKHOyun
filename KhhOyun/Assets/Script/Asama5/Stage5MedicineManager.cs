using System.Collections;
using UnityEngine;
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

    private bool vitaminSelected = false;
    private bool heartMedicineSelected = false;
    private bool syrupSelected = false;
    private bool stageCompleted = false;

    private void Start()
    {
        HideSlot(vitaminSlotImage);
        HideSlot(heartMedicineSlotImage);
        HideSlot(syrupSlotImage);
    }

    public void SelectMedicine(string medicineName, Image clickedImage = null)
    {
        if (stageCompleted)
            return;

        switch (medicineName)
        {
            case "vitamin":
                if (!vitaminSelected)
                {
                    vitaminSelected = true;
                    AddCorrectMedicine(vitaminSlotImage, vitaminSprite, vitaminSlotTarget, clickedImage);
                }
                break;

            case "kalpilaci":
                if (!heartMedicineSelected)
                {
                    heartMedicineSelected = true;
                    AddCorrectMedicine(heartMedicineSlotImage, heartMedicineSprite, heartMedicineSlotTarget, clickedImage);
                }
                break;

            case "surup":
                if (!syrupSelected)
                {
                    syrupSelected = true;
                    AddCorrectMedicine(syrupSlotImage, syrupSprite, syrupSlotTarget, clickedImage);
                }
                break;

            case "baskailac":
                score -= 20;
                warningPopup.Show("Bu ilaç sana ait deðil.\nBaþkasýnýn ilacýný kullanmamalýsýn!");
                break;

            default:
                Debug.LogWarning("Tanýmsýz ilaç adý: " + medicineName);
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

        if (clickedImage != null && flyAnimator != null)
        {
            flyAnimator.FlyToSlot(clickedImage, slotTarget, () =>
            {
                ShowSlot(slotImage, medicineSprite);
            });
        }
        else
        {
            ShowSlot(slotImage, medicineSprite);
        }
    }

    private void ShowSlot(Image slotImage, Sprite sprite)
    {
        if (slotImage == null || sprite == null)
            return;

        slotImage.sprite = sprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;
        slotImage.preserveAspect = true;
    }

    private void HideSlot(Image slotImage)
    {
        if (slotImage == null)
            return;

        slotImage.enabled = false;
    }

    private void CheckCompletion()
    {
        if (!stageCompleted && currentRequiredCount >= requiredCount)
        {
            stageCompleted = true;
            StartCoroutine(CompleteStageRoutine());
        }
    }

    private IEnumerator CompleteStageRoutine()
    {
        yield return new WaitForSeconds(0.7f);

        warningPopup.Show("Harika!\nDoðru ilaçlarý ilaç kutuna yerleþtirdin.");
    }
}