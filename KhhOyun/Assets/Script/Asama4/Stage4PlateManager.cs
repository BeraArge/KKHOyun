using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage4PlateManager : MonoBehaviour
{
    [Header("Popup")]
    public WarningPopupUI warningPopup;

    [Header("Dengeli Tabak")]
    public int requiredPlacements = 7;
    public int currentPlacements = 0;

    [Header("Aþama Geçiþi")]
    [SerializeField]
    private int stageNumber = 4;

    [SerializeField]
    private string mapSceneName = "Map";

    [SerializeField]
    private float mapReturnDelay = 0.4f;

    private bool stageCompleted;
    private bool isReturningToMap;

    private void OnEnable()
    {
        // Plate bölümü Farm tamamlandýktan sonra açýldýðýnda
        // sayaç temiz ve kullanýlabilir durumda baþlar.
        currentPlacements = 0;
        stageCompleted = false;
        isReturningToMap = false;
    }

    public void CorrectPlacement()
    {
        if (
            stageCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        currentPlacements++;

        if (
            currentPlacements >=
            requiredPlacements
        )
        {
            stageCompleted = true;

            StartCoroutine(
                CompleteStageRoutine()
            );
        }
    }

    public void WrongPlacement(
        string foodName
    )
    {
        if (
            stageCompleted ||
            isReturningToMap
        )
        {
            return;
        }

        ShowWarning(
            "Bu besin bu bölüme ait deðil.\n" +
            "Baþka bir bölümü dene."
        );
    }

    private IEnumerator CompleteStageRoutine()
    {
        if (warningPopup != null)
        {
            yield return warningPopup
                .ShowAndWaitForClose(
                    "Harika!\n" +
                    "Dengeli tabaðýn hazýr."
                );
        }
        else
        {
            Debug.LogWarning(
                "Harika! Dengeli tabaðýn hazýr."
            );
        }

        Debug.Log(
            "[Stage4PlateManager] Aþama 4 tamamlandý."
        );

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
                    stageCompleted = false;

                    ShowWarning(
                        "Aþama ilerlemesi kaydedilemedi.\n" +
                        "Lütfen internet baðlantýný kontrol edip tekrar dene."
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
                "[Stage4PlateManager] Map Scene Name alaný boþ."
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
                $"[Stage4PlateManager] '{mapSceneName}' sahnesi " +
                "Build Profiles içindeki Scene List'te bulunamadý."
            );

            isReturningToMap = false;
            yield break;
        }

        SceneManager.LoadScene(
            mapSceneName
        );
    }

    private void ShowWarning(
        string message
    )
    {
        if (warningPopup != null)
        {
            warningPopup.Show(message);
        }
        else
        {
            Debug.LogWarning(message);
        }
    }
}