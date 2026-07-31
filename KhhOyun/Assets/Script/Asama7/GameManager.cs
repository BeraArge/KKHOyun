using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Eğitim")]
    [Tooltip("Sahnedeki EducationPanel objesinin EducationPanelUI bileşeni.")]
    public EducationPanelUI educationPanel;

    [Tooltip("EducationPanelUI içindeki Steps listesinde kullanılacak adım kimliği.")]
    [SerializeField]
    private string educationStepId = "asama7_egitim1";

    [Header("Eğitim Tıklama Engelleyici")]
    [Tooltip(
        "Eğitim sırasında Task1 içindeki meyvelerin önünde " +
        "duracak görünmez blocker objesi."
    )]
    [SerializeField]
    private GameObject educationInputBlocker;

    [Header("Görev Ayarları")]
    [SerializeField]
    private int totalTasks = 5;

    [Header("Aşama Geçişi")]
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

    // Aynı görevin birden fazla kez sayılmasını engeller.
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

        // Eğitim açıkken blocker aktif olur.
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
                "[GameManager] Education Panel atanmadı. " +
                "Aşama 7 eğitimi atlanıyor."
            );
        }

        educationIsOpen = false;
        gameStarted = true;

        // Eğitim kapandıktan sonra blocker kapanır.
        SetEducationBlockerActive(false);

        Debug.Log(
            "[GameManager] Aşama 7 eğitimi tamamlandı. Oyun başladı."
        );
    }

    private void HandleTaskCompletion(
        int taskId
    )
    {
        // Eğitim kapanmadan hiçbir görev tamamlanmış sayılmaz.
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
                $"Görev {taskId} daha önce tamamlandı."
            );

            return;
        }

        completedTaskIds.Add(taskId);
        completedTasks++;

        Debug.Log(
            $"Harika! Görev {taskId} bitti. " +
            $"İlerleme durumu: {completedTasks}/{totalTasks}."
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
            "Tebrikler, tüm görevleri tamamladın!"
        );

        GameEvents.OnGameWon?.Invoke();

        CompleteStageAndReturnToMap();
    }

    private void CompleteStageAndReturnToMap()
    {
        if (isReturningToMap)
        {
            return;
        }

        isReturningToMap = true;

        StageProgress.CompleteStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    isReturningToMap = false;
                    gameCompleted = false;

                    Debug.LogError(
                        "[GameManager] Aşama 7 ilerlemesi " +
                        "sunucuya kaydedilemedi."
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
                "[GameManager] Map Scene Name alanı boş."
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
                "Build Profiles içindeki Scene List'te bulunamadı."
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
                "[GameManager] Education Input Blocker atanmadı."
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

        // Yalnızca test/debug amacıyla kullanılabilir.
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