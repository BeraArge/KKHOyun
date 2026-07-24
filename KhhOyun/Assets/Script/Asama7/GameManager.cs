using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    private int completedTasks = 0;

    private bool gameCompleted = false;
    private bool isReturningToMap = false;

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

    private void HandleTaskCompletion(
        int taskId
    )
    {
        if (
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

    private void Update()
    {
        if (
            Keyboard.current == null ||
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