using System;
using System.Collections.Generic;
using UnityEngine;

public static class StageProgress
{
    public const int TotalStageCount = 7;

    private static readonly Dictionary<int, bool>
        StartedStages =
            new Dictionary<int, bool>();

    private static readonly Dictionary<int, bool>
        CompletedStages =
            new Dictionary<int, bool>();

    private static int completedStageCount;
    private static bool isGameCompleted;
    private static bool reenteredSentThisSession;

    public static int HighestCompletedStage =>
        Mathf.Clamp(
            completedStageCount,
            0,
            TotalStageCount
        );

    public static int CurrentStage
    {
        get
        {
            if (HighestCompletedStage >= TotalStageCount)
                return TotalStageCount;

            return HighestCompletedStage + 1;
        }
    }

    public static void Initialize(
        Action onReady,
        Action<string> onError = null
    )
    {
        if (GameApiClient.Instance == null)
        {
            onError?.Invoke(
                "Sunucu bağlantısı kurulamadı."
            );

            return;
        }

        GameApiClient.Instance.StartGame(
            startSuccess =>
            {
                if (!startSuccess)
                {
                    onError?.Invoke(
                        "Oyun başlatılamadı."
                    );

                    return;
                }

                GameApiClient.Instance.GetProgress(
                    (getSuccess, snapshot) =>
                    {
                        if (!getSuccess)
                        {
                            onError?.Invoke(
                                "İlerleme bilgisi alınamadı."
                            );

                            return;
                        }

                        ApplySnapshot(snapshot);

                        if (
                            isGameCompleted &&
                            !reenteredSentThisSession
                        )
                        {
                            reenteredSentThisSession =
                                true;

                            GameApiClient.Instance
                                .MarkReentered(
                                    success =>
                                    {
                                        if (!success)
                                        {
                                            Debug.LogWarning(
                                                "[StageProgress] Yeniden giriş kaydedilemedi."
                                            );
                                        }
                                    }
                                );
                        }

                        onReady?.Invoke();
                    }
                );
            }
        );
    }

    private static void ApplySnapshot(
        GameProgressSnapshot snapshot
    )
    {
        StartedStages.Clear();
        CompletedStages.Clear();

        isGameCompleted =
            snapshot.IsGameCompleted;

        completedStageCount =
            Mathf.Clamp(
                snapshot.CompletedStageCount,
                0,
                TotalStageCount
            );

        foreach (
            KeyValuePair<int, bool> pair
            in snapshot.StageStarted
        )
        {
            StartedStages[pair.Key] =
                pair.Value;
        }

        foreach (
            KeyValuePair<int, bool> pair
            in snapshot.StageCompleted
        )
        {
            CompletedStages[pair.Key] =
                pair.Value;
        }

        Debug.Log(
            $"[StageProgress] Sunucu ilerlemesi uygulandı. " +
            $"Tamamlanan aşama sayısı: {completedStageCount}"
        );
    }

    public static void EnterStage(int stageNumber)
    {
        if (GameApiClient.Instance == null)
        {
            Debug.LogError(
                "[StageProgress] GameApiClient bulunamadı."
            );

            return;
        }

        bool wasOpenedBefore =
            StartedStages.TryGetValue(
                stageNumber,
                out bool started
            ) && started;

        if (wasOpenedBefore)
        {
            GameApiClient.Instance.RetryStage(
                stageNumber,
                success =>
                {
                    if (!success)
                    {
                        Debug.LogError(
                            $"[StageProgress] {stageNumber}. aşama retry API başarısız."
                        );

                        return;
                    }

                    Debug.Log(
                        $"[StageProgress] {stageNumber}. aşama yeniden açıldı. " +
                        "Retry sayısı artırıldı."
                    );
                }
            );

            return;
        }

        GameApiClient.Instance.StartStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    Debug.LogError(
                        $"[StageProgress] {stageNumber}. aşama start API başarısız."
                    );

                    return;
                }

                StartedStages[stageNumber] = true;

                Debug.Log(
                    $"[StageProgress] {stageNumber}. aşama ilk kez açıldı."
                );
            }
        );
    }

    public static void CompleteStage(
     int stageNumber,
     Action<bool> onComplete = null
 )
    {
        if (stageNumber < 1 ||
            stageNumber > TotalStageCount)
        {
            Debug.LogError(
                $"Geçersiz aşama numarası: {stageNumber}"
            );

            onComplete?.Invoke(false);
            return;
        }

        if (GameApiClient.Instance == null)
        {
            Debug.LogError(
                "[StageProgress] GameApiClient bulunamadı."
            );

            onComplete?.Invoke(false);
            return;
        }

        GameApiClient.Instance.CompleteStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    Debug.LogError(
                        $"[StageProgress] {stageNumber}. aşama tamamlanamadı."
                    );

                    onComplete?.Invoke(false);
                    return;
                }

                StartedStages[stageNumber] = true;
                CompletedStages[stageNumber] = true;

                completedStageCount =
                    Mathf.Max(
                        completedStageCount,
                        stageNumber
                    );

                Debug.Log(
                    $"[StageProgress] {stageNumber}. aşama tamamlandı."
                );

                if (stageNumber >= TotalStageCount)
                {
                    GameApiClient.Instance.CompleteGame(
                        gameSuccess =>
                        {
                            if (gameSuccess)
                            {
                                isGameCompleted = true;
                            }

                            onComplete?.Invoke(true);
                        }
                    );

                    return;
                }

                onComplete?.Invoke(true);
            }
        );
    }

    public static void RetryStage(
        int stageNumber
    )
    {
        if (GameApiClient.Instance == null)
            return;

        GameApiClient.Instance.RetryStage(
            stageNumber,
            success =>
            {
                if (!success)
                {
                    Debug.LogWarning(
                        $"[StageProgress] {stageNumber}. aşama retry kaydedilemedi."
                    );
                }
            }
        );
    }

    public static void ResetProgress()
    {
        StartedStages.Clear();
        CompletedStages.Clear();

        completedStageCount = 0;
        isGameCompleted = false;
        reenteredSentThisSession = false;

        Debug.Log(
            "[StageProgress] Yerel önbellek sıfırlandı."
        );
    }
}