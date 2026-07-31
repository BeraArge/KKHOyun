using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Stage6Manager : MonoBehaviour
{
    private enum Stage6Step
    {
        GirisBilgilendirmesi,
        NefesDarligi,
        GogusAgrisi,
        BasDonmesi,
        SonSecim,
        Tamamlandi
    }

    [Header("Başlangıç Durumu")]
    [SerializeField]
    private Stage6Step currentStep =
        Stage6Step.GirisBilgilendirmesi;

    [Header("Genel Süreler")]
    [SerializeField]
    private float popupFadeDuration = 0.65f;

    [SerializeField]
    private float normalRideDuration = 2.5f;

    [SerializeField]
    private float stepTransitionDelay = 1.5f;

    [SerializeField]
    private float childToBubbleDelay = 1.2f;

    [SerializeField]
    private float bubbleToChoiceDelay = 1.8f;

    [SerializeField]
    private float afterAlertDelay = 1f;

    [SerializeField]
    private float betweenStepsDelay = 2f;

    // --------------------------------------------------
    // EĞİTİM PANELİ
    // --------------------------------------------------

    [Header("Eğitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileşeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanılacak adım kimliği.")]
    [SerializeField]
    private string educationStepId = "asama6_egitim1";

    // --------------------------------------------------
    // BAŞLANGIÇ BİLGİLENDİRME PANELİ
    // --------------------------------------------------

    [Header("Başlangıç Bilgilendirme Paneli")]
    public GameObject introPanel;

    public CanvasGroup introCanvasGroup;

    [SerializeField]
    private float introFadeDuration = 0.8f;

    [Header("Bilgilendirme Paneli Açıklaması")]
    public TMP_Text introDescriptionText;

    [TextArea(3, 6)]
    public string introDescription =
        "Acil durum belirtilerini tanı!\n" +
        "Kendini kötü hissettiçinde doğru seçimi " +
        "yaparak yardım istemelisin.";

    // --------------------------------------------------
    // ÜST PANELLER
    // --------------------------------------------------

    [Header("Sol Üst Bilgilendirme Paneli")]
    public TMP_Text topLeftMessageText;

    [Header("Sağ Üst Görev Paneli")]
    public TMP_Text topRightTitleText;
    public TMP_Text topRightDescriptionText;

    // --------------------------------------------------
    // NORMAL ÇOCUK
    // --------------------------------------------------

    [Header("Normal Çocuk")]
    public GameObject normalChild;

    // --------------------------------------------------
    // NEFES DARLIĞI
    // --------------------------------------------------

    [Header("Nefes Darlığı")]
    public GameObject breathChild;
    public GameObject breathSpeechBubble;
    public GameObject breathChoicePanel;

    [Header("Nefes Seçim Butonları")]
    public Stage6ChoiceButton breathContinueButton;
    public Stage6ChoiceButton breathHideButton;
    public Stage6ChoiceButton breathHelpButton;

    [Header("Nefes Darlığı Başarı Popupı")]
    public GameObject breathCorrectPopup;
    public CanvasGroup breathCorrectPopupCanvasGroup;

    // --------------------------------------------------
    // GÖĞÜS AĞRISI
    // --------------------------------------------------

    [Header("Göğüs Ağrısı")]
    public GameObject chestPainChild;
    public GameObject chestPainSpeechBubble;
    public GameObject chestPainPanel;

    [Header("Göğüs Ağrısı Sayaç Görselleri")]
    public GameObject chestCountdown3;
    public GameObject chestCountdown2;
    public GameObject chestCountdown1;

    [SerializeField]
    private float chestCountdownStepDuration = 1.2f;

    [Header("Göğüs Ağrısı Başarı Popupı")]
    public GameObject chestCorrectPopup;
    public CanvasGroup chestCorrectPopupCanvasGroup;

    [Header("Göğüs Ağrısı Tekrar Popupı")]
    public GameObject chestRetryPopup;
    public CanvasGroup chestRetryPopupCanvasGroup;


    [SerializeField]
    private float chestChildToBubbleDelay = 1.2f;

    [SerializeField]
    private float chestBubbleToButtonDelay = 1.8f;

    [SerializeField]
    private float chestButtonToCountdownDelay = 0.6f;

    // --------------------------------------------------
    // BAŞ DÖNMESİ
    // --------------------------------------------------

    [Header("Baş Dönmesi")]
    public GameObject dizzinessChild;
    public GameObject dizzinessSpeechBubble;
    public GameObject balancePanel;

    [Header("Denge Mekaniği")]
    public RectTransform balanceMarker;

    [SerializeField]
    private float balanceStartX = 140f;

    [SerializeField]
    private float balanceMinX = -170f;

    [SerializeField]
    private float balanceMaxX = 170f;

    [SerializeField]
    private float balanceMoveAmount = 70f;

    [SerializeField]
    private float balanceSuccessHalfWidth = 35f;

    [SerializeField]
    private float balanceMoveDuration = 0.25f;

    [Header("Denge Başarı Popupı")]
    public GameObject balanceCorrectPopup;
    public CanvasGroup balanceCorrectPopupCanvasGroup;

    [Header("Denge Tekrar Popupı")]
    public GameObject balanceRetryPopup;
    public CanvasGroup balanceRetryPopupCanvasGroup;

    // --------------------------------------------------
    // SON SEÇİM
    // --------------------------------------------------

    [Header("Son Seçim")]
    public GameObject finalChoicePanel;

    [Header("Son Seçim Butonları")]
    public Stage6ChoiceButton familyChoiceButton;
    public Stage6ChoiceButton healthPointChoiceButton;

    [Header("Son Seçim Geri Bildirim Panelleri")]
    [Tooltip("Ailem seçildiçinde açılacak küçük uyarı paneli.")]
    public GameObject finalWarningPanel;

    [Tooltip("Çocuk ve ailesi buluştuktan sonra açılacak başarı paneli.")]
    public GameObject finalSuccessPanel;

    [Header("Ailem Butonu Yanlış Seçim Görünümü")]
    [Tooltip("Ailem butonunun ana Image bileşeni. Alfa azaltılmaz; renk doğrudan değiştirilir.")]
    public Image familyChoiceImage;

    [SerializeField]
    private Color familyWrongTint =
        new Color(0.55f, 0.55f, 0.55f, 1f);

    private Color familyChoiceOriginalColor =
        Color.white;

    [Header("Final Hareket Hedefleri")]
    public RectTransform normalChildRect;

    [Tooltip("Sağlık merkezi önünde duran ayakta Çocuk objesi.")]
    public GameObject standingChild;

    [Tooltip("cocukson objesinin RectTransform bileşeni.")]
    public RectTransform standingChildRect;

    [Tooltip("Bisikletli Çocuğun sağlık merkezine giderken ulaşacağı görünmez hedef.")]
    public RectTransform bikeArrivalPoint;

    public RectTransform familyRectTransform;
    public RectTransform familyTargetPoint;

    [Tooltip("cocuknormal Üzerindeki BicycleIdleMotion scriptini buraya atayön.")]
    public MonoBehaviour normalChildIdleMotion;

    [SerializeField]
    private Vector2 familyStartOffset =
        new Vector2(500f, 0f);

    [SerializeField]
    private float childMoveDuration = 2.8f;

    [SerializeField]
    private float familyMoveDuration = 2.3f;

    [Tooltip("Çocuk ayakta hâline geçtikten sonra ailenin harekete başlamadan Önce bekleyeceği süre.")]
    [SerializeField]
    private float familyMoveStartDelay = 0.8f;

    [SerializeField]
    private float delayBeforeFinalPopup = 1.2f;

    [Header("Final")]
    public GameObject finalCongratulationsPanel;
    public GameObject familyCharacters;

    [Header("Aşama Geçişi")]
    [SerializeField]
    private int stageNumber = 6;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    // --------------------------------------------------
    // PUAN VE KONTROL
    // --------------------------------------------------

    [Header("Puan")]
    public int score;

    private bool inputLocked;
    private bool introConfirmed;
    private bool breathSuccessConfirmed;
    private bool chestHelpPressed;
    private bool chestSuccessConfirmed;
    private bool chestRetryConfirmed;
    private bool balanceSuccessConfirmed;
    private bool balanceRetryConfirmed;
    private bool balanceMoveInProgress;
    private bool finalSuccessConfirmed;
    private bool isReturningToMap;

    private Coroutine mainSequence;
    private Coroutine chestCountdownCoroutine;
    private Coroutine balanceMoveCoroutine;

    // --------------------------------------------------
    // UNITY
    // --------------------------------------------------

    private void Start()
    {
        StageProgress.EnterStage(stageNumber);

        PrepareScene();

        mainSequence = StartCoroutine(
            StartEducationThenIntro()
        );
    }

    private IEnumerator StartEducationThenIntro()
    {
        inputLocked = true;

        HideAllGameplayObjects();

        // Eğitim sırasında yalnızca normal bisikletli Çocuk görünür.
        SetActiveSafe(
            normalChild,
            true
        );

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
                "Stage6Manager: Education Panel atanmadı. " +
                "Aşama 6 eğitimi atlanıyor."
            );
        }

        // Eğitim kapanınca mevcut Aşama 6 başlangıç akışına devam edilir.
        yield return StartCoroutine(
            StartStage6Intro()
        );
    }

    private void PrepareScene()
    {
        inputLocked = true;
        introConfirmed = false;
        breathSuccessConfirmed = false;
        chestHelpPressed = false;
        chestSuccessConfirmed = false;
        chestRetryConfirmed = false;
        balanceSuccessConfirmed = false;
        balanceRetryConfirmed = false;
        balanceMoveInProgress = false;
        finalSuccessConfirmed = false;
        isReturningToMap = false;

        HideAllGameplayObjects();

        SetActiveSafe(
            introPanel,
            false
        );

        SetActiveSafe(
            finalCongratulationsPanel,
            false
        );

        SetActiveSafe(
            familyCharacters,
            false
        );

        SetActiveSafe(
            standingChild,
            false
        );

        SetActiveSafe(
            finalWarningPanel,
            false
        );

        SetActiveSafe(
            finalSuccessPanel,
            false
        );

        if (familyChoiceImage != null)
        {
            familyChoiceOriginalColor =
                familyChoiceImage.color;
        }

        UpdateTopPanelsForIntro();
    }

    // --------------------------------------------------
    // BAŞLANGIÇ BİLGİLENDİRMESİ
    // --------------------------------------------------

    private IEnumerator StartStage6Intro()
    {
        currentStep =
            Stage6Step.GirisBilgilendirmesi;

        inputLocked = true;
        introConfirmed = false;

        HideAllGameplayObjects();

        // Bilgilendirme sırasında normal Çocuk görünür.
        SetActiveSafe(
            normalChild,
            true
        );

        SetTextSafe(
            introDescriptionText,
            introDescription
        );

        UpdateTopPanelsForIntro();

        SetActiveSafe(
            introPanel,
            true
        );

        PrepareCanvasGroup(
            introCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                introCanvasGroup,
                0f,
                1f,
                introFadeDuration
            )
        );

        if (introCanvasGroup != null)
        {
            introCanvasGroup.interactable = true;
            introCanvasGroup.blocksRaycasts = true;
        }

        // Tamam butonuna basılmasını bekler.
        yield return new WaitUntil(
            () => introConfirmed
        );

        if (introCanvasGroup != null)
        {
            introCanvasGroup.interactable = false;
            introCanvasGroup.blocksRaycasts = true;
        }

        yield return StartCoroutine(
            FadeCanvasGroup(
                introCanvasGroup,
                1f,
                0f,
                introFadeDuration
            )
        );

        SetActiveSafe(
            introPanel,
            false
        );

        yield return new WaitForSeconds(
            stepTransitionDelay
        );

        yield return StartCoroutine(
            StartBreathStepSequence()
        );
    }

    /// <summary>
    /// WarningPopup içindeki Tamam butonunun
    /// OnClick olayöna atanmalıdır.
    /// </summary>
    public void ConfirmIntro()
    {
        if (
            currentStep !=
            Stage6Step.GirisBilgilendirmesi
        )
        {
            return;
        }

        if (introConfirmed)
        {
            return;
        }

        introConfirmed = true;
    }

    // --------------------------------------------------
    // SEÇİM YÖNETİMİ
    // --------------------------------------------------

    public void MakeChoice(
        Stage6ChoiceButton.ChoiceType choice,
        Stage6ChoiceButton clickedButton
    )
    {
        if (
            inputLocked ||
            isReturningToMap
        )
        {
            return;
        }

        switch (currentStep)
        {
            case Stage6Step.NefesDarligi:

                HandleBreathChoice(
                    choice,
                    clickedButton
                );

                break;

            case Stage6Step.GogusAgrisi:

                HandleChestPainChoice(
                    choice
                );

                break;

            case Stage6Step.BasDonmesi:

                HandleBalanceChoice(
                    choice
                );

                break;

            case Stage6Step.SonSecim:

                HandleFinalChoice(
                    choice,
                    clickedButton
                );

                break;
        }
    }

    // --------------------------------------------------
    // NEFES DARLIĞI
    // --------------------------------------------------

    private IEnumerator StartBreathStepSequence()
    {
        currentStep =
            Stage6Step.NefesDarligi;

        inputLocked = true;

        HideAllGameplayObjects();
        ResetBreathButtons();

        UpdateTopPanelsForBreathWaiting();

        // Önce normal sürüş.
        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        SetActiveSafe(
            normalChild,
            false
        );

        // Nefes darlığı karakteri.
        SetActiveSafe(
            breathChild,
            true
        );

        UpdateTopPanelsForBreathProblem();

        yield return new WaitForSeconds(
            childToBubbleDelay
        );

        // Konuşma balonu. CanvasGroup varsa görünürlüğü de sıfırlanır.
        ShowObject(
            breathSpeechBubble
        );

        yield return new WaitForSeconds(
            bubbleToChoiceDelay
        );

        // Seçim paneli.
        ShowObject(
            breathChoicePanel
        );

        UpdateTopPanelsForBreathChoice();

        inputLocked = false;

        Debug.Log(
            "Nefes darlığı seçimleri açıldı."
        );
    }

    private void HandleBreathChoice(
        Stage6ChoiceButton.ChoiceType choice,
        Stage6ChoiceButton clickedButton
    )
    {
        switch (choice)
        {
            case Stage6ChoiceButton.ChoiceType
                .YardimNoktasinaGit:

                score += 20;
                inputLocked = true;

                mainSequence = StartCoroutine(
                    CompleteBreathStep()
                );

                break;

            case Stage6ChoiceButton.ChoiceType.DevamEt:
            case Stage6ChoiceButton.ChoiceType.Saklan:

                if (clickedButton != null)
                {
                    clickedButton.SetWrongState();
                }

                break;
        }
    }

    private IEnumerator CompleteBreathStep()
    {
        inputLocked = true;
        breathSuccessConfirmed = false;

        SetActiveSafe(
            breathChoicePanel,
            false
        );

        SetActiveSafe(
            breathSpeechBubble,
            false
        );

        // Doğru seçimde Çocuk normal bisikletli hâline döner.
        SetActiveSafe(
            breathChild,
            false
        );

        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        ShowObject(
            breathCorrectPopup
        );

        PrepareCanvasGroup(
            breathCorrectPopupCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                breathCorrectPopupCanvasGroup,
                0f,
                1f,
                popupFadeDuration
            )
        );

        EnableCanvasInteraction(
            breathCorrectPopupCanvasGroup
        );

        // Popup, içindeki butona basılmadan kapanmaz.
        yield return new WaitUntil(
            () => breathSuccessConfirmed
        );

        DisableCanvasInteraction(
            breathCorrectPopupCanvasGroup
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                breathCorrectPopupCanvasGroup,
                1f,
                0f,
                popupFadeDuration
            )
        );

        SetActiveSafe(
            breathCorrectPopup,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        yield return new WaitForSeconds(
            betweenStepsDelay
        );

        yield return StartCoroutine(
            StartChestPainStepSequence()
        );
    }

    /// <summary>
    /// Nefes başarı popupındaki devam/tamam butonuna atanır.
    /// </summary>
    public void ConfirmBreathSuccess()
    {
        if (
            currentStep !=
            Stage6Step.NefesDarligi
        )
        {
            return;
        }

        breathSuccessConfirmed = true;
    }

    private void ResetBreathButtons()
    {
        if (breathContinueButton != null)
        {
            breathContinueButton.ResetVisual();
        }

        if (breathHideButton != null)
        {
            breathHideButton.ResetVisual();
        }

        if (breathHelpButton != null)
        {
            breathHelpButton.ResetVisual();
        }
    }

    // --------------------------------------------------
    // GÖĞÜS AĞRISI
    // --------------------------------------------------

    private IEnumerator StartChestPainStepSequence()
    {
        currentStep =
            Stage6Step.GogusAgrisi;

        inputLocked = true;
        chestHelpPressed = false;

        StopChestCountdown();
        HideAllGameplayObjects(true);

        UpdateTopPanelsForChestPainWaiting();

        // Önce normal Çocuk kısa süre görünür.
        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        SetActiveSafe(
            normalChild,
            false
        );

        // Göğüs ağrısı karakteri görünür.
        SetActiveSafe(
            chestPainChild,
            true
        );

        UpdateTopPanelsForChestPainProblem();

        yield return new WaitForSeconds(
            chestChildToBubbleDelay
        );

        // Göğsüm ağrıyor konuşma balonu.
        SetActiveSafe(
            chestPainSpeechBubble,
            true
        );

        yield return new WaitForSeconds(
            chestBubbleToButtonDelay
        );

        // Yardım Üste paneli.
        ShowObject(
            chestPainPanel
        );

        yield return new WaitForSeconds(
            chestButtonToCountdownDelay
        );

        inputLocked = false;

        StartChestCountdown();

        Debug.Log(
            "Göğüs ağrısı bölümü başladı."
        );
    }

    private void StartChestCountdown()
    {
        StopChestCountdown();

        chestCountdownCoroutine = StartCoroutine(
            RunChestPainCountdown()
        );
    }

    private IEnumerator RunChestPainCountdown()
    {
        HideChestCountdownObjects();

        // 3
        SetActiveSafe(
            chestCountdown3,
            true
        );

        yield return new WaitForSeconds(
            chestCountdownStepDuration
        );

        if (chestHelpPressed)
        {
            yield break;
        }

        SetActiveSafe(
            chestCountdown3,
            false
        );

        // 2
        SetActiveSafe(
            chestCountdown2,
            true
        );

        yield return new WaitForSeconds(
            chestCountdownStepDuration
        );

        if (chestHelpPressed)
        {
            yield break;
        }

        SetActiveSafe(
            chestCountdown2,
            false
        );

        // 1
        SetActiveSafe(
            chestCountdown1,
            true
        );

        yield return new WaitForSeconds(
            chestCountdownStepDuration
        );

        if (chestHelpPressed)
        {
            yield break;
        }

        SetActiveSafe(
            chestCountdown1,
            false
        );

        chestCountdownCoroutine = null;

        // Süre doldu.
        yield return StartCoroutine(
            ShowChestRetryPopup()
        );
    }

    private void HandleChestPainChoice(
        Stage6ChoiceButton.ChoiceType choice
    )
    {
        if (
            choice !=
            Stage6ChoiceButton.ChoiceType.YardimIste
        )
        {
            return;
        }

        if (chestHelpPressed)
        {
            return;
        }

        chestHelpPressed = true;
        inputLocked = true;

        StopChestCountdown();
        HideChestCountdownObjects();

        mainSequence = StartCoroutine(
            CompleteChestPainStep()
        );

        Debug.Log(
            "Yardım iste butonuna zamanında basıldı."
        );
    }

    private IEnumerator CompleteChestPainStep()
    {
        inputLocked = true;
        chestSuccessConfirmed = false;

        StopChestCountdown();

        SetActiveSafe(
            chestPainPanel,
            false
        );

        SetActiveSafe(
            chestPainSpeechBubble,
            false
        );

        // Başarıda Çocuk normal bisikletli hâline döner.
        SetActiveSafe(
            chestPainChild,
            false
        );

        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        ShowObject(
            chestCorrectPopup
        );

        PrepareCanvasGroup(
            chestCorrectPopupCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                chestCorrectPopupCanvasGroup,
                0f,
                1f,
                popupFadeDuration
            )
        );

        EnableCanvasInteraction(
            chestCorrectPopupCanvasGroup
        );

        // Başarı popupı butona basılmadan kapanmaz.
        yield return new WaitUntil(
            () => chestSuccessConfirmed
        );

        DisableCanvasInteraction(
            chestCorrectPopupCanvasGroup
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                chestCorrectPopupCanvasGroup,
                1f,
                0f,
                popupFadeDuration
            )
        );

        SetActiveSafe(
            chestCorrectPopup,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        yield return new WaitForSeconds(
            betweenStepsDelay
        );

        StartDizzinessStep();
    }

    /// <summary>
    /// Göğüs başarı popupındaki devam/tamam butonuna atanır.
    /// </summary>
    public void ConfirmChestSuccess()
    {
        if (
            currentStep !=
            Stage6Step.GogusAgrisi
        )
        {
            return;
        }

        chestSuccessConfirmed = true;
    }

    private IEnumerator ShowChestRetryPopup()
    {
        inputLocked = true;
        chestHelpPressed = false;
        chestRetryConfirmed = false;

        HideChestCountdownObjects();

        // Başarısız durumda Çocuk göğüs ağrısı hâlinde kalır.
        SetActiveSafe(
            chestPainChild,
            true
        );

        SetActiveSafe(
            normalChild,
            false
        );

        SetActiveSafe(
            chestPainPanel,
            false
        );

        SetActiveSafe(
            chestPainSpeechBubble,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        ShowObject(
            chestRetryPopup
        );

        // Popup açıldığında göğüs ağrısı yaşayan Çocuk
        // kesinlikle ekranda kalmaya devam eder.
        SetActiveSafe(
            chestPainChild,
            true
        );

        SetActiveSafe(
            normalChild,
            false
        );

        PrepareCanvasGroup(
            chestRetryPopupCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                chestRetryPopupCanvasGroup,
                0f,
                1f,
                popupFadeDuration
            )
        );

        EnableCanvasInteraction(
            chestRetryPopupCanvasGroup
        );

        // Tekrar dene butonuna basılana kadar popup açık kalır.
        // Bu sırada başka bir işlem karakteri kapatsa bile
        // göğüs ağrısı yaşayan Çocuk tekrar aktif tutulur.
        while (!chestRetryConfirmed)
        {
            SetActiveSafe(
                chestPainChild,
                true
            );

            SetActiveSafe(
                normalChild,
                false
            );

            yield return null;
        }

        DisableCanvasInteraction(
            chestRetryPopupCanvasGroup
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                chestRetryPopupCanvasGroup,
                1f,
                0f,
                popupFadeDuration
            )
        );

        SetActiveSafe(
            chestRetryPopup,
            false
        );

        // Popup kapandıktan sonra da Çocuk aynı durumda kalır.
        SetActiveSafe(
            chestPainChild,
            true
        );

        SetActiveSafe(
            normalChild,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        // Önce konuşma balonu yeniden görünür.
        ShowObject(
            chestPainSpeechBubble
        );

        yield return new WaitForSeconds(
            chestBubbleToButtonDelay
        );

        // Sonra yardım butonu yeniden açılır.
        ShowObject(
            chestPainPanel
        );

        yield return new WaitForSeconds(
            chestButtonToCountdownDelay
        );

        chestHelpPressed = false;
        inputLocked = false;

        StartChestCountdown();

        Debug.Log(
            "Göğüs ağrısı görevi yeniden başladı."
        );
    }

    /// <summary>
    /// Göğüs tekrar popupındaki tekrar dene/tamam butonuna atanır.
    /// </summary>
    public void ConfirmChestRetry()
    {
        if (
            currentStep !=
            Stage6Step.GogusAgrisi
        )
        {
            return;
        }

        chestRetryConfirmed = true;
    }

    private void StopChestCountdown()
    {
        if (chestCountdownCoroutine != null)
        {
            StopCoroutine(
                chestCountdownCoroutine
            );

            chestCountdownCoroutine = null;
        }

        HideChestCountdownObjects();
    }

    private void HideChestCountdownObjects()
    {
        SetActiveSafe(
            chestCountdown3,
            false
        );

        SetActiveSafe(
            chestCountdown2,
            false
        );

        SetActiveSafe(
            chestCountdown1,
            false
        );
    }

    // --------------------------------------------------
    // BAŞ DÖNMESİ
    // --------------------------------------------------

    private void StartDizzinessStep()
    {
        mainSequence = StartCoroutine(
            StartDizzinessStepSequence()
        );
    }

    private IEnumerator StartDizzinessStepSequence()
    {
        currentStep =
            Stage6Step.BasDonmesi;

        inputLocked = true;
        balanceSuccessConfirmed = false;
        balanceRetryConfirmed = false;
        balanceMoveInProgress = false;

        StopBalanceMovement();
        HideAllGameplayObjects(true);
        ResetBalanceMarker();

        // Önce kısa süre normal sürüş görünür.
        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        SetActiveSafe(
            normalChild,
            false
        );

        // Baş dönmesi yaşayan Çocuk görünür.
        SetActiveSafe(
            dizzinessChild,
            true
        );

        yield return new WaitForSeconds(
            childToBubbleDelay
        );

        // Başım dönüyor konuşma balonu.
        SetActiveSafe(
            dizzinessSpeechBubble,
            true
        );

        yield return new WaitForSeconds(
            bubbleToChoiceDelay
        );

        // Denge paneli ve yön butonları açılır.
        SetActiveSafe(
            balancePanel,
            true
        );

        inputLocked = false;

        Debug.Log(
            "Baş dönmesi ve denge bölümü başladı."
        );
    }

    private void HandleBalanceChoice(
        Stage6ChoiceButton.ChoiceType choice
    )
    {
        if (balanceMoveInProgress)
        {
            return;
        }

        float direction = 0f;

        if (
            choice ==
            Stage6ChoiceButton.ChoiceType.SolaDon
        )
        {
            direction = -1f;
        }
        else if (
            choice ==
            Stage6ChoiceButton.ChoiceType.SagaDon
        )
        {
            direction = 1f;
        }
        else
        {
            return;
        }

        if (balanceMarker == null)
        {
            Debug.LogError(
                "Stage6Manager: Balance Marker alanına hareket edecek denge ayracı atanmadı."
            );

            return;
        }

        float targetX = Mathf.Clamp(
            balanceMarker.anchoredPosition.x +
            direction * balanceMoveAmount,
            balanceMinX,
            balanceMaxX
        );

        balanceMoveCoroutine = StartCoroutine(
            MoveBalanceMarker(targetX)
        );
    }

    private IEnumerator MoveBalanceMarker(
        float targetX
    )
    {
        balanceMoveInProgress = true;
        inputLocked = true;

        Vector2 startPosition =
            balanceMarker.anchoredPosition;

        Vector2 targetPosition =
            new Vector2(
                targetX,
                startPosition.y
            );

        float elapsedTime = 0f;

        while (
            elapsedTime <
            balanceMoveDuration
        )
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime /
                balanceMoveDuration
            );

            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            balanceMarker.anchoredPosition =
                Vector2.Lerp(
                    startPosition,
                    targetPosition,
                    smoothProgress
                );

            yield return null;
        }

        balanceMarker.anchoredPosition =
            targetPosition;

        balanceMoveInProgress = false;
        balanceMoveCoroutine = null;

        float currentX =
            balanceMarker.anchoredPosition.x;

        // Ayracın orta güvenli alana gelmesi başarıdır.
        if (
            Mathf.Abs(currentX) <=
            balanceSuccessHalfWidth
        )
        {
            mainSequence = StartCoroutine(
                CompleteBalanceStep()
            );

            yield break;
        }

        // Ayracın kırmızı uçlardan birine ulaşması başarısızlıktır.
        bool reachedDangerEdge =
            currentX <= balanceMinX + 0.01f ||
            currentX >= balanceMaxX - 0.01f;

        if (reachedDangerEdge)
        {
            mainSequence = StartCoroutine(
                ShowBalanceRetryPopup()
            );

            yield break;
        }

        inputLocked = false;
    }

    private IEnumerator CompleteBalanceStep()
    {
        inputLocked = true;
        balanceSuccessConfirmed = false;

        StopBalanceMovement();

        SetActiveSafe(
            balancePanel,
            false
        );

        SetActiveSafe(
            dizzinessSpeechBubble,
            false
        );

        // Başarılı olduğunda Çocuk normal bisikletli hâline döner.
        SetActiveSafe(
            dizzinessChild,
            false
        );

        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        ShowObject(
            balanceCorrectPopup
        );

        PrepareCanvasGroup(
            balanceCorrectPopupCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                balanceCorrectPopupCanvasGroup,
                0f,
                1f,
                popupFadeDuration
            )
        );

        EnableCanvasInteraction(
            balanceCorrectPopupCanvasGroup
        );

        // Başarı popupı butona basılmadan kapanmaz.
        yield return new WaitUntil(
            () => balanceSuccessConfirmed
        );

        DisableCanvasInteraction(
            balanceCorrectPopupCanvasGroup
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                balanceCorrectPopupCanvasGroup,
                1f,
                0f,
                popupFadeDuration
            )
        );

        SetActiveSafe(
            balanceCorrectPopup,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        yield return new WaitForSeconds(
            betweenStepsDelay
        );

        StartFinalChoiceStep();
    }

    public void ConfirmBalanceSuccess()
    {
        if (
            currentStep !=
            Stage6Step.BasDonmesi
        )
        {
            return;
        }

        balanceSuccessConfirmed = true;
    }

    private IEnumerator ShowBalanceRetryPopup()
    {
        inputLocked = true;
        balanceRetryConfirmed = false;

        StopBalanceMovement();

        // Başarısız durumda başı dönen Çocuk ekranda kalır.
        SetActiveSafe(
            dizzinessChild,
            true
        );

        SetActiveSafe(
            normalChild,
            false
        );

        SetActiveSafe(
            balancePanel,
            false
        );

        SetActiveSafe(
            dizzinessSpeechBubble,
            false
        );

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        ShowObject(
            balanceRetryPopup
        );

        PrepareCanvasGroup(
            balanceRetryPopupCanvasGroup,
            0f,
            false,
            true
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                balanceRetryPopupCanvasGroup,
                0f,
                1f,
                popupFadeDuration
            )
        );

        EnableCanvasInteraction(
            balanceRetryPopupCanvasGroup
        );

        // Tekrar dene butonuna basılana kadar popup ve Çocuk açık kalır.
        while (!balanceRetryConfirmed)
        {
            SetActiveSafe(
                dizzinessChild,
                true
            );

            SetActiveSafe(
                normalChild,
                false
            );

            yield return null;
        }

        DisableCanvasInteraction(
            balanceRetryPopupCanvasGroup
        );

        yield return StartCoroutine(
            FadeCanvasGroup(
                balanceRetryPopupCanvasGroup,
                1f,
                0f,
                popupFadeDuration
            )
        );

        SetActiveSafe(
            balanceRetryPopup,
            false
        );

        // Yeniden denemede Çocuk baş dönmesi hâlinde kalır.
        SetActiveSafe(
            dizzinessChild,
            true
        );

        SetActiveSafe(
            normalChild,
            false
        );

        ResetBalanceMarker();

        yield return new WaitForSeconds(
            afterAlertDelay
        );

        SetActiveSafe(
            dizzinessSpeechBubble,
            true
        );

        yield return new WaitForSeconds(
            bubbleToChoiceDelay
        );

        SetActiveSafe(
            balancePanel,
            true
        );

        inputLocked = false;

        Debug.Log(
            "Denge görevi yeniden başladı."
        );
    }

    public void ConfirmBalanceRetry()
    {
        if (
            currentStep !=
            Stage6Step.BasDonmesi
        )
        {
            return;
        }

        balanceRetryConfirmed = true;
    }

    private void ResetBalanceMarker()
    {
        if (balanceMarker == null)
        {
            return;
        }

        Vector2 position =
            balanceMarker.anchoredPosition;

        position.x = Mathf.Clamp(
            balanceStartX,
            balanceMinX,
            balanceMaxX
        );

        balanceMarker.anchoredPosition =
            position;
    }

    private void StopBalanceMovement()
    {
        if (balanceMoveCoroutine != null)
        {
            StopCoroutine(
                balanceMoveCoroutine
            );

            balanceMoveCoroutine = null;
        }

        balanceMoveInProgress = false;
    }

    private void StartFinalChoiceStep()
    {
        mainSequence = StartCoroutine(
            StartFinalChoiceStepSequence()
        );
    }

    private IEnumerator StartFinalChoiceStepSequence()
    {
        currentStep =
            Stage6Step.SonSecim;

        inputLocked = true;

        HideAllGameplayObjects(true);
        ResetFinalChoiceButtons();

        SetActiveSafe(
            familyCharacters,
            false
        );

        SetActiveSafe(
            standingChild,
            false
        );

        SetActiveSafe(
            finalCongratulationsPanel,
            false
        );

        SetActiveSafe(
            finalWarningPanel,
            false
        );

        SetActiveSafe(
            finalSuccessPanel,
            false
        );

        SetActiveSafe(
            normalChild,
            true
        );

        yield return new WaitForSeconds(
            normalRideDuration
        );

        SetActiveSafe(
            finalChoicePanel,
            true
        );

        inputLocked = false;

        Debug.Log(
            "Son seçim bölümü başladı."
        );
    }

    // --------------------------------------------------
    // SON SEÇİM
    // --------------------------------------------------

    private void HandleFinalChoice(
        Stage6ChoiceButton.ChoiceType choice,
        Stage6ChoiceButton clickedButton
    )
    {
        if (
            choice ==
            Stage6ChoiceButton.ChoiceType.Aile
        )
        {
            // CanvasGroup alpha azaltmak yerine butonun gerçek rengi değiştirilir.
            // Böylece ana panelin içinde basılı duran eski buton görseli ortaya çıkmaz.
            SetFamilyChoiceWrongVisual();

            // Küçük uyarı paneli açılır.
            // Sağlık Noktası seçeneği kullanılmaya devam edebilir.
            SetActiveSafe(
                finalWarningPanel,
                true
            );

            Debug.Log(
                "Önce sağlık noktasına gitmelisin."
            );

            return;
        }

        if (
            choice ==
            Stage6ChoiceButton.ChoiceType
                .SaglikNoktasi
        )
        {
            inputLocked = true;

            SetActiveSafe(
                finalWarningPanel,
                false
            );

            mainSequence = StartCoroutine(
                CompleteFinalChoiceSequence()
            );
        }
    }

    private IEnumerator CompleteFinalChoiceSequence()
    {
        currentStep =
            Stage6Step.Tamamlandi;

        inputLocked = true;

        SetActiveSafe(
            finalChoicePanel,
            false
        );

        SetActiveSafe(
            finalCongratulationsPanel,
            false
        );

        SetActiveSafe(
            finalWarningPanel,
            false
        );

        SetActiveSafe(
            finalSuccessPanel,
            false
        );

        SetActiveSafe(
            standingChild,
            false
        );

        // Aile, Çocuk hedefe ulaşana kadar gizli kalır.
        SetActiveSafe(
            familyCharacters,
            false
        );

        // Final hareketi başladığında bisiklet salınımı durdurulur.
        if (normalChildIdleMotion != null)
        {
            normalChildIdleMotion.enabled = false;
        }

        SetActiveSafe(
            normalChild,
            true
        );

        if (
            normalChildRect == null ||
            bikeArrivalPoint == null
        )
        {
            Debug.LogError(
                "Stage6Manager: Normal Child Rect veya Bike Arrival Point atanmamış."
            );
        }

        if (
            standingChild == null ||
            standingChildRect == null
        )
        {
            Debug.LogError(
                "Stage6Manager: Standing Child veya Standing Child Rect atanmamış."
            );
        }

        if (
            familyRectTransform == null ||
            familyTargetPoint == null
        )
        {
            Debug.LogError(
                "Stage6Manager: Family Rect Transform veya Family Target Point atanmamış."
            );
        }

        Vector3 childStartWorldPosition =
            normalChildRect != null
                ? normalChildRect.position
                : Vector3.zero;

        Vector2 childStartSize =
            normalChildRect != null
                ? normalChildRect.sizeDelta
                : Vector2.zero;

        Vector3 childEndWorldPosition =
            bikeArrivalPoint != null
                ? bikeArrivalPoint.position
                : childStartWorldPosition;

        Vector2 childEndSize =
            bikeArrivalPoint != null
                ? bikeArrivalPoint.sizeDelta
                : childStartSize;

        Vector3 familyEndWorldPosition =
            familyTargetPoint != null
                ? familyTargetPoint.position
                : Vector3.zero;

        Vector3 familyStartWorldPosition =
            familyEndWorldPosition;

        if (familyRectTransform != null)
        {
            RectTransform familyParent =
                familyRectTransform.parent as RectTransform;

            if (familyParent != null)
            {
                Vector3 localTarget =
                    familyParent.InverseTransformPoint(
                        familyEndWorldPosition
                    );

                localTarget +=
                    new Vector3(
                        familyStartOffset.x,
                        familyStartOffset.y,
                        0f
                    );

                familyStartWorldPosition =
                    familyParent.TransformPoint(
                        localTarget
                    );
            }
            else
            {
                familyStartWorldPosition +=
                    new Vector3(
                        familyStartOffset.x,
                        familyStartOffset.y,
                        0f
                    );
            }

            familyRectTransform.position =
                familyStartWorldPosition;
        }

        // --------------------------------------------------
        // 1. ÖNCE BİSİKLETLİ ÇOCUK SAĞLIK NOKTASINA GİDER
        // --------------------------------------------------

        float childElapsedTime = 0f;

        while (
            childElapsedTime <
            childMoveDuration
        )
        {
            childElapsedTime +=
                Time.deltaTime;

            float childProgress =
                childMoveDuration <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        childElapsedTime /
                        childMoveDuration
                    );

            float smoothChildProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    childProgress
                );

            if (normalChildRect != null)
            {
                normalChildRect.position =
                    Vector3.Lerp(
                        childStartWorldPosition,
                        childEndWorldPosition,
                        smoothChildProgress
                    );

                normalChildRect.sizeDelta =
                    Vector2.Lerp(
                        childStartSize,
                        childEndSize,
                        smoothChildProgress
                    );
            }

            yield return null;
        }

        if (normalChildRect != null)
        {
            normalChildRect.position =
                childEndWorldPosition;

            normalChildRect.sizeDelta =
                childEndSize;
        }

        // Bisikletli Çocuk hedefe ulaşınca ayakta Çocuk görünür.
        SetActiveSafe(
            normalChild,
            false
        );

        SetActiveSafe(
            standingChild,
            true
        );

        // Çocuğun dönüşümünün anlaşılması için kısa bekleme.
        yield return new WaitForSeconds(
            familyMoveStartDelay
        );

        // --------------------------------------------------
        // 2. SONRA AİLE EKRANIN SAĞINDAN ÇOCUĞUN YANINA GELİR
        // --------------------------------------------------

        SetActiveSafe(
            familyCharacters,
            true
        );

        float familyElapsedTime = 0f;

        while (
            familyElapsedTime <
            familyMoveDuration
        )
        {
            familyElapsedTime +=
                Time.deltaTime;

            float familyProgress =
                familyMoveDuration <= 0f
                    ? 1f
                    : Mathf.Clamp01(
                        familyElapsedTime /
                        familyMoveDuration
                    );

            float smoothFamilyProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    familyProgress
                );

            if (familyRectTransform != null)
            {
                familyRectTransform.position =
                    Vector3.Lerp(
                        familyStartWorldPosition,
                        familyEndWorldPosition,
                        smoothFamilyProgress
                    );
            }

            yield return null;
        }

        if (familyRectTransform != null)
        {
            familyRectTransform.position =
                familyEndWorldPosition;
        }

        yield return new WaitForSeconds(
            delayBeforeFinalPopup
        );

        if (finalSuccessPanel != null)
        {
            SetActiveSafe(
                finalSuccessPanel,
                true
            );
        }
        else
        {
            // Eski Inspector atamalarının bozulmaması için yedek kullanım.
            SetActiveSafe(
                finalCongratulationsPanel,
                true
            );
        }

        UpdateTopPanelsForCompleted();

        finalSuccessConfirmed = false;

        Debug.Log(
            "6. aşama tamamlandı. Final başarı panelindeki buton bekleniyor."
        );

        // Son başarı panelindeki Tamam butonuna basılmasını bekler.
        yield return new WaitUntil(
            () => finalSuccessConfirmed
        );

        CompleteStageAndReturnToMap();
    }

    /// <summary>
    /// sonsecimaferin panelindeki Tamam butonunun
    /// OnClick olayöna atanmalıdır.
    /// </summary>
    public void ConfirmFinalSuccess()
    {
        if (
            currentStep != Stage6Step.Tamamlandi ||
            isReturningToMap
        )
        {
            return;
        }

        finalSuccessConfirmed = true;
    }

    private void CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            return;
        }

        isReturningToMap = true;
        inputLocked = true;

        StageProgress.CompleteStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    isReturningToMap = false;
                    inputLocked = false;

                    Debug.LogError(
                        "Stage6Manager: Aşama 6 ilerlemesi sunucuya kaydedilemedi."
                    );

                    return;
                }

                StartCoroutine(
                    ReturnToMapRoutine()
                );
            }
        );
    }

    private IEnumerator ReturnToMapRoutine()
    {
        yield return new WaitForSecondsRealtime(
            mapReturnDelay
        );

        if (
            string.IsNullOrWhiteSpace(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                "Stage6Manager: Map Scene Name alanı boş."
            );

            isReturningToMap = false;
            yield break;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                $"Stage6Manager: '{mapSceneName}' sahnesi " +
                "Build Profiles içindeki Scene List'te bulunamadı."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    private void SetFamilyChoiceWrongVisual()
    {
        if (familyChoiceImage != null)
        {
            Color tintedColor =
                familyWrongTint;

            // Alfa tam opak kalır. Böylece arkadaki gömülü buton görünümü
            // yeniden ortaya çıkmaz.
            tintedColor.a = 1f;
            familyChoiceImage.color =
                tintedColor;
        }

        if (
            familyChoiceButton != null &&
            familyChoiceButton.button != null
        )
        {
            familyChoiceButton.button.interactable =
                false;
        }
    }

    private void ResetFinalChoiceButtons()
    {
        if (familyChoiceButton != null)
        {
            familyChoiceButton.ResetVisual();
        }

        if (healthPointChoiceButton != null)
        {
            healthPointChoiceButton.ResetVisual();
        }

        if (familyChoiceImage != null)
        {
            familyChoiceImage.color =
                familyChoiceOriginalColor;
        }

        SetActiveSafe(
            finalWarningPanel,
            false
        );

        SetActiveSafe(
            finalSuccessPanel,
            false
        );
    }

    // --------------------------------------------------
    // ÜST PANEL METİNLERİ
    // --------------------------------------------------

    private void UpdateTopPanelsForIntro()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForBreathWaiting()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForBreathProblem()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForBreathChoice()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForBreathSuccess()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForNormalRide()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForChestPainWaiting()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForChestPainProblem()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForChestPainChoice()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForChestPainSuccess()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForChestPainRetry()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForDizziness()
    {
        SetStableTopPanelTexts();
    }

    private void UpdateTopPanelsForCompleted()
    {
        SetStableTopPanelTexts();
    }

    private void SetStableTopPanelTexts()
    {
        SetTopPanelTexts(
            "Vücudundaki belirtileri fark et ve gerektiçinde yardım iste.",
            "Görev",
            "Doğru davranışı seç"
        );
    }

    private void SetTopPanelTexts(
        string leftMessage,
        string taskTitle,
        string taskDescription
    )
    {
        SetTextSafe(
            topLeftMessageText,
            leftMessage
        );

        SetTextSafe(
            topRightTitleText,
            taskTitle
        );

        SetTextSafe(
            topRightDescriptionText,
            taskDescription
        );
    }

    // --------------------------------------------------
    // YARDIMCI METOTLAR
    // --------------------------------------------------

    private void HideAllGameplayObjects(
        bool keepNormalChild = false
    )
    {
        if (!keepNormalChild)
        {
            SetActiveSafe(
                normalChild,
                false
            );
        }

        // Nefes
        SetActiveSafe(
            breathChild,
            false
        );

        SetActiveSafe(
            breathSpeechBubble,
            false
        );

        SetActiveSafe(
            breathChoicePanel,
            false
        );

        SetActiveSafe(
            breathCorrectPopup,
            false
        );

        // Göğüs ağrısı
        SetActiveSafe(
            chestPainChild,
            false
        );

        SetActiveSafe(
            chestPainSpeechBubble,
            false
        );

        SetActiveSafe(
            chestPainPanel,
            false
        );

        HideChestCountdownObjects();

        SetActiveSafe(
            chestCorrectPopup,
            false
        );

        SetActiveSafe(
            chestRetryPopup,
            false
        );

        // Baş dönmesi
        SetActiveSafe(
            dizzinessChild,
            false
        );

        SetActiveSafe(
            dizzinessSpeechBubble,
            false
        );

        SetActiveSafe(
            balancePanel,
            false
        );

        SetActiveSafe(
            balanceCorrectPopup,
            false
        );

        SetActiveSafe(
            balanceRetryPopup,
            false
        );

        // Son seçim
        SetActiveSafe(
            finalChoicePanel,
            false
        );

        SetActiveSafe(
            standingChild,
            false
        );

        SetActiveSafe(
            familyCharacters,
            false
        );

        SetActiveSafe(
            finalCongratulationsPanel,
            false
        );

        SetActiveSafe(
            finalWarningPanel,
            false
        );

        SetActiveSafe(
            finalSuccessPanel,
            false
        );
    }

    private IEnumerator FadeCanvasGroup(
        CanvasGroup canvasGroup,
        float startAlpha,
        float targetAlpha,
        float duration
    )
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        if (duration <= 0f)
        {
            canvasGroup.alpha =
                targetAlpha;

            yield break;
        }

        canvasGroup.alpha =
            startAlpha;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime +=
                Time.deltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsedTime / duration
                );

            float smoothProgress =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    progress
                );

            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    smoothProgress
                );

            yield return null;
        }

        canvasGroup.alpha =
            targetAlpha;
    }

    private void PrepareCanvasGroup(
        CanvasGroup canvasGroup,
        float alpha,
        bool interactable,
        bool blocksRaycasts
    )
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha =
            alpha;

        canvasGroup.interactable =
            interactable;

        canvasGroup.blocksRaycasts =
            blocksRaycasts;
    }

    private void ShowObject(
        GameObject target
    )
    {
        if (target == null)
        {
            return;
        }

        target.SetActive(true);

        CanvasGroup group =
            target.GetComponent<CanvasGroup>();

        if (group != null)
        {
            group.alpha = 1f;
        }
    }

    private void EnableCanvasInteraction(
        CanvasGroup canvasGroup
    )
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void DisableCanvasInteraction(
        CanvasGroup canvasGroup
    )
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
    }

    private void SetActiveSafe(
        GameObject target,
        bool active
    )
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    private void SetTextSafe(
        TMP_Text target,
        string value
    )
    {
        if (target != null)
        {
            target.text = value;
        }
    }
}