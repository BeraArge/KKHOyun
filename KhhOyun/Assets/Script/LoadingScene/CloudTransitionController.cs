using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudTransitionController : MonoBehaviour
{
    public static CloudTransitionController Instance;

    [Header("Bulutlar")]
    [SerializeField] private RectTransform leftCloudBottom;
    [SerializeField] private RectTransform leftCloudTop;
    [SerializeField] private RectTransform rightCloudTop;
    [SerializeField] private RectTransform rightCloudBottom;

    [Header("Açýk Konum Targetlarý")]
    [SerializeField] private RectTransform leftBottomOpenTarget;
    [SerializeField] private RectTransform leftTopOpenTarget;
    [SerializeField] private RectTransform rightTopOpenTarget;
    [SerializeField] private RectTransform rightBottomOpenTarget;

    [Header("Açýlma Ayarlarý")]
    [SerializeField] private float openDuration = 0.85f;
    [SerializeField] private float topCloudDelay = 0.06f;
    [SerializeField] private float newSceneWaitDuration = 0.15f;

    private Vector2 leftBottomClosedPos;
    private Vector2 leftTopClosedPos;
    private Vector2 rightTopClosedPos;
    private Vector2 rightBottomClosedPos;

    private Vector3 leftBottomClosedScale;
    private Vector3 leftTopClosedScale;
    private Vector3 rightTopClosedScale;
    private Vector3 rightBottomClosedScale;

    private bool isOpening;
    private bool waitingForTargetScene;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CaptureClosedState()
    {
        if (!CheckReferences())
        {
            Debug.LogError(
                "CloudTransitionController referanslarý eksik."
            );

            return;
        }

        leftBottomClosedPos =
            leftCloudBottom.anchoredPosition;

        leftTopClosedPos =
            leftCloudTop.anchoredPosition;

        rightTopClosedPos =
            rightCloudTop.anchoredPosition;

        rightBottomClosedPos =
            rightCloudBottom.anchoredPosition;

        leftBottomClosedScale =
            leftCloudBottom.localScale;

        leftTopClosedScale =
            leftCloudTop.localScale;

        rightTopClosedScale =
            rightCloudTop.localScale;

        rightBottomClosedScale =
            rightCloudBottom.localScale;

        /*
         * Bundan sonra açýlacak ilk sahnede
         * bulutlarýn açýlmasý gerektiðini belirtir.
         */
        waitingForTargetScene = true;

        Debug.Log(
            "Bulutlarýn kapalý durumu kaydedildi. " +
            "Hedef sahne bekleniyor."
        );
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode loadSceneMode
    )
    {
        if (!waitingForTargetScene || isOpening)
        {
            return;
        }

        /*
         * LoadingScene ilk açýldýðýnda deðil,
         * CaptureClosedState çaðrýldýktan sonra açýlan
         * hedef sahnede çalýþýr.
         */
        waitingForTargetScene = false;

        StartCoroutine(
            OpenAndDestroyRoutine()
        );
    }

    public void OpenClouds()
    {
        if (isOpening)
        {
            return;
        }

        waitingForTargetScene = false;

        StartCoroutine(
            OpenAndDestroyRoutine()
        );
    }

    private IEnumerator OpenAndDestroyRoutine()
    {
        isOpening = true;

        /*
         * Yeni sahnenin Canvas ve görsellerinin
         * oluþmasý için kýsa süre bekler.
         */
        yield return null;
        yield return null;

        if (newSceneWaitDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                newSceneWaitDuration
            );
        }

        Debug.Log(
            "Bulut açýlma animasyonu baþladý."
        );

        yield return OpenCloudsRoutine();

        Debug.Log(
            "Bulutlar açýldý. CloudTransitionCanvas siliniyor."
        );

        if (Instance == this)
        {
            Instance = null;
        }

        Destroy(gameObject);
    }

    private IEnumerator OpenCloudsRoutine()
    {
        if (!CheckReferences())
        {
            Debug.LogError(
                "Bulut açýlma referanslarý eksik."
            );

            yield break;
        }

        float totalDuration =
            openDuration + topCloudDelay;

        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float bottomT = Mathf.Clamp01(
                elapsedTime / openDuration
            );

            float topT = Mathf.Clamp01(
                (elapsedTime - topCloudDelay) /
                openDuration
            );

            bottomT = EaseInOutCubic(bottomT);
            topT = EaseInOutCubic(topT);

            AnimateCloud(
                leftCloudBottom,
                leftBottomClosedPos,
                leftBottomOpenTarget.anchoredPosition,
                leftBottomClosedScale,
                leftBottomOpenTarget.localScale,
                bottomT
            );

            AnimateCloud(
                rightCloudBottom,
                rightBottomClosedPos,
                rightBottomOpenTarget.anchoredPosition,
                rightBottomClosedScale,
                rightBottomOpenTarget.localScale,
                bottomT
            );

            AnimateCloud(
                leftCloudTop,
                leftTopClosedPos,
                leftTopOpenTarget.anchoredPosition,
                leftTopClosedScale,
                leftTopOpenTarget.localScale,
                topT
            );

            AnimateCloud(
                rightCloudTop,
                rightTopClosedPos,
                rightTopOpenTarget.anchoredPosition,
                rightTopClosedScale,
                rightTopOpenTarget.localScale,
                topT
            );

            yield return null;
        }

        SetFinalOpenState();
    }

    private bool CheckReferences()
    {
        return
            leftCloudBottom != null &&
            leftCloudTop != null &&
            rightCloudTop != null &&
            rightCloudBottom != null &&
            leftBottomOpenTarget != null &&
            leftTopOpenTarget != null &&
            rightTopOpenTarget != null &&
            rightBottomOpenTarget != null;
    }

    private void AnimateCloud(
        RectTransform cloud,
        Vector2 startPosition,
        Vector2 targetPosition,
        Vector3 startScale,
        Vector3 targetScale,
        float progress
    )
    {
        if (cloud == null)
        {
            return;
        }

        cloud.anchoredPosition =
            Vector2.Lerp(
                startPosition,
                targetPosition,
                progress
            );

        cloud.localScale =
            Vector3.Lerp(
                startScale,
                targetScale,
                progress
            );
    }

    private void SetFinalOpenState()
    {
        SetCloudOpenState(
            leftCloudBottom,
            leftBottomOpenTarget
        );

        SetCloudOpenState(
            leftCloudTop,
            leftTopOpenTarget
        );

        SetCloudOpenState(
            rightCloudTop,
            rightTopOpenTarget
        );

        SetCloudOpenState(
            rightCloudBottom,
            rightBottomOpenTarget
        );
    }

    private void SetCloudOpenState(
        RectTransform cloud,
        RectTransform target
    )
    {
        if (cloud == null || target == null)
        {
            return;
        }

        cloud.anchoredPosition =
            target.anchoredPosition;

        cloud.localScale =
            target.localScale;
    }

    private float EaseInOutCubic(float x)
    {
        return x < 0.5f
            ? 4f * x * x * x
            : 1f -
              Mathf.Pow(
                  -2f * x + 2f,
                  3f
              ) / 2f;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }
}