using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [Header("Loading Bar")]
    [SerializeField] private Image barFill;

    [Header("Bar Ayarlarý")]
    [SerializeField] private float fillDuration = 3f;
    [SerializeField] private float startDelay = 0.15f;
    [SerializeField] private float afterFillDelay = 1.2f;

    [Header("Bulutlar")]
    [SerializeField] private RectTransform leftCloudBottom;
    [SerializeField] private RectTransform leftCloudTop;
    [SerializeField] private RectTransform rightCloudTop;
    [SerializeField] private RectTransform rightCloudBottom;

    [Header("Kapalý Konum Targetlarý")]
    [SerializeField] private RectTransform leftBottomClosedTarget;
    [SerializeField] private RectTransform leftTopClosedTarget;
    [SerializeField] private RectTransform rightTopClosedTarget;
    [SerializeField] private RectTransform rightBottomClosedTarget;

    [Header("Bulut Animasyonu")]
    [SerializeField] private float cloudCloseDuration = 1.05f;
    [SerializeField] private float topCloudDelay = 0.04f;

    [Header("Kapalý Durum Ölçekleri")]
    [SerializeField] private float bottomCloudClosedScale = 3.2f;
    [SerializeField] private float topCloudClosedScale = 3f;

    private Vector2 leftBottomOpenPos;
    private Vector2 leftTopOpenPos;
    private Vector2 rightTopOpenPos;
    private Vector2 rightBottomOpenPos;

    private Vector3 leftBottomOpenScale;
    private Vector3 leftTopOpenScale;
    private Vector3 rightTopOpenScale;
    private Vector3 rightBottomOpenScale;

    private Coroutine loadingRoutine;
    private bool isLoadingTargetScene;

    private string targetSceneName;

    private void Awake()
    {
        if (barFill != null)
        {
            barFill.fillAmount = 0f;
            barFill.enabled = false;
        }

        CacheOpenState();
    }

    private void Start()
    {
        /*
         * Map sahnesinde seçilen aþamanýn adý
         * LoadingSceneData üzerinden alýnýr.
         */
        targetSceneName =
            LoadingSceneData.TargetSceneName;

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError(
                "LoadingSceneManager: Hedef sahne bilgisi bulunamadý. " +
                "LoadingScene, StageMapManager üzerinden açýlmalýdýr."
            );

            return;
        }

        if (barFill == null)
        {
            Debug.LogError(
                "LoadingSceneManager: BarFill referansý atanmadý."
            );

            return;
        }

        if (!CheckCloudReferences())
        {
            Debug.LogError(
                "LoadingSceneManager: Bulut veya kapalý target referanslarý eksik."
            );

            return;
        }

        if (CloudTransitionController.Instance == null)
        {
            Debug.LogError(
                "LoadingSceneManager: CloudTransitionController bulunamadý. " +
                "Scriptin CloudTransitionCanvas üzerinde olduðundan emin olun."
            );

            return;
        }

        loadingRoutine = StartCoroutine(
            LoadingSequenceRoutine()
        );
    }

    private void CacheOpenState()
    {
        if (leftCloudBottom != null)
        {
            leftBottomOpenPos =
                leftCloudBottom.anchoredPosition;

            leftBottomOpenScale =
                leftCloudBottom.localScale;
        }

        if (leftCloudTop != null)
        {
            leftTopOpenPos =
                leftCloudTop.anchoredPosition;

            leftTopOpenScale =
                leftCloudTop.localScale;
        }

        if (rightCloudTop != null)
        {
            rightTopOpenPos =
                rightCloudTop.anchoredPosition;

            rightTopOpenScale =
                rightCloudTop.localScale;
        }

        if (rightCloudBottom != null)
        {
            rightBottomOpenPos =
                rightCloudBottom.anchoredPosition;

            rightBottomOpenScale =
                rightCloudBottom.localScale;
        }
    }

    private bool CheckCloudReferences()
    {
        return
            leftCloudBottom != null &&
            leftCloudTop != null &&
            rightCloudTop != null &&
            rightCloudBottom != null &&
            leftBottomClosedTarget != null &&
            leftTopClosedTarget != null &&
            rightTopClosedTarget != null &&
            rightBottomClosedTarget != null;
    }

    private IEnumerator LoadingSequenceRoutine()
    {
        yield return null;

        barFill.fillAmount = 0f;
        barFill.enabled = true;

        Canvas.ForceUpdateCanvases();

        if (startDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                startDelay
            );
        }

        yield return FillBarRoutine();

        if (afterFillDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(
                afterFillDelay
            );
        }

        yield return CloseCloudsRoutine();

        Debug.Log(
            "Loading kapanýþ animasyonu tamamlandý."
        );

        CloudTransitionController controller =
            CloudTransitionController.Instance;

        if (controller == null)
        {
            Debug.LogError(
                "Bulutlar kapandý ancak CloudTransitionController bulunamadý."
            );

            yield break;
        }

        /*
         * Yeni sahnede bulutlarýn hangi kapalý konumdan
         * açýlacaðýný kaydeder.
         */
        controller.CaptureClosedState();

        LoadTargetScene();
    }

    private IEnumerator FillBarRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float normalProgress =
                Mathf.Clamp01(
                    elapsedTime / fillDuration
                );

            float smoothProgress =
                SmootherStep(normalProgress);

            barFill.fillAmount =
                smoothProgress;

            yield return null;
        }

        barFill.fillAmount = 1f;
    }

    private IEnumerator CloseCloudsRoutine()
    {
        Vector2 leftBottomClosedPos =
            leftBottomClosedTarget.anchoredPosition;

        Vector2 leftTopClosedPos =
            leftTopClosedTarget.anchoredPosition;

        Vector2 rightTopClosedPos =
            rightTopClosedTarget.anchoredPosition;

        Vector2 rightBottomClosedPos =
            rightBottomClosedTarget.anchoredPosition;

        Vector3 leftBottomClosedScale =
            GetScaledVector(
                leftBottomOpenScale,
                bottomCloudClosedScale
            );

        Vector3 rightBottomClosedScale =
            GetScaledVector(
                rightBottomOpenScale,
                bottomCloudClosedScale
            );

        Vector3 leftTopClosedScale =
            GetScaledVector(
                leftTopOpenScale,
                topCloudClosedScale
            );

        Vector3 rightTopClosedScale =
            GetScaledVector(
                rightTopOpenScale,
                topCloudClosedScale
            );

        float totalDuration =
            cloudCloseDuration + topCloudDelay;

        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float bottomT =
                Mathf.Clamp01(
                    elapsedTime /
                    cloudCloseDuration
                );

            float topT =
                Mathf.Clamp01(
                    (elapsedTime - topCloudDelay) /
                    cloudCloseDuration
                );

            bottomT =
                EaseInOutCubic(bottomT);

            topT =
                EaseInOutCubic(topT);

            AnimateCloud(
                leftCloudBottom,
                leftBottomOpenPos,
                leftBottomClosedPos,
                leftBottomOpenScale,
                leftBottomClosedScale,
                bottomT
            );

            AnimateCloud(
                rightCloudBottom,
                rightBottomOpenPos,
                rightBottomClosedPos,
                rightBottomOpenScale,
                rightBottomClosedScale,
                bottomT
            );

            AnimateCloud(
                leftCloudTop,
                leftTopOpenPos,
                leftTopClosedPos,
                leftTopOpenScale,
                leftTopClosedScale,
                topT
            );

            AnimateCloud(
                rightCloudTop,
                rightTopOpenPos,
                rightTopClosedPos,
                rightTopOpenScale,
                rightTopClosedScale,
                topT
            );

            yield return null;
        }

        SetCloudFinalState(
            leftCloudBottom,
            leftBottomClosedPos,
            leftBottomClosedScale
        );

        SetCloudFinalState(
            rightCloudBottom,
            rightBottomClosedPos,
            rightBottomClosedScale
        );

        SetCloudFinalState(
            leftCloudTop,
            leftTopClosedPos,
            leftTopClosedScale
        );

        SetCloudFinalState(
            rightCloudTop,
            rightTopClosedPos,
            rightTopClosedScale
        );
    }

    private void LoadTargetScene()
    {
        if (isLoadingTargetScene)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            Debug.LogError(
                "LoadingSceneManager: Hedef sahne adý boþ."
            );

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError(
                $"'{targetSceneName}' sahnesi yüklenemiyor. " +
                "Build Profiles içindeki Scene List'e ekleyin."
            );

            return;
        }

        isLoadingTargetScene = true;

        Debug.Log(
            $"Hedef sahne açýlýyor: {targetSceneName}"
        );

        /*
         * Hedef sahne adýný artýk kullanmayacaðýmýz için temizleriz.
         * targetSceneName deðiþkeninde yerel kopyasý duruyor.
         */
        LoadingSceneData.Clear();

        /*
         * LoadingScene kapanýr.
         * CloudTransitionCanvas, DontDestroyOnLoad sayesinde kalýr.
         * Yeni sahne açýldýðýnda bulutlar açýlýr ve Canvas silinir.
         */
        SceneManager.LoadScene(
            targetSceneName
        );
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

    private void SetCloudFinalState(
        RectTransform cloud,
        Vector2 targetPosition,
        Vector3 targetScale
    )
    {
        if (cloud == null)
        {
            return;
        }

        cloud.anchoredPosition =
            targetPosition;

        cloud.localScale =
            targetScale;
    }

    private Vector3 GetScaledVector(
        Vector3 originalScale,
        float scaleMultiplier
    )
    {
        return new Vector3(
            originalScale.x * scaleMultiplier,
            originalScale.y * scaleMultiplier,
            originalScale.z
        );
    }

    private float SmootherStep(float x)
    {
        return
            x * x * x *
            (
                x *
                (
                    x * 6f - 15f
                ) +
                10f
            );
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

    private void OnDisable()
    {
        loadingRoutine = null;
    }
}