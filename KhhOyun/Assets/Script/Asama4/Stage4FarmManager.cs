using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4FarmManager : MonoBehaviour
{
    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Collected Food Slots")]
    public Image[] collectedSlots;
    public RectTransform[] collectedSlotTargets;

    [Header("Game Values")]
    public int score = 0;
    public int requiredHealthyFoodCount = 7;
    public int collectedHealthyFoodCount = 0;

    private bool stageCompleted = false;
    private HashSet<string> collectedFoods = new HashSet<string>();

    private void Start()
    {
        foreach (Image slot in collectedSlots)
        {
            if (slot != null)
                slot.enabled = false;
        }
    }

    public void SelectFood(string foodName, Image clickedImage = null)
    {
        if (stageCompleted)
            return;

        switch (foodName)
        {
            case "ekmek":
            case "pilav":
            case "pirinc":
            case "brokoli":
            case "havuc":
            case "erik":
            case "kiraz":
            case "zeytinyagi":
                SelectHealthyFood(foodName, clickedImage);
                break;

            case "hamburger":
                score -= 10;
                warningPopup.Show("Hamburgeri sýk tüketmemeliyiz.\nKalbine daha faydalý besinleri seçelim.");
                break;

            case "cips":
                score -= 10;
                warningPopup.Show("Cips yerine daha saðlýklý atýþtýrmalýklar seçebilirsin.");
                break;

            case "kola":
            case "soda":
                score -= 10;
                warningPopup.Show("Gazlý içecekler saðlýklý bir seçim deðildir.");
                break;

            case "tuz":
                score -= 15;
                warningPopup.Show("Aþýrý tuz kalbini yorabilir.");
                break;

            default:
                Debug.LogWarning("Tanýmsýz besin adý: " + foodName);
                break;
        }

        CheckCompletion();
    }

    private void SelectHealthyFood(string foodName, Image clickedImage)
    {
        if (collectedFoods.Contains(foodName))
            return;

        if (clickedImage == null)
        {
            Debug.LogError(foodName + " için Fly Image atanmadý.");
            return;
        }

        collectedFoods.Add(foodName);

        score += 10;

        int slotIndex = collectedHealthyFoodCount;
        collectedHealthyFoodCount++;

        if (slotIndex >= collectedSlots.Length)
            return;

        Image targetSlot = collectedSlots[slotIndex];
        RectTransform targetRect = collectedSlotTargets[slotIndex];

        if (flyAnimator != null)
        {
            flyAnimator.FlyToSlot(clickedImage, targetRect, () =>
            {
                ShowSlot(targetSlot, clickedImage.sprite);
            });
        }
        else
        {
            ShowSlot(targetSlot, clickedImage.sprite);
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

    private void CheckCompletion()
    {
        if (!stageCompleted && collectedHealthyFoodCount >= requiredHealthyFoodCount)
        {
            stageCompleted = true;
            StartCoroutine(CompleteStageRoutine());
        }
    }

    private IEnumerator CompleteStageRoutine()
    {
        yield return new WaitForSeconds(0.7f);

        yield return warningPopup.ShowAndWaitForClose(
            "Harikasýn!\nSaðlýklý besinleri topladýn.\nÞimdi dengeli tabak hazýrlayabiliriz!"
        );

        Debug.Log("Dengeli Tabak bölümüne geçilecek.");
    }
}