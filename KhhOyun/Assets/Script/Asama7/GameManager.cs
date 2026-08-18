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


    // SADECE BUNU EKLİYORUZ
    [Header("Görev Bilgilendirme Popup")]
    [SerializeField]
    private WarningPopupUI warningPopup;


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


    // =========================================================
    // GENEL EĞİTİM
    // =========================================================

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


        // =====================================================
        // YENİ:
        // Genel eğitim bittikten sonra yalnızca Görev 1
        // bilgilendirmesini gösteriyoruz.
        // Görevin kendisine DOKUNMUYORUZ.
        // =====================================================

        if (warningPopup != null)
        {
            yield return warningPopup
                .ShowAndWaitForClose(
                    GetTaskWarningText(1)
                );
        }
        else
        {
            Debug.LogWarning(
                "[GameManager] WarningPopup atanmadı."
            );
        }


        gameStarted = true;


        // Eğitim ve popup bittikten sonra blocker kapanır.
        SetEducationBlockerActive(false);


        Debug.Log(
            "[GameManager] Aşama 7 eğitimi tamamlandı. Oyun başladı."
        );
    }


    // =========================================================
    // POPUP YAZILARI
    // =========================================================

    private string GetTaskWarningText(int taskId)
    {
        switch (taskId)
        {
            case 1:
                return "Ağaçtan 5 elma topla ve sepete yerleştir. Diğer meyvelere dokunmamaya dikkat et!";

            case 2:
                return "Tansiyon aletini kullan ve ekrandaki soruyu dikkatlice cevapla. Normal kan basıncı değerini seç.";

            case 3:
                return "Alarm çaldığında karşına farklı ilaçlar çıkacak. Sadece kullanman gereken 3 ilaçı seç.";

            case 4:
                return "Besinleri doğru gruplarla eşleştir.";

            case 5:
                return "Takvimi incele ve bugünden 1 hafta sonraki kontrol gününü işaretle.";

            default:
                return "Yeni görev başlıyor. Hazırsan devam et!";
        }
    }


    // =========================================================
    // SADECE SONRAKİ POPUP'I AÇAR
    // =========================================================

    private IEnumerator ShowNextTaskWarning(
        int taskId
    )
    {
        if (warningPopup == null)
        {
            yield break;
        }


        // Popup açıkken arkaya basılmasını engelle.
        SetEducationBlockerActive(true);


        yield return warningPopup
            .ShowAndWaitForClose(
                GetTaskWarningText(taskId)
            );


        // Popup kapandı.
        SetEducationBlockerActive(false);
    }


    // =========================================================
    // MEVCUT GÖREV TAMAMLAMA SİSTEMİ
    // =========================================================

    private void HandleTaskCompletion(
        int taskId
    )
    {
        // BU KISIM SENİN ESKİ KODUN.
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

            return;
        }


        // =====================================================
        // YENİ EKLENEN TEK MANTIK:
        //
        // Örneğin Task 1 kendi koduyla tamamlanınca event gelir.
        // Burada yalnızca Görev 2 popup'ı açılır.
        //
        // Görev 2'yi açmıyoruz.
        // Task1'i kapatmıyoruz.
        // Task2'yi başlatmıyoruz.
        // Mevcut task kodlarına karışmıyoruz.
        // =====================================================

        int nextTaskId = taskId + 1;


        if (nextTaskId <= totalTasks)
        {
            StartCoroutine(
                ShowNextTaskWarning(
                    nextTaskId
                )
            );
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