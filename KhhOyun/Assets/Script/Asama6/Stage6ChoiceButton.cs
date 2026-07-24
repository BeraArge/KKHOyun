using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage6ChoiceButton : MonoBehaviour, IPointerClickHandler
{
    public enum ChoiceType
    {
        DevamEt,
        Saklan,
        YardimNoktasinaGit,
        YardimIste,
        SolaDon,
        SagaDon,
        Aile,
        SaglikNoktasi
    }

    [Header("Seçim")]
    public ChoiceType choiceType;

    [Header("Manager")]
    public Stage6Manager stage6Manager;

    [Header("Görsel")]
    [Tooltip("Kartýn tamamýný soldurmak için bu objeye CanvasGroup ekleyin.")]
    public CanvasGroup canvasGroup;

    [Tooltip("CanvasGroup kullanýlmýyorsa ana görsel buraya atanabilir.")]
    public Image targetImage;

    [Tooltip("Objede Button bileþeni varsa buraya atayýn.")]
    public Button button;

    private bool isDisabled;

    private void Awake()
    {
        // Inspector'da atanmadýysa ayný objeden bulmaya çalýþýr.
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (targetImage == null)
        {
            targetImage = GetComponent<Image>();
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDisabled)
        {
            return;
        }

        if (stage6Manager == null)
        {
            Debug.LogError(
                "Stage6Manager atanmadý: " + gameObject.name
            );

            return;
        }

        stage6Manager.MakeChoice(choiceType, this);
    }

    public void SetWrongState()
    {
        isDisabled = true;

        // CanvasGroup bütün çocuk görselleri ve yazýlarý birlikte soldurur.
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.40f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else if (targetImage != null)
        {
            Color color = targetImage.color;
            color.a = 0.40f;
            targetImage.color = color;
        }

        if (button != null)
        {
            button.interactable = false;
        }

        transform.localScale = new Vector3(
            0.96f,
            0.96f,
            1f
        );
    }

    public void ResetVisual()
    {
        isDisabled = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else if (targetImage != null)
        {
            Color color = targetImage.color;
            color.a = 1f;
            targetImage.color = color;
        }

        if (button != null)
        {
            button.interactable = true;
        }

        transform.localScale = Vector3.one;
    }
}