using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class StageMapItem : MonoBehaviour, IPointerClickHandler
{
    [Header("Aþama Bilgisi")]
    [SerializeField] private int stageNumber;
    [SerializeField] private string sceneName;

    [Header("Durum Ýkonlarý")]
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject completedIcon;

    [Header("Kilitli Aþama Görünümü")]
    [SerializeField]
    private Color lockedColor =
        new Color(0.45f, 0.45f, 0.45f, 0.60f);

    [Header("Aktif Aþama Görünümü")]
    [Range(1f, 1.30f)]
    [SerializeField] private float currentScale = 1.15f;

    private Image stageImage;
    private Vector3 normalScale;
    private StageMapManager manager;

    public int StageNumber => stageNumber;
    public string SceneName => sceneName;

    private void Awake()
    {
        stageImage = GetComponent<Image>();
        normalScale = transform.localScale;

        // Ana aþama görselinin týklama alabilmesi için açýk olmalý.
        stageImage.raycastTarget = true;

        FindManager();
    }

    private void Start()
    {
        // Awake sýrasýnda bulunamazsa tekrar arar.
        if (manager == null)
        {
            FindManager();
        }
    }

    private void FindManager()
    {
        manager = GetComponentInParent<StageMapManager>();

        if (manager == null)
        {
            manager = FindFirstObjectByType<StageMapManager>();
        }
    }

    public void SetVisualState(
        bool isLocked,
        bool isCurrent,
        bool isCompleted
    )
    {
        if (stageImage == null)
        {
            stageImage = GetComponent<Image>();
        }

        if (isLocked)
        {
            ApplyLockedState();
            return;
        }

        if (isCurrent)
        {
            ApplyCurrentState();
            return;
        }

        if (isCompleted)
        {
            ApplyCompletedState();
            return;
        }

        ApplyCurrentState();
    }

    private void ApplyLockedState()
    {
        // Kilitli aþama gri ve soluk görünür.
        stageImage.color = lockedColor;
        transform.localScale = normalScale;

        SetIconState(
            showLock: true,
            showCompleted: false
        );
    }

    private void ApplyCurrentState()
    {
        // Aktif aþama tam renkli ve diðerlerinden daha büyük görünür.
        stageImage.color = Color.white;
        transform.localScale = normalScale * currentScale;

        // Aktif aþamayý diðer UI görsellerinin önüne getirir.
        transform.SetAsLastSibling();

        SetIconState(
            showLock: false,
            showCompleted: false
        );
    }

    private void ApplyCompletedState()
    {
        // Tamamlanan aþamanýn rengi normal kalýr.
        stageImage.color = Color.white;
        transform.localScale = normalScale;

        SetIconState(
            showLock: false,
            showCompleted: true
        );
    }

    private void SetIconState(
        bool showLock,
        bool showCompleted
    )
    {
        if (lockIcon != null)
        {
            lockIcon.SetActive(showLock);
        }

        if (completedIcon != null)
        {
            completedIcon.SetActive(showCompleted);
        }
    }

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (manager == null)
        {
            FindManager();
        }

        if (manager == null)
        {
            Debug.LogError(
                $"{gameObject.name} için StageMapManager bulunamadý. " +
                "StageMapManager scriptini 1–7 nesnelerinin parent'ý " +
                "olan Sahne nesnesine ekleyin."
            );

            return;
        }

        manager.StageClicked(stageNumber);
    }
}