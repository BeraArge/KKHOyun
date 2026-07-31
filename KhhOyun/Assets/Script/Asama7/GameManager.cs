using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("E�itim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bile�eni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI i�indeki Steps listesinde kullan�lacak ad�m kimli�i.")]
    [SerializeField]
    private string educationStepId = "asama7_egitim1";

    [Header("E�itim T�klama Engelleyici")]
    [Tooltip("E�itim s�ras�nda Task1 i�indeki meyvelerin �n�nde duracak g�r�nmez blocker objesi.")]
    [SerializeField]
    private GameObject educationInputBlocker;

    [Header("G�rev Ayarlar�")]
    [SerializeField]
    private int totalTasks = 5;

    [Header("A�ama Ge�i�i")]
    [SerializeField]
    private int stageNumber = 7;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    private int completedTasks;

    private bool educationIsOpen;
    private bool gameStarted;
    private bool gameCompleted;
    private bool isReturningToMap;

    // Ayn� g�revin birden fazla kez say�lmas�n� engeller.
    private readonly HashSet<int> completedTaskIds =
        new HashSet<int>();

    private void OnEnable()
    {
        GameEvents.OnTaskCompleted +=
            HandleTaskCompletion;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskCompleted -=
            HandleTaskCompletion;
    }

    private void Start()
    {
        StageProgress.EnterStage(stageNumber);

        completedTasks = 0;
        educationIsOpen = true;
        gameStarted = false;
        gameCompleted = false;
        isReturningToMap = false;

        completedTaskIds.Clear();

        // E�itim a��kken blocker aktif olur.
        SetEducationBlockerActive(true);

        StartCoroutine(
            StartEducationRoutine()
        );
    }

    private IEnumerator StartEducationRoutine()
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
                "[GameManager] Education Panel atanmad�. " +
                "A�ama 7 e�itimi atlan�yor."
            );
        }

        educationIsOpen = false;
        gameStarted = true;

        // E�itim kapand�ktan sonra blocker kapan�r.
        SetEducationBlockerActive(false);

        Debug.Log(
            "[GameManager] A�ama 7 e�itimi tamamland�. Oyun ba�lad�."
        );
    }

    private void HandleTaskCompletion(
        int taskId
    )
    {
        // E�itim kapanmadan hi�bir g�rev tamamlanm�� say�lmaz.
        if (
            educationIsOpen ||
            !gameStarted ||
            gameCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        if (completedTaskIds.Contains(taskId))
        {
            Debug.Log(
                $"G�rev {taskId} daha �nce tamamland�."
            );

            return;
        }

        completedTaskIds.Add(taskId);
        completedTasks++;

        Debug.Log(
            $"Harika! G�rev {taskId} bitti. " +
            $"�lerleme durumu: {completedTasks}/{totalTasks}."
        );

        if (completedTasks >= totalTasks)
        {
            CompleteStage();
        }
    }

    private void CompleteStage()
    {
        if (
            !gameStarted ||
            gameCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        gameCompleted = true;

        Debug.Log(
            "Tebrikler, t�m g�revleri tamamlad�n!"
        );

        GameEvents.OnGameWon?.Invoke();

        StartCoroutine(
            CompleteStageAndReturnToMap()
        );
    }

    private IEnumerator CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            yield break;
        }

        isReturningToMap = true;

        StageProgress.CompleteStage(
            stageNumber
        );

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
                "[GameManager] Map Scene Name alan� bo�."
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
                $"[GameManager] '{mapSceneName}' sahnesi " +
                "Build Profiles i�indeki Scene List'te bulunamad�."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    private void SetEducationBlockerActive(
        bool active
    )
    {
        if (educationInputBlocker != null)
        {
            educationInputBlocker.SetActive(active);
        }
        else
        {
            Debug.LogWarning(
                "[GameManager] Education Input Blocker atanmad�."
            );
        }
    }

    private void Update()
    {
        if (
            Keyboard.current == null ||
            educationIsOpen ||
            !gameStarted ||
            gameCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        // Yaln�zca test/debug amac�yla kullan�labilir.
        if (
            Keyboard.current.digit1Key
                .wasPressedThisFrame
        )
        {
            GameEvents.OnTaskCompleted?.Invoke(1);
        }

        if (
            Keyboard.current.digit2Key
                .wasPressedThisFrame
        )
        {
            GameEvents.OnTaskCompleted?.Invoke(2);
        }

        if (
            Keyboard.current.digit3Key
                .wasPressedThisFrame
        )
        {
            GameEvents.OnTaskCompleted?.Invoke(3);
        }

        if (
            Keyboard.current.digit4Key
                .wasPressedThisFrame
        )
        {
            GameEvents.OnTaskCompleted?.Invoke(4);
        }

        if (
            Keyboard.current.digit5Key
                .wasPressedThisFrame
        )
        {
            GameEvents.OnTaskCompleted?.Invoke(5);
        }
    }
}