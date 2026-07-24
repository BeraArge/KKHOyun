using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Eðitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileþeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanýlacak adým kimliði.")]
    [SerializeField]
    private string educationStepId = "asama7_egitim1";

    [Header("Eðitim Týklama Engelleyici")]
    [Tooltip("Eðitim sýrasýnda Task1 içindeki meyvelerin önünde duracak görünmez blocker objesi.")]
    [SerializeField]
    private GameObject educationInputBlocker;

    [Header("Görev Ayarlarý")]
    [SerializeField]
    private int totalTasks = 5;

    [Header("Aþama Geçiþi")]
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

    // Ayný görevin birden fazla kez sayýlmasýný engeller.
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
        completedTasks = 0;
        educationIsOpen = true;
        gameStarted = false;
        gameCompleted = false;
        isReturningToMap = false;

        completedTaskIds.Clear();

        // Eðitim açýkken blocker aktif olur.
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
                "[GameManager] Education Panel atanmadý. " +
                "Aþama 7 eðitimi atlanýyor."
            );
        }

        educationIsOpen = false;
        gameStarted = true;

        // Eðitim kapandýktan sonra blocker kapanýr.
        SetEducationBlockerActive(false);

        Debug.Log(
            "[GameManager] Aþama 7 eðitimi tamamlandý. Oyun baþladý."
        );
    }

    private void HandleTaskCompletion(
        int taskId
    )
    {
        // Eðitim kapanmadan hiçbir görev tamamlanmýþ sayýlmaz.
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
                $"Görev {taskId} daha önce tamamlandý."
            );

            return;
        }

        completedTaskIds.Add(taskId);
        completedTasks++;

        Debug.Log(
            $"Harika! Görev {taskId} bitti. " +
            $"Ýlerleme durumu: {completedTasks}/{totalTasks}."
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
            "Tebrikler, tüm görevleri tamamladýn!"
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
                "[GameManager] Map Scene Name alaný boþ."
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
                "Build Profiles içindeki Scene List'te bulunamadý."
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
                "[GameManager] Education Input Blocker atanmadý."
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

        // Yalnýzca test/debug amacýyla kullanýlabilir.
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