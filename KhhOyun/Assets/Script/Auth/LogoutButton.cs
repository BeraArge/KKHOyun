using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutButton : MonoBehaviour
{
    [SerializeField]
    private string authSceneName = "Auth";

    public void Logout()
    {
        // Giriþ yapan kullanýcýnýn yerel oturumunu kapatýr.
        AppSession.Logout();

        // Önceki kullanýcýnýn bellekte tutulan aþama
        // ilerlemesini temizler.
        StageProgress.ResetProgress();

        if (
            string.IsNullOrWhiteSpace(
                authSceneName
            )
        )
        {
            Debug.LogError(
                "LogoutButton: Auth Scene Name alaný boþ."
            );

            return;
        }

        if (
            !Application.CanStreamedLevelBeLoaded(
                authSceneName
            )
        )
        {
            Debug.LogError(
                $"LogoutButton: '{authSceneName}' sahnesi " +
                "Build Profiles içindeki Scene List'te bulunamadý."
            );

            return;
        }

        SceneManager.LoadScene(
            authSceneName
        );
    }
}