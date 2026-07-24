using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4FarmManager : MonoBehaviour
{
    [Header("Eðitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileþeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanýlacak adým kimliði.")]
    [SerializeField]
    private string educationStepId = "asama4_egitim1";

    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Bölüm Geçiþi")]
    [Tooltip("Saðlýklý besinlerin toplandýðý Farm bölümünün ana objesi.")]
    public GameObject farmContainer;

    [Tooltip("Farm bölümü tamamlandýktan sonra açýlacak Dengeli Tabak bölümü.")]
    public GameObject plateContainer;

    [Header("Collected Food Slots")]
    public Image[] collectedSlots;
    public RectTransform[] collectedSlotTargets;

    [Header("Game Values")]
    public int score = 0;
    public int requiredHealthyFoodCount = 7;
    public int collectedHealthyFoodCount = 0;

    private bool stageCompleted;
    private bool transitionInProgress;
    private bool educationIsOpen;

    private readonly HashSet<string> collectedFoods =
        new HashSet<string>();

    private void Start()
    {
        stageCompleted = false;
        transitionInProgress = false;
        educationIsOpen = true;

        // Eðitim bitene kadar iki oyun bölümü de kapalý kalýr.
        if (farmContainer != null)
        {
            farmContainer.SetActive(false);
        }

        if (plateContainer != null)
        {
            plateContainer.SetActive(false);
        }

        if (collectedSlots != null)
        {
            foreach (Image slot in collectedSlots)
            {
                if (slot != null)
                {
                    slot.enabled = false;
                }
            }
        }

        StartCoroutine(
            StartEducationThenFarm()
        );
    }

    private IEnumerator StartEducationThenFarm()
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
                "Stage4FarmManager: Education Panel atanmadý. " +
                "Eðitim adýmý atlanýyor."
            );
        }

        educationIsOpen = false;

        // Eðitim kapandýktan sonra Farm bölümü açýlýr.
        if (farmContainer != null)
        {
            farmContainer.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Stage4FarmManager: Farm Container atanmadý."
            );
        }

        // Dengeli Tabak, Farm tamamlanana kadar kapalý kalýr.
        if (plateContainer != null)
        {
            plateContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] Eðitim tamamlandý. Farm bölümü baþladý."
        );
    }

    public void SelectFood(
        string foodName,
        Image clickedImage = null
    )
    {
        if (
            educationIsOpen ||
            stageCompleted ||
            transitionInProgress
        )
        {
            return;
        }

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

                SelectHealthyFood(
                    foodName,
                    clickedImage
                );

                break;

            case "hamburger":

                score -= 10;

                ShowWarning(
                    "Hamburgeri sýk tüketmemeliyiz.\n" +
                    "Kalbine daha faydalý besinleri seçelim."
                );

                break;

            case "cips":

                score -= 10;

                ShowWarning(
                    "Cips yerine daha saðlýklý atýþtýrmalýklar seçebilirsin."
                );

                break;

            case "kola":
            case "soda":

                score -= 10;

                ShowWarning(
                    "Gazlý içecekler saðlýklý bir seçim deðildir."
                );

                break;

            case "tuz":

                score -= 15;

                ShowWarning(
                    "Aþýrý tuz kalbini yorabilir."
                );

                break;

            default:

                Debug.LogWarning(
                    "Tanýmsýz besin adý: " +
                    foodName
                );

                break;
        }

        CheckCompletion();
    }

    private void SelectHealthyFood(
        string foodName,
        Image clickedImage
    )
    {
        if (collectedFoods.Contains(foodName))
        {
            return;
        }

        if (clickedImage == null)
        {
            Debug.LogError(
                foodName +
                " için Fly Image atanmadý."
            );

            return;
        }

        collectedFoods.Add(foodName);

        score += 10;

        int slotIndex =
            collectedHealthyFoodCount;

        collectedHealthyFoodCount++;

        if (
            collectedSlots == null ||
            collectedSlotTargets == null ||
            slotIndex >= collectedSlots.Length ||
            slotIndex >= collectedSlotTargets.Length
        )
        {
            Debug.LogWarning(
                "Toplanan besin için yeterli slot veya hedef bulunamadý."
            );

            return;
        }

        Image targetSlot =
            collectedSlots[slotIndex];

        RectTransform targetRect =
            collectedSlotTargets[slotIndex];

        if (
            flyAnimator != null &&
            targetRect != null
        )
        {
            flyAnimator.FlyToSlot(
                clickedImage,
                targetRect,
                () =>
                {
                    ShowSlot(
                        targetSlot,
                        clickedImage.sprite
                    );
                }
            );
        }
        else
        {
            ShowSlot(
                targetSlot,
                clickedImage.sprite
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

    private void CheckCompletion()
    {
        if (
            transitionInProgress ||
            stageCompleted ||
            collectedHealthyFoodCount <
            requiredHealthyFoodCount
        )
        {
            return;
        }

        transitionInProgress = true;

        StartCoroutine(
            CompleteFarmRoutine()
        );
    }

    private IEnumerator CompleteFarmRoutine()
    {
        yield return new WaitForSecondsRealtime(
            0.7f
        );

        if (warningPopup != null)
        {
            yield return warningPopup
                .ShowAndWaitForClose(
                    "Harikasýn!\n" +
                    "Saðlýklý besinleri topladýn.\n" +
                    "Þimdi dengeli tabak hazýrlayabiliriz!"
                );
        }
        else
        {
            Debug.LogWarning(
                "Harikasýn! Saðlýklý besinleri topladýn. " +
                "Þimdi dengeli tabak hazýrlayabiliriz!"
            );
        }

        stageCompleted = true;

        if (plateContainer != null)
        {
            plateContainer.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Stage4FarmManager: Plate Container atanmadý."
            );
        }

        if (farmContainer != null)
        {
            farmContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] Farm bölümü tamamlandý. " +
            "Dengeli Tabak bölümüne geçildi."
        );
    }

    private void ShowWarning(
        string message
    )
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