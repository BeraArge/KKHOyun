using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage4FarmManager : MonoBehaviour
{
    [Header("E�itim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bile�eni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI i�indeki Steps listesinde kullan�lacak ad�m kimli�i.")]
    [SerializeField]
    private string educationStepId = "asama4_egitim1";

    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Fly Animation")]
    public ItemFlyAnimator flyAnimator;

    [Header("B�l�m Ge�i�i")]
    [Tooltip("Sa�l�kl� besinlerin topland��� Farm b�l�m�n�n ana objesi.")]
    public GameObject farmContainer;

    [Tooltip("Farm b�l�m� tamamland�ktan sonra a��lacak Dengeli Tabak b�l�m�.")]
    public GameObject plateContainer;

    [Header("Collected Food Slots")]
    public Image[] collectedSlots;
    public RectTransform[] collectedSlotTargets;

    [Header("Game Values")]
    public int score = 0;
    public int requiredHealthyFoodCount = 7;
    public int collectedHealthyFoodCount = 0;

    [Header("A�ama Ge�i�i")]
    [Tooltip("Bu de�er, A�ama 4'�n Plate b�l�m�ndeki stageNumber ile ayn� olmal�d�r.")]
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

        // E�itim bitene kadar iki oyun b�l�m� de kapal� kal�r.
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
                "Stage4FarmManager: Education Panel atanmad�. " +
                "E�itim ad�m� atlan�yor."
            );
        }

        educationIsOpen = false;

        // E�itim kapand�ktan sonra Farm b�l�m� a��l�r.
        if (farmContainer != null)
        {
            farmContainer.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "Stage4FarmManager: Farm Container atanmad�."
            );
        }

        // Dengeli Tabak, Farm tamamlanana kadar kapal� kal�r.
        if (plateContainer != null)
        {
            plateContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] E�itim tamamland�. Farm b�l�m� ba�lad�."
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
                    "Hamburgeri s�k t�ketmemeliyiz.\n" +
                    "Kalbine daha faydal� besinleri se�elim."
                );

                break;

            case "cips":

                score -= 10;

                ShowWarning(
                    "Cips yerine daha sa�l�kl� at��t�rmal�klar se�ebilirsin."
                );

                break;

            case "kola":
            case "soda":

                score -= 10;

                ShowWarning(
                    "Gazl� i�ecekler sa�l�kl� bir se�im de�ildir."
                );

                break;

            case "tuz":

                score -= 15;

                ShowWarning(
                    "A��r� tuz kalbini yorabilir."
                );

                break;

            default:

                Debug.LogWarning(
                    "Tan�ms�z besin ad�: " +
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
                " i�in Fly Image atanmad�."
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
                "Toplanan besin i�in yeterli slot veya hedef bulunamad�."
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
                    "Harikas�n!\n" +
                    "Sa�l�kl� besinleri toplad�n.\n" +
                    "�imdi dengeli tabak haz�rlayabiliriz!"
                );
        }
        else
        {
            Debug.LogWarning(
                "Harikas�n! Sa�l�kl� besinleri toplad�n. " +
                "�imdi dengeli tabak haz�rlayabiliriz!"
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
                "Stage4FarmManager: Plate Container atanmad�."
            );
        }

        if (farmContainer != null)
        {
            farmContainer.SetActive(false);
        }

        Debug.Log(
            "[Stage4FarmManager] Farm b�l�m� tamamland�. " +
            "Dengeli Tabak b�l�m�ne ge�ildi."
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