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

    [Header("Ba�lang�� Durumu")]
    [SerializeField]
    private Stage6Step currentStep =
        Stage6Step.GirisBilgilendirmesi;

    [Header("Genel S�reler")]
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
    // E��T�M PANEL�
    // --------------------------------------------------

    [Header("E�itim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bile�eni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI i�indeki Steps listesinde kullan�lacak ad�m kimli�i.")]
    [SerializeField]
    private string educationStepId = "asama6_egitim1";

    // --------------------------------------------------
    // BA�LANGI� B�LG�LEND�RME PANEL�
    // --------------------------------------------------

    [Header("Ba�lang�� Bilgilendirme Paneli")]
    public GameObject introPanel;

    public CanvasGroup introCanvasGroup;

    [SerializeField]
    private float introFadeDuration = 0.8f;

    [Header("Bilgilendirme Paneli A��klamas�")]
    public TMP_Text introDescriptionText;

    [TextArea(3, 6)]
    public string introDescription =
        "Acil durum belirtilerini tan�!\n" +
        "Kendini k�t� hissetti�inde do�ru se�imi " +
        "yaparak yard�m istemelisin.";

    // --------------------------------------------------
    // �ST PANELLER
    // --------------------------------------------------

    [Header("Sol �st Bilgilendirme Paneli")]
    public TMP_Text topLeftMessageText;

    [Header("Sa� �st G�rev Paneli")]
    public TMP_Text topRightTitleText;
    public TMP_Text topRightDescriptionText;

    // --------------------------------------------------
    // NORMAL �OCUK
    // --------------------------------------------------

    [Header("Normal �ocuk")]
    public GameObject normalChild;

    // --------------------------------------------------
    // NEFES DARLI�I
    // --------------------------------------------------

    [Header("Nefes Darl���")]
    public GameObject breathChild;
    public GameObject breathSpeechBubble;
    public GameObject breathChoicePanel;

    [Header("Nefes Se�im Butonlar�")]
    public Stage6ChoiceButton breathContinueButton;
    public Stage6ChoiceButton breathHideButton;
    public Stage6ChoiceButton breathHelpButton;

    [Header("Nefes Darl��� Ba�ar� Popup�")]
    public GameObject breathCorrectPopup;
    public CanvasGroup breathCorrectPopupCanvasGroup;

    // --------------------------------------------------
    // G���S A�RISI
    // --------------------------------------------------

    [Header("G���s A�r�s�")]
    public GameObject chestPainChild;
    public GameObject chestPainSpeechBubble;
    public GameObject chestPainPanel;

    [Header("G���s A�r�s� Saya� G�rselleri")]
    public GameObject chestCountdown3;
    public GameObject chestCountdown2;
    public GameObject chestCountdown1;

    [SerializeField]
    private float chestCountdownStepDuration = 1.2f;

    [Header("G���s A�r�s� Ba�ar� Popup�")]
    public GameObject chestCorrectPopup;
    public CanvasGroup chestCorrectPopupCanvasGroup;

    [Header("G���s A�r�s� Tekrar Popup�")]
    public GameObject chestRetryPopup;
    public CanvasGroup chestRetryPopupCanvasGroup;

    [SerializeField]
    private float chestRetryPopupDuration = 3.5f;

    [SerializeField]
    private float chestChildToBubbleDelay = 1.2f;

    [SerializeField]
    private float chestBubbleToButtonDelay = 1.8f;

    [SerializeField]
    private float chestButtonToCountdownDelay = 0.6f;

    // --------------------------------------------------
    // BA� D�NMES�
    // --------------------------------------------------

    [Header("Ba� D�nmesi")]
    public GameObject dizzinessChild;
    public GameObject dizzinessSpeechBubble;
    public GameObject balancePanel;

    [Header("Denge Mekani�i")]
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

    [Header("Denge Ba�ar� Popup�")]
    public GameObject balanceCorrectPopup;
    public CanvasGroup balanceCorrectPopupCanvasGroup;

    [Header("Denge Tekrar Popup�")]
    public GameObject balanceRetryPopup;
    public CanvasGroup balanceRetryPopupCanvasGroup;

    // --------------------------------------------------
    // SON SE��M
    // --------------------------------------------------

    [Header("Son Se�im")]
    public GameObject finalChoicePanel;

    [Header("Son Se�im Butonlar�")]
    public Stage6ChoiceButton familyChoiceButton;
    public Stage6ChoiceButton healthPointChoiceButton;

    [Header("Son Se�im Geri Bildirim Panelleri")]
    [Tooltip("Ailem se�ildi�inde a��lacak k���k uyar� paneli.")]
    public GameObject finalWarningPanel;

    [Tooltip("�ocuk ve ailesi bulu�tuktan sonra a��lacak ba�ar� paneli.")]
    public GameObject finalSuccessPanel;

    [Header("Ailem Butonu Yanl�� Se�im G�r�n�m�")]
    [Tooltip("Ailem butonunun ana Image bile�eni. Alfa azalt�lmaz; renk do�rudan de�i�tirilir.")]
    public Image familyChoiceImage;

    [SerializeField]
    private Color familyWrongTint =
        new Color(0.55f, 0.55f, 0.55f, 1f);

    private Color familyChoiceOriginalColor =
        Color.white;

    [Header("Final Hareket Hedefleri")]
    public RectTransform normalChildRect;

    [Tooltip("Sa�l�k merkezi �n�nde duran ayakta �ocuk objesi.")]
    public GameObject standingChild;

    [Tooltip("cocukson objesinin RectTransform bile�eni.")]
    public RectTransform standingChildRect;

    [Tooltip("Bisikletli �ocu�un sa�l�k merkezine giderken ula�aca�� g�r�nmez hedef.")]
    public RectTransform bikeArrivalPoint;

    public RectTransform familyRectTransform;
    public RectTransform familyTargetPoint;

    [Tooltip("cocuknormal �zerindeki BicycleIdleMotion scriptini buraya atay�n.")]
    public MonoBehaviour normalChildIdleMotion;

    [SerializeField]
    private Vector2 familyStartOffset =
        new Vector2(500f, 0f);

    [SerializeField]
    private float childMoveDuration = 2.8f;

    [SerializeField]
    private float familyMoveDuration = 2.3f;

    [Tooltip("�ocuk ayakta h�line ge�tikten sonra ailenin harekete ba�lamadan �nce bekleyece�i s�re.")]
    [SerializeField]
    private float familyMoveStartDelay = 0.8f;

    [SerializeField]
    private float delayBeforeFinalPopup = 1.2f;

    [Header("Final")]
    public GameObject finalCongratulationsPanel;
    public GameObject familyCharacters;

    [Header("A�ama Ge�i�i")]
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

        // E�itim s�ras�nda yaln�zca normal bisikletli �ocuk g�r�n�r.
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
                "Stage6Manager: Education Panel atanmad�. " +
                "A�ama 6 e�itimi atlan�yor."
            );
        }

        // E�itim kapan�nca mevcut A�ama 6 ba�lang�� ak���na devam edilir.
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
    // BA�LANGI� B�LG�LEND�RMES�
    // --------------------------------------------------

    private IEnumerator StartStage6Intro()
    {
        currentStep =
            Stage6Step.GirisBilgilendirmesi;

        inputLocked = true;
        introConfirmed = false;

        HideAllGameplayObjects();

        // Bilgilendirme s�ras�nda normal �ocuk g�r�n�r.
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

        // Tamam butonuna bas�lmas�n� bekler.
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
    /// WarningPopup i�indeki Tamam butonunun
    /// OnClick olay�na atanmal�d�r.
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
    // SE��M Y�NET�M�
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
    // NEFES DARLI�I
    // --------------------------------------------------

    private IEnumerator StartBreathStepSequence()
    {
        currentStep =
            Stage6Step.NefesDarligi;

        inputLocked = true;

        HideAllGameplayObjects();
        ResetBreathButtons();

        UpdateTopPanelsForBreathWaiting();

        // �nce normal s�r��.
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

        // Nefes darl��� karakteri.
        SetActiveSafe(
            breathChild,
            true
        );

        UpdateTopPanelsForBreathProblem();

        yield return new WaitForSeconds(
            childToBubbleDelay
        );

        // Konu�ma balonu. CanvasGroup varsa g�r�n�rl��� de s�f�rlan�r.
        ShowObject(
            breathSpeechBubble
        );

        yield return new WaitForSeconds(
            bubbleToChoiceDelay
        );

        // Se�im paneli.
        ShowObject(
            breathChoicePanel
        );

        UpdateTopPanelsForBreathChoice();

        inputLocked = false;

        Debug.Log(
            "Nefes darl��� se�imleri a��ld�."
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

        // Do�ru se�imde �ocuk normal bisikletli h�line d�ner.
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

        // Popup, i�indeki butona bas�lmadan kapanmaz.
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
    /// Nefes ba�ar� popup�ndaki devam/tamam butonuna atan�r.
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
    // G���S A�RISI
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

        // �nce normal �ocuk k�sa s�re g�r�n�r.
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

        // G���s a�r�s� karakteri g�r�n�r.
        SetActiveSafe(
            chestPainChild,
            true
        );

        UpdateTopPanelsForChestPainProblem();

        yield return new WaitForSeconds(
            chestChildToBubbleDelay
        );

        // G��s�m a�r�yor konu�ma balonu.
        SetActiveSafe(
            chestPainSpeechBubble,
            true
        );

        yield return new WaitForSeconds(
            chestBubbleToButtonDelay
        );

        // Yard�m �ste paneli.
        ShowObject(
            chestPainPanel
        );

        yield return new WaitForSeconds(
            chestButtonToCountdownDelay
        );

        inputLocked = false;

        StartChestCountdown();

        Debug.Log(
            "G���s a�r�s� b�l�m� ba�lad�."
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

        // S�re doldu.
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
            "Yard�m iste butonuna zaman�nda bas�ld�."
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

        // Ba�ar�da �ocuk normal bisikletli h�line d�ner.
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

        // Ba�ar� popup� butona bas�lmadan kapanmaz.
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
    /// G���s ba�ar� popup�ndaki devam/tamam butonuna atan�r.
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

        // Ba�ar�s�z durumda �ocuk g���s a�r�s� h�linde kal�r.
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

        // Popup a��ld���nda g���s a�r�s� ya�ayan �ocuk
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

        // Tekrar dene butonuna bas�lana kadar popup a��k kal�r.
        // Bu s�rada ba�ka bir i�lem karakteri kapatsa bile
        // g���s a�r�s� ya�ayan �ocuk tekrar aktif tutulur.
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

        // Popup kapand�ktan sonra da �ocuk ayn� durumda kal�r.
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

        // �nce konu�ma balonu yeniden g�r�n�r.
        ShowObject(
            chestPainSpeechBubble
        );

        yield return new WaitForSeconds(
            chestBubbleToButtonDelay
        );

        // Sonra yard�m butonu yeniden a��l�r.
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
            "G���s a�r�s� g�revi yeniden ba�lad�."
        );
    }

    /// <summary>
    /// G���s tekrar popup�ndaki tekrar dene/tamam butonuna atan�r.
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
    // BA� D�NMES�
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

        // �nce k�sa s�re normal s�r�� g�r�n�r.
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

        // Ba� d�nmesi ya�ayan �ocuk g�r�n�r.
        SetActiveSafe(
            dizzinessChild,
            true
        );

        yield return new WaitForSeconds(
            childToBubbleDelay
        );

        // Ba��m d�n�yor konu�ma balonu.
        SetActiveSafe(
            dizzinessSpeechBubble,
            true
        );

        yield return new WaitForSeconds(
            bubbleToChoiceDelay
        );

        // Denge paneli ve y�n butonlar� a��l�r.
        SetActiveSafe(
            balancePanel,
            true
        );

        inputLocked = false;

        Debug.Log(
            "Ba� d�nmesi ve denge b�l�m� ba�lad�."
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
                "Stage6Manager: Balance Marker alan�na hareket edecek denge ayrac� atanmad�."
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

        // Ayrac�n orta g�venli alana gelmesi ba�ar�d�r.
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

        // Ayrac�n k�rm�z� u�lardan birine ula�mas� ba�ar�s�zl�kt�r.
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

        // Ba�ar�l� oldu�unda �ocuk normal bisikletli h�line d�ner.
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

        // Ba�ar� popup� butona bas�lmadan kapanmaz.
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

        // Ba�ar�s�z durumda ba�� d�nen �ocuk ekranda kal�r.
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

        // Tekrar dene butonuna bas�lana kadar popup ve �ocuk a��k kal�r.
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

        // Yeniden denemede �ocuk ba� d�nmesi h�linde kal�r.
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
            "Denge g�revi yeniden ba�lad�."
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
            "Son se�im b�l�m� ba�lad�."
        );
    }

    // --------------------------------------------------
    // SON SE��M
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
            // CanvasGroup alpha azaltmak yerine butonun ger�ek rengi de�i�tirilir.
            // B�ylece ana panelin i�inde bas�l� duran eski buton g�rseli ortaya ��kmaz.
            SetFamilyChoiceWrongVisual();

            // K���k uyar� paneli a��l�r.
            // Sa�l�k Noktas� se�ene�i kullan�lmaya devam edebilir.
            SetActiveSafe(
                finalWarningPanel,
                true
            );

            Debug.Log(
                "�nce sa�l�k noktas�na gitmelisin."
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

        // Aile, �ocuk hedefe ula�ana kadar gizli kal�r.
        SetActiveSafe(
            familyCharacters,
            false
        );

        // Final hareketi ba�lad���nda bisiklet sal�n�m� durdurulur.
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
                "Stage6Manager: Normal Child Rect veya Bike Arrival Point atanmam��."
            );
        }

        if (
            standingChild == null ||
            standingChildRect == null
        )
        {
            Debug.LogError(
                "Stage6Manager: Standing Child veya Standing Child Rect atanmam��."
            );
        }

        if (
            familyRectTransform == null ||
            familyTargetPoint == null
        )
        {
            Debug.LogError(
                "Stage6Manager: Family Rect Transform veya Family Target Point atanmam��."
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
        // 1. �NCE B�S�KLETL� �OCUK SA�LIK NOKTASINA G�DER
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

        // Bisikletli �ocuk hedefe ula��nca ayakta �ocuk g�r�n�r.
        SetActiveSafe(
            normalChild,
            false
        );

        SetActiveSafe(
            standingChild,
            true
        );

        // �ocu�un d�n���m�n�n anla��lmas� i�in k�sa bekleme.
        yield return new WaitForSeconds(
            familyMoveStartDelay
        );

        // --------------------------------------------------
        // 2. SONRA A�LE EKRANIN SA�INDAN �OCU�UN YANINA GEL�R
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
            // Eski Inspector atamalar�n�n bozulmamas� i�in yedek kullan�m.
            SetActiveSafe(
                finalCongratulationsPanel,
                true
            );
        }

        UpdateTopPanelsForCompleted();

        finalSuccessConfirmed = false;

        Debug.Log(
            "6. a�ama tamamland�. Final ba�ar� panelindeki buton bekleniyor."
        );

        // Son ba�ar� panelindeki Tamam butonuna bas�lmas�n� bekler.
        yield return new WaitUntil(
            () => finalSuccessConfirmed
        );

        CompleteStageAndReturnToMap();
    }

    /// <summary>
    /// sonsecimaferin panelindeki Tamam butonunun
    /// OnClick olay�na atanmal�d�r.
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
            stageNumber
        );

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
            string.IsNullOrWhiteSpace(
                mapSceneName
            )
        )
        {
            Debug.LogError(
                "Stage6Manager: Map Scene Name alan� bo�."
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
                "Build Profiles i�indeki Scene List'te bulunamad�."
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

            // Alfa tam opak kal�r. B�ylece arkadaki g�m�l� buton g�r�n�m�
            // yeniden ortaya ��kmaz.
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
    // �ST PANEL MET�NLER�
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
            "V�cudundaki belirtileri fark et ve gerekti�inde yard�m iste.",
            "G�rev",
            "Do�ru davran��� se�"
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

        // G���s a�r�s�
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

        // Ba� d�nmesi
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

        // Son se�im
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