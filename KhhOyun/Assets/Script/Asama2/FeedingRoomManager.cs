using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FeedingRoomManager : MonoBehaviour
{
    [Header("Stage Reference")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Tepsi Slotları - Sıvı (su/ayran)")]
    public Image liquidSlotImage;
    public Sprite liquidSprite;
    public RectTransform liquidSlotTarget;

    [Header("Tepsi Slotları - Çorba")]
    public Image soupSlotImage;
    public Sprite soupSprite;
    public RectTransform soupSlotTarget;

    [Header("Tepsi Slotları - Pirinç")]
    public Image riceSlotImage;
    public Sprite riceSprite;
    public RectTransform riceSlotTarget;

    [Header("Ayarlar")]
    public int maxWrongAttempts = 3;

    private bool liquidDone = false;
    private bool soupDone = false;
    private bool riceDone = false;
    private bool feedingComplete = false;

    private int wrongCount = 0;

    public void SelectItem(string itemName, GameObject source)
    {
        Debug.Log("[FRM] SelectItem: " + itemName);
        if (feedingComplete) return;

        switch (itemName)
        {
            case "su":
            case "ayran":
                if (!liquidDone)
                {
                    liquidDone = true;
                    HandleCorrectItem(source, liquidSlotImage, liquidSprite, liquidSlotTarget, "Harika! Önce az miktarla başlanır.");
                }
                break;

            case "corba":
                if (!soupDone)
                {
                    soupDone = true;
                    HandleCorrectItem(source, soupSlotImage, soupSprite, soupSlotTarget, "Güzel, çorba sindirimin için iyi!");
                }
                break;

            case "pirinc":
                if (!riceDone)
                {
                    riceDone = true;
                    HandleCorrectItem(source, riceSlotImage, riceSprite, riceSlotTarget, "Harika, artık katı gıdaya geçiyorsun!");
                }
                break;

            case "hamburger":
            case "gazliicecek":
                if (stage2Manager != null) stage2Manager.AddScore(-10);
                wrongCount++;
                ShowMessage("Önce az miktarla başlanır, sindirimine dikkat et!");
                if (wrongCount >= maxWrongAttempts)
                {
                    RestartStage();
                    return;
                }
                break;

            default:
                Debug.LogWarning("[FRM] Tanınmayan itemName: " + itemName);
                break;
        }
    }

    private void HandleCorrectItem(GameObject source, Image traySlotImage, Sprite traySprite, RectTransform slotTarget, string message)
    {
        if (stage2Manager != null) stage2Manager.AddScore(10);
        ShowMessage(message);

        Image sourceImage = source != null ? source.GetComponent<Image>() : null;

        void OnArrived()
        {
            ShowTraySlot(traySlotImage, traySprite);
            if (source != null) source.SetActive(false);
            CheckCompletion();
        }

        if (sourceImage != null && flyAnimator != null && slotTarget != null)
            flyAnimator.FlyToSlot(sourceImage, slotTarget, OnArrived);
        else
            OnArrived();
    }

    private void ShowTraySlot(Image slotImage, Sprite sprite)
    {
        if (slotImage == null)
        {
            Debug.LogError("[FRM] Tepsi slot Image atanmadı — Inspector'da bağla!");
            return;
        }
        if (sprite == null)
        {
            Debug.LogError("[FRM] Tepsi slot Sprite atanmadı — Inspector'da bağla!");
            return;
        }
        slotImage.sprite = sprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;
    }

    private void ShowMessage(string message)
    {
        if (warningPopup != null) warningPopup.Show(message);
        else Debug.LogError("[FRM] warningPopup NULL — Inspector'da bağla!");
    }

    private void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckCompletion()
    {
        if (!feedingComplete && liquidDone && soupDone && riceDone)
        {
            feedingComplete = true;
            if (stage2Manager != null) stage2Manager.OnPhaseHComplete();
        }
    }
}
