using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4FarmManager : MonoBehaviour
{
    [Header("Eğitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileşeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanılacak adım kimliği.")]
    [SerializeField]
    private string educationStepId = "asama4_egitim1";

    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("Bölüm Geçişi")]
    [Tooltip("Sağlıklı besinlerin toplandığı Farm bölümünün ana objesi.")]
    public GameObject farmContainer;

    [Tooltip("Farm bölümü tamamlandıktan sonra açılacak Dengeli Tabak bölümü.")]
    public GameObject plateContainer;

    [Header("Collected Food Slots")]
    public Image[] collectedSlots;
    public RectTransform[] collectedSlotTargets;

    [Header("Game Values")]
    public int score = 0;
    public int requiredHealthyFoodCount = 7;
    public int collectedHealthyFoodCount = 0;

    [Header("Aşama Geçişi")]
    [Tooltip("Bu değer, Aşama 4'ün Plate bölümündeki stageNumber ile aynı olmalıdır.")]
    [SerializeField]
    private int stageNumber = 4;

    private bool stageCompleted;
    private bool transitionInProgress;
    private bool educationIsOpen;

    private readonly HashSet<string> collectedFoods =
        new HashSet<string>();

    private void Start()
    {
        StageProgress.EnterStage(stageNumber);

        stageCompleted = false;
        transitionInProgress = false;
        educationIsOpen = true;

        // Eğitim bitene kadar iki oyun bölümü de kapalı kalır.
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
                "Stage4FarmManager: Education Panel atanmadı. " +
                "Eğitim adımı atlanıyor."
            );
        }

        educationIsOpen = false;

        // Eğitim kapandıktan sonra Farm bölümü açılır.
        if (farmContainer != null)
        {
            farmContainer.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Stage4FarmManager: Farm Container atanmadı."
            );
        }

        // Dengeli Tabak, Farm tamamlanana kadar kapalı kalır.
        if (plateContainer != null)
        {
            plateContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] Eğitim tamamlandı. Farm bölümü başladı."
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
                    "Hamburgeri sık tüketmemeliyiz.\n" +
                    "Kalbine daha faydalı besinleri seçelim."
                );

                break;

            case "cips":

                score -= 10;

                ShowWarning(
                    "Cips yerine daha sağlıklı atıştırmalıklar seçebilirsin."
                );

                break;

            case "kola":
            case "soda":

                score -= 10;

                ShowWarning(
                    "Gazlı içecekler sağlıklı bir seçim değildir."
                );

                break;

            case "tuz":

                score -= 15;

                ShowWarning(
                    "Aşırı tuz kalbini yorabilir."
                );

                break;

            default:

                Debug.LogWarning(
                    "Tanımsız besin adı: " +
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
                " için Fly Image atanmadı."
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
                "Toplanan besin için yeterli slot veya hedef bulunamadı."
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
                    "Harikasın!\n" +
                    "Sağlıklı besinleri topladın.\n" +
                    "Şimdi dengeli tabak hazırlayabiliriz!"
                );
        }
        else
        {
            Debug.LogWarning(
                "Harikasın! Sağlıklı besinleri topladın. " +
                "Şimdi dengeli tabak hazırlayabiliriz!"
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
                "Stage4FarmManager: Plate Container atanmadı."
            );
        }

        if (farmContainer != null)
        {
            farmContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] Farm bölümü tamamlandı. " +
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