using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Sunucudaki UserGameProgress / UserStageProgress API'lerine erişen düşük seviye ağ katmanı.
/// Sahneye elle eklenmez; ilk sahne yüklenmeden önce kendi kendini oluşturup
/// DontDestroyOnLoad ile kalıcı hâle gelir (bkz. CloudTransitionController deseni).
/// </summary>
public class GameApiClient : MonoBehaviour
{
    public static GameApiClient Instance { get; private set; }

    // TODO: Login akışı eklendiğinde userId, giriş yapan kullanıcıdan gelecek.
    // Şimdilik Postman testlerindeki sabit test kullanıcısıyla aynı.
    private const int TestUserId = 3;
    private const string BaseUrl = "https://localhost:5001";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject host = new GameObject("GameApiClient");
        host.AddComponent<GameApiClient>();
    }

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

    public int UserId => TestUserId;

    // ─── PUBLIC API ──────────────────────────────────────────────────────────

    public void StartGame(Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/start";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    public void GetProgress(Action<bool, GameProgressSnapshot> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbGET, url, (success, body) =>
        {
            GameProgressSnapshot snapshot = success ? ParseProgress(body) : new GameProgressSnapshot();
            onComplete?.Invoke(success, snapshot);
        }));
    }

    public void StartStage(int stageId, Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/stages/{stageId}/start";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    public void RetryStage(int stageId, Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/stages/{stageId}/retry";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    public void CompleteStage(int stageId, Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/stages/{stageId}/completestage";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    public void CompleteGame(Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/completegamefinal";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    public void MarkReentered(Action<bool> onComplete)
    {
        string url = $"{BaseUrl}/api/game/users/{TestUserId}/reentered";
        StartCoroutine(SendRequest(UnityWebRequest.kHttpVerbPOST, url, (success, _) => onComplete?.Invoke(success)));
    }

    // ─── HTTP ────────────────────────────────────────────────────────────────

    private IEnumerator SendRequest(string method, string url, Action<bool, string> onComplete)
    {
        UnityWebRequest request;

        if (method == UnityWebRequest.kHttpVerbGET)
        {
            request = UnityWebRequest.Get(url);
        }
        else
        {
            request = new UnityWebRequest(url, method)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
        }

        // Yerel geliştirme sunucusu (localhost) genelde self-signed HTTPS sertifikası kullanır.
        // Bu bypass SADECE localhost/127.0.0.1 hedeflerinde aktif olur, üretim sunucusuna karşı kullanılmamalıdır.
        if (IsLocalDevUrl(url))
        {
            request.certificateHandler = new AcceptAllCertificatesHandler();
        }

        using (request)
        {
            yield return request.SendWebRequest();

            bool success = request.result == UnityWebRequest.Result.Success;
            string body = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;

            if (!success)
            {
                Debug.LogError(
                    $"[GameApiClient] {method} {url} başarısız: {request.error}\n" +
                    $"Yanıt: {body}"
                );
            }
            else
            {
                Debug.Log($"[GameApiClient] {method} {url} -> {request.responseCode}");
            }

            onComplete?.Invoke(success, body);
        }
    }

    private static bool IsLocalDevUrl(string url)
    {
        return url.Contains("localhost") || url.Contains("127.0.0.1");
    }

    private sealed class AcceptAllCertificatesHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }

    // ─── JSON PARSE (şema onaylanana kadar esnek/tahmini okuma) ───────────────

    private static GameProgressSnapshot ParseProgress(string json)
    {
        var snapshot = new GameProgressSnapshot();

        if (string.IsNullOrWhiteSpace(json))
        {
            return snapshot;
        }

        JObject root;

        try
        {
            root = JObject.Parse(json);
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"[GameApiClient] İlerleme JSON'ı parse edilemedi: {e.Message}\n" +
                $"Ham yanıt: {json}"
            );

            return snapshot;
        }

        snapshot.IsGameCompleted = ReadBool(root, "isCompleted", "IsCompleted", "completed", "Completed");

        JArray stages = FindArray(root, "stages", "Stages", "userStageProgresses", "UserStageProgresses");

        if (stages == null)
        {
            Debug.LogWarning(
                "[GameApiClient] İlerleme yanıtında aşama listesi bulunamadı. " +
                "Alan adları tahmin edilenden farklı olabilir (bkz. GameApiClient.ParseProgress). " +
                $"Ham yanıt: {json}"
            );

            return snapshot;
        }

        foreach (JToken token in stages)
        {
            if (token is not JObject stageObj)
            {
                continue;
            }

            int? stageId = ReadInt(stageObj, "stageId", "StageId", "stageNumber", "StageNumber");

            if (stageId == null)
            {
                continue;
            }

            bool completed =
                ReadBool(stageObj, "isCompleted", "IsCompleted") ||
                ReadNonNullValue(stageObj, "completedAt", "CompletedAt");

            snapshot.StageStarted[stageId.Value] = true;
            snapshot.StageCompleted[stageId.Value] = completed;
        }

        return snapshot;
    }

    private static JToken FindToken(JObject obj, params string[] candidateNames)
    {
        foreach (string name in candidateNames)
        {
            JToken token = obj.GetValue(name, StringComparison.OrdinalIgnoreCase);

            if (token != null && token.Type != JTokenType.Null)
            {
                return token;
            }
        }

        return null;
    }

    private static bool ReadBool(JObject obj, params string[] names)
    {
        JToken token = FindToken(obj, names);
        return token != null && token.Type == JTokenType.Boolean && token.Value<bool>();
    }

    private static int? ReadInt(JObject obj, params string[] names)
    {
        JToken token = FindToken(obj, names);

        if (token == null)
        {
            return null;
        }

        try
        {
            return token.Value<int>();
        }
        catch
        {
            return null;
        }
    }

    private static bool ReadNonNullValue(JObject obj, params string[] names)
    {
        return FindToken(obj, names) != null;
    }

    private static JArray FindArray(JObject obj, params string[] names)
    {
        return FindToken(obj, names) as JArray;
    }
}

/// <summary>
/// GET /api/game/users/{userId} yanıtının, oyunun ihtiyaç duyduğu kısmına indirgenmiş hâli.
/// Alan adları henüz gerçek şemayla doğrulanmadı; bkz. GameApiClient.ParseProgress.
/// </summary>
public class GameProgressSnapshot
{
    public bool IsGameCompleted;
    public readonly Dictionary<int, bool> StageStarted = new Dictionary<int, bool>();
    public readonly Dictionary<int, bool> StageCompleted = new Dictionary<int, bool>();
}
