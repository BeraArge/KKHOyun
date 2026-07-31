using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aşama ilerlemesinin oturum içi (in-memory) önbelleği ve sunucu API'siyle köprüsü.
/// Sunucu tek doğruluk kaynağıdır: PlayerPrefs kullanılmaz. Map sahnesi StageProgress.Initialize
/// ile taze veriyi çekmeden kilit/açık kararı vermez; aşama yöneticileri ise EnterStage/CompleteStage
/// üzerinden sunucuyu (kısa gecikmeyi oyuncuya hissettirmeden) günceller.
/// </summary>
public static class StageProgress
{
    public const int TotalStageCount = 7;

    private static readonly Dictionary<int, bool> _started = new Dictionary<int, bool>();
    private static readonly Dictionary<int, bool> _completed = new Dictionary<int, bool>();

    private static bool _isGameCompleted;
    private static bool _reenteredSentThisSession;

    public static int HighestCompletedStage
    {
        get
        {
            int highest = 0;

            for (int stage = 1; stage <= TotalStageCount; stage++)
            {
                if (_completed.TryGetValue(stage, out bool done) && done)
                {
                    highest = stage;
                }
                else
                {
                    break;
                }
            }

            return highest;
        }
    }

    public static int CurrentStage => HighestCompletedStage + 1;

    /// <summary>
    /// Oyunu (varsa mevcut kaydı) başlatır ve güncel ilerlemeyi sunucudan çeker.
    /// Map sahnesinin Start() metodunda, aşama kilit durumları çizilmeden önce çağrılmalıdır.
    /// </summary>
    public static void Initialize(Action onReady, Action<string> onError = null)
    {
        if (GameApiClient.Instance == null)
        {
            Debug.LogError("[StageProgress] GameApiClient bulunamadı.");
            onError?.Invoke("Sunucu bağlantısı kurulamadı.");
            return;
        }

        GameApiClient.Instance.StartGame(startSuccess =>
        {
            if (!startSuccess)
            {
                onError?.Invoke("Sunucuya bağlanılamadı. Lütfen tekrar deneyin.");
                return;
            }

            GameApiClient.Instance.GetProgress((getSuccess, snapshot) =>
            {
                if (!getSuccess)
                {
                    onError?.Invoke("İlerleme bilgisi alınamadı. Lütfen tekrar deneyin.");
                    return;
                }

                ApplySnapshot(snapshot);

                if (_isGameCompleted && !_reenteredSentThisSession)
                {
                    _reenteredSentThisSession = true;

                    GameApiClient.Instance.MarkReentered(success =>
                    {
                        if (!success)
                        {
                            Debug.LogWarning("[StageProgress] Yeniden giriş sunucuya iletilemedi.");
                        }
                    });
                }

                onReady?.Invoke();
            });
        });
    }

    private static void ApplySnapshot(GameProgressSnapshot snapshot)
    {
        _isGameCompleted = snapshot.IsGameCompleted;

        foreach (KeyValuePair<int, bool> pair in snapshot.StageStarted)
        {
            _started[pair.Key] = pair.Value;
        }

        foreach (KeyValuePair<int, bool> pair in snapshot.StageCompleted)
        {
            _completed[pair.Key] = pair.Value;
        }
    }

    /// <summary>
    /// Bir aşama sahnesi açıldığında çağrılır. Önbelleğe göre start ya da retry gönderir
    /// (fire-and-forget — aşamanın eğitim/oyun akışı network'ü beklemez).
    /// </summary>
    public static void EnterStage(int stageNumber)
    {
        if (GameApiClient.Instance == null)
        {
            Debug.LogError("[StageProgress] GameApiClient bulunamadı, EnterStage atlanıyor.");
            return;
        }

        bool alreadyStarted = _started.TryGetValue(stageNumber, out bool started) && started;
        bool alreadyCompleted = _completed.TryGetValue(stageNumber, out bool completed) && completed;

        if (alreadyStarted && !alreadyCompleted)
        {
            GameApiClient.Instance.RetryStage(stageNumber, success =>
            {
                if (!success)
                {
                    Debug.LogWarning($"[StageProgress] {stageNumber}. aşama için retry sunucuya iletilemedi.");
                }
            });
        }
        else
        {
            GameApiClient.Instance.StartStage(stageNumber, success =>
            {
                if (!success)
                {
                    Debug.LogWarning($"[StageProgress] {stageNumber}. aşama için start sunucuya iletilemedi.");
                }
            });
        }

        _started[stageNumber] = true;
    }

    /// <summary>
    /// Bir aşama tamamlandığında çağrılır (imza mevcut çağrı noktalarıyla aynı kalır).
    /// Önbellek hemen (iyimser) güncellenir; sunucu çağrısı arka planda yapılır.
    /// </summary>
    public static void CompleteStage(int stageNumber)
    {
        int highestCompleted = HighestCompletedStage;

        if (stageNumber <= highestCompleted)
        {
            Debug.Log($"{stageNumber}. aşama daha önce tamamlanmış.");
            return;
        }

        if (stageNumber != highestCompleted + 1)
        {
            Debug.LogWarning(
                $"Aşama sırası atlanamaz. Beklenen aşama: {highestCompleted + 1}, " +
                $"tamamlanmak istenen aşama: {stageNumber}"
            );

            return;
        }

        _started[stageNumber] = true;
        _completed[stageNumber] = true;

        Debug.Log($"{stageNumber}. aşama tamamlandı ve kaydedildi.");

        if (GameApiClient.Instance == null)
        {
            Debug.LogError("[StageProgress] GameApiClient bulunamadı, tamamlanma sunucuya iletilemedi.");
            return;
        }

        GameApiClient.Instance.CompleteStage(stageNumber, success =>
        {
            if (!success)
            {
                Debug.LogError($"[StageProgress] {stageNumber}. aşama sunucuya iletilemedi.");
                return;
            }

            if (stageNumber >= TotalStageCount)
            {
                GameApiClient.Instance.CompleteGame(gameSuccess =>
                {
                    if (gameSuccess)
                    {
                        _isGameCompleted = true;
                        Debug.Log("[StageProgress] Oyun tamamlandı ve kaydedildi.");
                    }
                    else
                    {
                        Debug.LogWarning("[StageProgress] Oyun tamamlama sunucuya iletilemedi.");
                    }
                });
            }
        });
    }

    /// <summary>
    /// Yalnızca yerel oturum önbelleğini sıfırlar (dev/editor kısayolu).
    /// Sunucudaki kayıtları etkilemez.
    /// </summary>
    public static void ResetProgress()
    {
        _started.Clear();
        _completed.Clear();
        _isGameCompleted = false;
        _reenteredSentThisSession = false;

        Debug.Log("[StageProgress] Yerel oturum ilerlemesi sıfırlandı (sunucudaki kayıtlar etkilenmez).");
    }
}
