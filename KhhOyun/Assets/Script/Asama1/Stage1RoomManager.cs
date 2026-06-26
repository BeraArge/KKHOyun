using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Stage1RoomManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text messageText;

    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Bag Slots")]
    public Image pajamaSlotImage;
    public Image toothbrushSlotImage;
    public Image toySlotImage;

    [Header("Slot Targets")]
    public RectTransform pajamaSlotTarget;
    public RectTransform toothbrushSlotTarget;
    public RectTransform toySlotTarget;

    [Header("Item Sprites")]
    public Sprite pajamaSprite;
    public Sprite toothbrushSprite;
    public Sprite toySprite;

    [Header("Game Values")]
    public int score = 0;
    public int requiredCount = 2;
    public int currentRequiredCount = 0;

    private bool pajamaSelected = false;
    private bool toothbrushSelected = false;
    private bool toySelected = false;
    private bool stageCompleted = false;
    private bool toyRemovedFromBag = false;

    private void Start()
    {
        if (messageText != null)
            messageText.text = "Merhaba! Bugün hastane için çantaný hazýrlayacaðýz.";

        HideSlot(pajamaSlotImage);
        HideSlot(toothbrushSlotImage);
        HideSlot(toySlotImage);
    }

    public void SelectItem(string itemName, Image clickedImage = null)
    {
        if (stageCompleted)
            return;

        switch (itemName)
        {
            case "pijama":
                if (!pajamaSelected)
                {
                    pajamaSelected = true;
                    AddRequiredItem(pajamaSlotImage, pajamaSprite, pajamaSlotTarget, clickedImage);
                }
                break;

            case "disfircasi":
                if (!toothbrushSelected)
                {
                    toothbrushSelected = true;
                    AddRequiredItem(toothbrushSlotImage, toothbrushSprite, toothbrushSlotTarget, clickedImage);
                }
                break;

            case "ayicik":
                if (!toySelected)
                {
                    toySelected = true;
                    score += 10;
                    AddOptionalToy(clickedImage);
                }
                break;

            case "cips":
                score -= 10;
                warningPopup.Show("Þimdilik abur cubur almamalýsýn.");
                break;

            case "soda":
                score -= 10;
                warningPopup.Show("Gazlý içecekler ameliyat öncesinde uygun deðildir.");
                break;

            case "oyuncaklar":
                score -= 5;
                warningPopup.Show("Gereksiz oyuncaklarý yanýna almana gerek yok.");
                break;

            case "yemek":
                warningPopup.Show("Ameliyat öncesinde yemek yasak olabilir. Doktorunu dinlemelisin.");
                RestartStage();
                break;

            case "su":
                warningPopup.Show("Ameliyat öncesinde su içmek yasak olabilir. Doktorunu dinlemelisin.");
                RestartStage();
                break;
        }

        CheckCompletion();
    }

    private void AddRequiredItem(Image slotImage, Sprite itemSprite, RectTransform slotTarget, Image clickedImage)
    {
        score += 10;
        currentRequiredCount++;

        if (clickedImage != null && flyAnimator != null)
        {
            flyAnimator.FlyToSlot(clickedImage, slotTarget, () =>
            {
                ShowSlot(slotImage, itemSprite);
            });
        }
        else
        {
            ShowSlot(slotImage, itemSprite);
        }
    }

    private void AddOptionalToy(Image clickedImage)
    {
        if (clickedImage != null && flyAnimator != null)
        {
            flyAnimator.FlyToSlot(clickedImage, toySlotTarget, () =>
            {
                ShowSlot(toySlotImage, toySprite);
            });
        }
        else
        {
            ShowSlot(toySlotImage, toySprite);
        }
    }

    private void ShowSlot(Image slotImage, Sprite sprite)
    {
        if (slotImage == null || sprite == null)
            return;

        slotImage.sprite = sprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;
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

        if (toySelected && !toyRemovedFromBag)
        {
            toyRemovedFromBag = true;

            if (toySlotImage != null)
                toySlotImage.enabled = false;

            yield return warningPopup.ShowAndWaitForClose(
                "Oyuncaðýný yanýna alabilirsin.\nAma o ameliyata giremez,\nodanda seni bekleyecek."
            );
        }

        yield return warningPopup.ShowAndWaitForClose(
            "Harikasýn!\nÇantan hazýr."
        );
    }

    private void RestartStage()
    {
        Debug.Log("Oyun baþa dönecek.");
    }
}