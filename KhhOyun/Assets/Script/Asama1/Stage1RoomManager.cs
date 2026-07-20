using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Aþama Geçiþi")]
    [SerializeField] private int stageNumber = 1;
    [SerializeField] private string mapSceneName = "Map";
    [SerializeField] private float mapReturnDelay = 0.4f;

    private bool pajamaSelected;
    private bool toothbrushSelected;
    private bool toySelected;
    private bool stageCompleted;
    private bool toyRemovedFromBag;
    private bool isReturningToMap;

    private void Start()
    {
        if (messageText != null)
        {
            messageText.text =
                "Merhaba! Bugün hastane için çantaný hazýrlayacaðýz.";
        }

        HideSlot(pajamaSlotImage);
        HideSlot(toothbrushSlotImage);
        HideSlot(toySlotImage);
    }

    public void SelectItem(
        string itemName,
        Image clickedImage = null
    )
    {
        if (stageCompleted || isReturningToMap)
        {
            return;
        }

        switch (itemName)
        {
            case "pijama":
                SelectPajama(clickedImage);
                break;

            case "disfircasi":
                SelectToothbrush(clickedImage);
                break;

            case "ayicik":
                SelectToy(clickedImage);
                break;

            case "cips":
                score -= 10;
                ShowWarning(
                    "Þimdilik abur cubur almamalýsýn."
                );
                break;

            case "soda":
                score -= 10;
                ShowWarning(
                    "Gazlý içecekler ameliyat öncesinde uygun deðildir."
                );
                break;

            case "oyuncaklar":
                score -= 5;
                ShowWarning(
                    "Gereksiz oyuncaklarý yanýna almana gerek yok."
                );
                break;

            case "yemek":
                StartCoroutine(
                    RestartStageRoutine(
                        "Ameliyat öncesinde yemek yasak olabilir. " +
                        "Doktorunu dinlemelisin."
                    )
                );
                return;

            case "su":
                StartCoroutine(
                    RestartStageRoutine(
                        "Ameliyat öncesinde su içmek yasak olabilir. " +
                        "Doktorunu dinlemelisin."
                    )
                );
                return;

            default:
                Debug.LogWarning(
                    $"Tanýmlanmayan eþya: {itemName}"
                );
                break;
        }

        CheckCompletion();
    }

    private void SelectPajama(Image clickedImage)
    {
        if (pajamaSelected)
        {
            return;
        }

        pajamaSelected = true;

        AddRequiredItem(
            pajamaSlotImage,
            pajamaSprite,
            pajamaSlotTarget,
            clickedImage
        );
    }

    private void SelectToothbrush(Image clickedImage)
    {
        if (toothbrushSelected)
        {
            return;
        }

        toothbrushSelected = true;

        AddRequiredItem(
            toothbrushSlotImage,
            toothbrushSprite,
            toothbrushSlotTarget,
            clickedImage
        );
    }

    private void SelectToy(Image clickedImage)
    {
        if (toySelected)
        {
            return;
        }

        toySelected = true;
        score += 10;

        AddOptionalToy(clickedImage);
    }

    private void AddRequiredItem(
        Image slotImage,
        Sprite itemSprite,
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
                () => ShowSlot(slotImage, itemSprite)
            );
        }
        else
        {
            ShowSlot(slotImage, itemSprite);
        }
    }

    private void AddOptionalToy(Image clickedImage)
    {
        if (
            clickedImage != null &&
            flyAnimator != null &&
            toySlotTarget != null
        )
        {
            flyAnimator.FlyToSlot(
                clickedImage,
                toySlotTarget,
                () => ShowSlot(toySlotImage, toySprite)
            );
        }
        else
        {
            ShowSlot(toySlotImage, toySprite);
        }
    }

    private void ShowSlot(
        Image slotImage,
        Sprite sprite
    )
    {
        if (slotImage == null || sprite == null)
        {
            return;
        }

        slotImage.sprite = sprite;
        slotImage.color = Color.white;
        slotImage.enabled = true;
    }

    private void HideSlot(Image slotImage)
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
            currentRequiredCount < requiredCount
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
        yield return new WaitForSecondsRealtime(0.7f);

        if (
            toySelected &&
            !toyRemovedFromBag
        )
        {
            toyRemovedFromBag = true;

            if (toySlotImage != null)
            {
                toySlotImage.enabled = false;
            }

            if (warningPopup != null)
            {
                yield return warningPopup.ShowAndWaitForClose(
                    "Oyuncaðýný yanýna alabilirsin.\n" +
                    "Ama o ameliyata giremez,\n" +
                    "odanda seni bekleyecek."
                );
            }
        }

        if (warningPopup != null)
        {
            yield return warningPopup.ShowAndWaitForClose(
                "Harikasýn!\nÇantan hazýr."
            );
        }

        CompleteStageAndReturnToMap();
    }

    private void CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            return;
        }

        isReturningToMap = true;

        StageProgress.CompleteStage(stageNumber);

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
            string.IsNullOrWhiteSpace(mapSceneName)
        )
        {
            Debug.LogError(
                "Map Scene Name alaný boþ."
            );

            yield break;
        }

        SceneManager.LoadScene(mapSceneName);
    }

    private IEnumerator RestartStageRoutine(
        string warningMessage
    )
    {
        stageCompleted = true;

        if (warningPopup != null)
        {
            yield return warningPopup.ShowAndWaitForClose(
                warningMessage
            );
        }

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void ShowWarning(string message)
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
}