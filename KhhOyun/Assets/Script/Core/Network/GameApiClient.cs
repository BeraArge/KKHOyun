using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// UserGameProgress / UserStageProgress API bağlantılarını yönetir.
/// Sahneye eklenmez; uygulama açılırken otomatik oluşur ve sahneler arasında kalır.
/// </summary>
public class GameApiClient : MonoBehaviour
{
    public static GameApiClient Instance { get; private set; }

    [Header("API")]
    public static string BaseUrl = "https://heart-j.com";

    public int UserId => AppSession.UserId;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

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

    public void StartGame(Action<bool> onComplete)
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/start";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    public void GetProgress(
        Action<bool, GameProgressSnapshot> onComplete
    )
    {
        if (AppSession.UserId <= 0)
        {
            Debug.LogError(
                "[GameApiClient] Geçerli kullanıcı oturumu bulunamadı."
            );

            onComplete?.Invoke(
                false,
                new GameProgressSnapshot()
            );

            return;
        }

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{AppSession.UserId}";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbGET,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    GameProgressSnapshot snapshot =
                        apiSuccess
                            ? ParseProgress(body)
                            : new GameProgressSnapshot();

                    onComplete?.Invoke(
                        apiSuccess,
                        snapshot
                    );
                }
            )
        );
    }

    public void StartStage(
        int stageId,
        Action<bool> onComplete
    )
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/stages/{stageId}/start";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    public void RetryStage(
        int stageId,
        Action<bool> onComplete
    )
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/stages/{stageId}/retry";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    public void CompleteStage(
        int stageId,
        Action<bool> onComplete
    )
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/stages/{stageId}/completestage";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    public void CompleteGame(Action<bool> onComplete)
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/completegamefinal";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    public void MarkReentered(Action<bool> onComplete)
    {
        if (!TryGetUserId(out int userId, onComplete))
            return;

        string url =
            $"{BaseUrl.TrimEnd('/')}/api/game/users/{userId}/reentered";

        StartCoroutine(
            SendRequest(
                UnityWebRequest.kHttpVerbPOST,
                url,
                (success, body) =>
                {
                    bool apiSuccess =
                        success && ReadIsSuccess(body);

                    onComplete?.Invoke(apiSuccess);
                }
            )
        );
    }

    private bool TryGetUserId(
        out int userId,
        Action<bool> onComplete
    )
    {
        userId = AppSession.UserId;

        if (userId > 0)
            return true;

        Debug.LogError(
            "[GameApiClient] Geçerli kullanıcı oturumu bulunamadı."
        );

        onComplete?.Invoke(false);
        return false;
    }

    private IEnumerator SendRequest(
        string method,
        string url,
        Action<bool, string> onComplete
    )
    {
        UnityWebRequest request;

        if (method == UnityWebRequest.kHttpVerbGET)
        {
            request = UnityWebRequest.Get(url);
        }
        else
        {
            request = new UnityWebRequest(url, method);
            request.downloadHandler =
                new DownloadHandlerBuffer();
        }

        request.timeout = 20;

        if (IsLocalDevUrl(url))
        {
            request.certificateHandler =
                new AcceptAllCertificatesHandler();
        }

        using (request)
        {
            yield return request.SendWebRequest();

            string body =
                request.downloadHandler != null
                    ? request.downloadHandler.text
                    : string.Empty;

            bool transportSuccess =
                request.result ==
                UnityWebRequest.Result.Success;

            Debug.Log(
                $"[GameApiClient] {method} {url}\n" +
                $"Result: {request.result}\n" +
                $"HTTP: {request.responseCode}\n" +
                $"Error: {request.error}\n" +
                $"Body:\n{body}"
            );

            onComplete?.Invoke(
                transportSuccess,
                body
            );
        }
    }

    private static bool ReadIsSuccess(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            JObject root = JObject.Parse(json);

            JToken token =
                root.GetValue(
                    "isSuccess",
                    StringComparison.OrdinalIgnoreCase
                );

            return token != null &&
                   token.Type == JTokenType.Boolean &&
                   token.Value<bool>();
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[GameApiClient] isSuccess okunamadı: " +
                exception.Message
            );

            return false;
        }
    }

    private static GameProgressSnapshot ParseProgress(
        string json
    )
    {
        GameProgressSnapshot snapshot =
            new GameProgressSnapshot();

        if (string.IsNullOrWhiteSpace(json))
            return snapshot;

        try
        {
            JObject root = JObject.Parse(json);

            JObject data =
                root["data"] as JObject;

            if (data == null)
            {
                Debug.LogError(
                    "[GameApiClient] Progress cevabında data alanı bulunamadı."
                );

                return snapshot;
            }

            snapshot.GameProgressId =
                data.Value<int?>("id") ?? 0;

            snapshot.UserId =
                data.Value<int?>("userId") ?? 0;

            snapshot.IsGameCompleted =
                data.Value<bool?>("isCompleted") ?? false;

            snapshot.ReenteredAfterCompletion =
                data.Value<bool?>(
                    "reenteredAfterCompletion"
                ) ?? false;

            snapshot.CompletedStageCount =
                data.Value<int?>(
                    "completedStageCount"
                ) ?? 0;

            snapshot.StartedAtUtc =
                data.Value<string>(
                    "startedAtUtc"
                );

            snapshot.CompletedAtUtc =
                data.Value<string>(
                    "completedAtUtc"
                );

            JArray stages =
                data["stages"] as JArray;

            if (stages == null)
                return snapshot;

            foreach (JToken token in stages)
            {
                if (token is not JObject stageObject)
                    continue;

                int stageId =
                    stageObject.Value<int?>(
                        "stageId"
                    ) ?? 0;

                if (stageId <= 0)
                    continue;

                bool isCompleted =
                    stageObject.Value<bool?>(
                        "isCompleted"
                    ) ?? false;

                snapshot.StageStarted[stageId] =
                    true;

                snapshot.StageCompleted[stageId] =
                    isCompleted;
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(
                "[GameApiClient] Progress JSON parse hatası: " +
                exception.Message +
                "\nHam cevap:\n" +
                json
            );
        }

        return snapshot;
    }

    private static bool IsLocalDevUrl(string url)
    {
        return
            url.Contains("localhost") ||
            url.Contains("127.0.0.1");
    }

    private sealed class AcceptAllCertificatesHandler
        : CertificateHandler
    {
        protected override bool ValidateCertificate(
            byte[] certificateData
        )
        {
            return true;
        }
    }
}

public class GameProgressSnapshot
{
    public int GameProgressId;
    public int UserId;
    public bool IsGameCompleted;
    public bool ReenteredAfterCompletion;
    public int CompletedStageCount;
    public string StartedAtUtc;
    public string CompletedAtUtc;

    public readonly Dictionary<int, bool>
        StageStarted =
            new Dictionary<int, bool>();

    public readonly Dictionary<int, bool>
        StageCompleted =
            new Dictionary<int, bool>();
}