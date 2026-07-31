using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthUIManager : MonoBehaviour
{
    [Header("API")]
    [SerializeField] private string baseUrl = "https://localhost:5001";
    [SerializeField] private string loginEndpoint = "/api/auth/login";
    [SerializeField] private string registerEndpoint = "/api/auth/register";

    [Header("Sahne Geçiþleri")]
    [SerializeField] private string mapSceneName = "Map";
    [SerializeField] private float mapLoadDelay = 0.25f;

    [Header("Popup")]
    [SerializeField] private WarningPopupUI warningPopup;

    [Header("Paneller")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Üst Sekmeler")]
    [SerializeField] private Button loginTabButton;
    [SerializeField] private Button registerTabButton;

    [Header("Alt Geçiþler")]
    [SerializeField] private Button goRegisterButton;
    [SerializeField] private Button goLoginButton;

    [Header("Giriþ")]
    [SerializeField] private TMP_InputField loginUsernameInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private Button loginButton;

    [Header("Kayýt")]
    [SerializeField] private TMP_InputField registerNameInput;
    [SerializeField] private TMP_InputField registerSurnameInput;
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerPasswordRepeatInput;
    [SerializeField] private Button registerButton;

    [Header("Loading")]
    [SerializeField] private GameObject loadingOverlay;

    private bool requestInProgress;
    private bool sceneTransitionInProgress;

    private void Awake()
    {
        BindButtons();
        SetLoading(false);
        ShowLoginPanel();
        LoadRememberedUsername();

        // Kullanýcý daha önce baþarýlý giriþ yaptýysa ve çýkýþ yapmadýysa
        // Auth ekranýný göstermeden Map sahnesine geç.
        if (AppSession.HasActiveSession)
        {
            StartCoroutine(LoadMapRoutine());
        }
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    private void BindButtons()
    {
        loginTabButton?.onClick.AddListener(ShowLoginPanel);
        registerTabButton?.onClick.AddListener(ShowRegisterPanel);
        goRegisterButton?.onClick.AddListener(ShowRegisterPanel);
        goLoginButton?.onClick.AddListener(ShowLoginPanel);
        loginButton?.onClick.AddListener(OnLoginClicked);
        registerButton?.onClick.AddListener(OnRegisterClicked);
    }

    private void UnbindButtons()
    {
        loginTabButton?.onClick.RemoveListener(ShowLoginPanel);
        registerTabButton?.onClick.RemoveListener(ShowRegisterPanel);
        goRegisterButton?.onClick.RemoveListener(ShowRegisterPanel);
        goLoginButton?.onClick.RemoveListener(ShowLoginPanel);
        loginButton?.onClick.RemoveListener(OnLoginClicked);
        registerButton?.onClick.RemoveListener(OnRegisterClicked);
    }

    public void ShowLoginPanel()
    {
        if (requestInProgress || sceneTransitionInProgress)
            return;

        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (registerPanel != null)
            registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        if (requestInProgress || sceneTransitionInProgress)
            return;

        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (registerPanel != null)
            registerPanel.SetActive(true);
    }

    private void OnLoginClicked()
    {
        if (requestInProgress || sceneTransitionInProgress)
            return;

        string username = loginUsernameInput != null
            ? loginUsernameInput.text.Trim()
            : string.Empty;

        string password = loginPasswordInput != null
            ? loginPasswordInput.text
            : string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            ShowPopup("Kullanýcý adý alanýný doldurmalýsýn.");
            loginUsernameInput?.ActivateInputField();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowPopup("Þifre alanýný doldurmalýsýn.");
            loginPasswordInput?.ActivateInputField();
            return;
        }

        StartCoroutine(LoginRoutine(username, password));
    }

    private IEnumerator LoginRoutine(string username, string password)
    {
        SetRequestState(true);

        LoginRequest model = new LoginRequest
        {
            username = username,
            password = password
        };

        using UnityWebRequest request = CreatePostRequest(
            BuildUrl(loginEndpoint),
            JsonUtility.ToJson(model)
        );

        yield return request.SendWebRequest();

        SetRequestState(false);

        if (HasConnectionError(request))
        {
            ShowPopup(
                "Sunucuya baðlanýlamadý. API adresini ve sunucuyu kontrol et."
            );
            yield break;
        }

        string responseJson = request.downloadHandler != null
            ? request.downloadHandler.text
            : string.Empty;

        if (!TryParseAuthResponse(responseJson, out AuthResponse response))
        {
            ShowPopup("Sunucudan gelen cevap okunamadý.");
            yield break;
        }

        bool isSuccess = response.isSuccess || response.success;

        string responseMessage = GetResponseMessage(
            response,
            isSuccess ? "Giriþ baþarýlý." : "Giriþ yapýlamadý."
        );

        if (!isSuccess)
        {
            ShowPopup(responseMessage);
            yield break;
        }

        if (response.data == null || response.data.id <= 0)
        {
            ShowPopup("Giriþ baþarýlý ancak kullanýcý bilgileri alýnamadý.");
            yield break;
        }

        // Oturum bilgileri, Beni Hatýrla seçili olsun veya olmasýn kalýcý tutulur.
        // Kullanýcý yalnýzca Çýkýþ Yap dediðinde silinir.
        AppSession.Save(
            response.data.id,
            response.data.username,
            response.data.name,
            response.data.surname,
            response.data.roleDTO != null ? response.data.roleDTO.id : 0,
            response.data.roleDTO != null ? response.data.roleDTO.name : string.Empty
        );

        // Beni Hatýrla yalnýzca giriþ alanýna kullanýcý adýný tekrar yazar.
        SaveRememberedUsername(username);

        Debug.Log(
            "[AuthUIManager] Giriþ baþarýlý. Kullanýcý oturumu kaydedildi.\n" +
            responseJson
        );

        yield return ShowPopupAndWait(responseMessage);
        yield return LoadMapRoutine();
    }

    private void OnRegisterClicked()
    {
        if (requestInProgress || sceneTransitionInProgress)
            return;

        string name = registerNameInput != null
            ? registerNameInput.text.Trim()
            : string.Empty;

        string surname = registerSurnameInput != null
            ? registerSurnameInput.text.Trim()
            : string.Empty;

        string username = registerUsernameInput != null
            ? registerUsernameInput.text.Trim()
            : string.Empty;

        string password = registerPasswordInput != null
            ? registerPasswordInput.text
            : string.Empty;

        string passwordRepeat = registerPasswordRepeatInput != null
            ? registerPasswordRepeatInput.text
            : string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            ShowPopup("Ad alanýný doldurmalýsýn.");
            registerNameInput?.ActivateInputField();
            return;
        }

        if (string.IsNullOrWhiteSpace(surname))
        {
            ShowPopup("Soyad alanýný doldurmalýsýn.");
            registerSurnameInput?.ActivateInputField();
            return;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            ShowPopup("Kullanýcý adý alanýný doldurmalýsýn.");
            registerUsernameInput?.ActivateInputField();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowPopup("Þifre alanýný doldurmalýsýn.");
            registerPasswordInput?.ActivateInputField();
            return;
        }

        if (string.IsNullOrWhiteSpace(passwordRepeat))
        {
            ShowPopup("Þifre tekrar alanýný doldurmalýsýn.");
            registerPasswordRepeatInput?.ActivateInputField();
            return;
        }

        if (password.Length < 6)
        {
            ShowPopup("Þifre en az 6 karakter olmalýdýr.");
            registerPasswordInput?.ActivateInputField();
            return;
        }

        if (password != passwordRepeat)
        {
            ShowPopup("Þifreler birbiriyle ayný olmalýdýr.");
            registerPasswordRepeatInput?.ActivateInputField();
            return;
        }

        StartCoroutine(RegisterRoutine(
            name,
            surname,
            username,
            password,
            passwordRepeat
        ));
    }

    private IEnumerator RegisterRoutine(
        string name,
        string surname,
        string username,
        string password,
        string passwordRepeat)
    {
        SetRequestState(true);

        RegisterRequest model = new RegisterRequest
        {
            name = name,
            surname = surname,
            username = username,
            password = password,
            passwordRepeat = passwordRepeat
        };

        using UnityWebRequest request = CreatePostRequest(
            BuildUrl(registerEndpoint),
            JsonUtility.ToJson(model)
        );

        yield return request.SendWebRequest();

        SetRequestState(false);

        if (HasConnectionError(request))
        {
            ShowPopup(
                "Sunucuya baðlanýlamadý. API adresini ve sunucuyu kontrol et."
            );
            yield break;
        }

        string responseJson = request.downloadHandler != null
            ? request.downloadHandler.text
            : string.Empty;

        if (!TryParseAuthResponse(responseJson, out AuthResponse response))
        {
            ShowPopup("Sunucudan gelen cevap okunamadý.");
            yield break;
        }

        bool isSuccess = response.isSuccess || response.success;

        string responseMessage = GetResponseMessage(
            response,
            isSuccess ? "Kayýt baþarýlý." : "Kayýt oluþturulamadý."
        );

        if (!isSuccess)
        {
            ShowPopup(responseMessage);
            yield break;
        }

        yield return ShowPopupAndWait(responseMessage);

        if (loginUsernameInput != null)
            loginUsernameInput.text = username;

        if (loginPasswordInput != null)
            loginPasswordInput.text = string.Empty;

        ClearRegisterInputs();
        ShowLoginPanel();
    }

    private void SaveRememberedUsername(string username)
    {
        bool rememberUsername =
            rememberMeToggle != null &&
            rememberMeToggle.isOn;

        PlayerPrefs.SetInt(
            "RememberUsername",
            rememberUsername ? 1 : 0
        );

        if (rememberUsername)
        {
            PlayerPrefs.SetString(
                "RememberedUsername",
                username
            );
        }
        else
        {
            PlayerPrefs.DeleteKey(
                "RememberedUsername"
            );
        }

        PlayerPrefs.Save();
    }

    private void LoadRememberedUsername()
    {
        bool rememberUsername =
            PlayerPrefs.GetInt(
                "RememberUsername",
                0
            ) == 1;

        if (rememberMeToggle != null)
            rememberMeToggle.isOn = rememberUsername;

        if (rememberUsername && loginUsernameInput != null)
        {
            loginUsernameInput.text =
                PlayerPrefs.GetString(
                    "RememberedUsername",
                    string.Empty
                );
        }
    }

    private IEnumerator LoadMapRoutine()
    {
        if (sceneTransitionInProgress)
            yield break;

        sceneTransitionInProgress = true;
        SetLoading(true);

        yield return new WaitForSecondsRealtime(mapLoadDelay);

        if (string.IsNullOrWhiteSpace(mapSceneName))
        {
            SetLoading(false);
            sceneTransitionInProgress = false;
            ShowPopup("Map Scene Name alaný boþ.");
            yield break;
        }

        if (!Application.CanStreamedLevelBeLoaded(mapSceneName))
        {
            SetLoading(false);
            sceneTransitionInProgress = false;

            ShowPopup(
                $"'{mapSceneName}' sahnesi Build Profiles Scene List içinde bulunamadý."
            );
            yield break;
        }

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(mapSceneName);

        if (operation == null)
        {
            SetLoading(false);
            sceneTransitionInProgress = false;
            ShowPopup("Map sahnesi yüklenemedi.");
            yield break;
        }

        while (!operation.isDone)
            yield return null;
    }

    private IEnumerator ShowPopupAndWait(string message)
    {
        if (warningPopup != null)
        {
            yield return warningPopup.ShowAndWaitForClose(message);
        }
        else
        {
            Debug.LogWarning(message);
            yield return new WaitForSecondsRealtime(0.35f);
        }
    }

    private bool TryParseAuthResponse(
        string responseJson,
        out AuthResponse response)
    {
        response = null;

        if (string.IsNullOrWhiteSpace(responseJson))
            return false;

        try
        {
            response =
                JsonUtility.FromJson<AuthResponse>(
                    responseJson
                );

            return response != null;
        }
        catch
        {
            Debug.LogError(
                "[AuthUIManager] JSON okunamadý:\n" +
                responseJson
            );

            return false;
        }
    }

    private UnityWebRequest CreatePostRequest(
        string url,
        string json)
    {
        byte[] body =
            Encoding.UTF8.GetBytes(json);

        UnityWebRequest request =
            new UnityWebRequest(
                url,
                UnityWebRequest.kHttpVerbPOST
            );

        request.uploadHandler =
            new UploadHandlerRaw(body);

        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        request.timeout = 20;

        return request;
    }

    private bool HasConnectionError(UnityWebRequest request)
    {
        return
            request.result ==
            UnityWebRequest.Result.ConnectionError ||
            request.result ==
            UnityWebRequest.Result.DataProcessingError;
    }

    private string BuildUrl(string endpoint)
    {
        string root =
            (baseUrl ?? string.Empty)
            .TrimEnd('/');

        string path =
            (endpoint ?? string.Empty)
            .Trim();

        if (!path.StartsWith("/"))
            path = "/" + path;

        return root + path;
    }

    private string GetResponseMessage(
        AuthResponse response,
        string fallbackMessage)
    {
        if (response == null)
            return fallbackMessage;

        if (!string.IsNullOrWhiteSpace(response.message))
            return response.message;

        if (!string.IsNullOrWhiteSpace(response.reason))
            return response.reason;

        return fallbackMessage;
    }

    private void ClearRegisterInputs()
    {
        if (registerNameInput != null)
            registerNameInput.text = string.Empty;

        if (registerSurnameInput != null)
            registerSurnameInput.text = string.Empty;

        if (registerUsernameInput != null)
            registerUsernameInput.text = string.Empty;

        if (registerPasswordInput != null)
            registerPasswordInput.text = string.Empty;

        if (registerPasswordRepeatInput != null)
            registerPasswordRepeatInput.text = string.Empty;
    }

    private void SetRequestState(bool active)
    {
        requestInProgress = active;
        SetLoading(active);

        if (loginButton != null)
            loginButton.interactable = !active;

        if (registerButton != null)
            registerButton.interactable = !active;

        if (loginTabButton != null)
            loginTabButton.interactable = !active;

        if (registerTabButton != null)
            registerTabButton.interactable = !active;

        if (goRegisterButton != null)
            goRegisterButton.interactable = !active;

        if (goLoginButton != null)
            goLoginButton.interactable = !active;
    }

    private void SetLoading(bool active)
    {
        if (loadingOverlay != null)
            loadingOverlay.SetActive(active);
    }

    private void ShowPopup(string message)
    {
        if (warningPopup != null)
            warningPopup.Show(message);
        else
            Debug.LogWarning(message);
    }

    [System.Serializable]
    private class LoginRequest
    {
        public string username;
        public string password;
    }

    [System.Serializable]
    private class RegisterRequest
    {
        public string name;
        public string surname;
        public string username;
        public string password;
        public string passwordRepeat;
    }

    [System.Serializable]
    private class AuthResponse
    {
        public bool isSuccess;
        public bool success;
        public AuthData data;
        public string message;
        public string reason;
        public string redirect;
        public int resultType;
    }

    [System.Serializable]
    private class AuthData
    {
        public int id;
        public int rolId;
        public string username;
        public string name;
        public string surname;
        public AccessTokenData accessToken;
        public RoleData roleDTO;
    }

    [System.Serializable]
    private class AccessTokenData
    {
        public string token;
        public string expiration;
    }

    [System.Serializable]
    private class RoleData
    {
        public int id;
        public string name;
    }
}