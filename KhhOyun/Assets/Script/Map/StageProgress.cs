using UnityEngine;

public static class StageProgress
{
    private const string CompletedStageKey = "HighestCompletedStage";

    public const int TotalStageCount = 7;

    public static int HighestCompletedStage
    {
        get
        {
            return Mathf.Clamp(
                PlayerPrefs.GetInt(CompletedStageKey, 0),
                0,
                TotalStageCount
            );
        }
    }

    public static int CurrentStage
    {
        get
        {
            return HighestCompletedStage + 1;
        }
    }

    public static void CompleteStage(int stageNumber)
    {
        int highestCompleted = HighestCompletedStage;

        // Daha önce tamamlanmýþ bir aþama tekrar oynanýyorsa
        // ilerleme deðiþmez.
        if (stageNumber <= highestCompleted)
        {
            Debug.Log(
                $"{stageNumber}. aþama daha önce tamamlanmýþ."
            );

            return;
        }

        // Aþama atlamayý engeller.
        if (stageNumber != highestCompleted + 1)
        {
            Debug.LogWarning(
                $"Aþama sýrasý atlanamaz. " +
                $"Beklenen aþama: {highestCompleted + 1}, " +
                $"tamamlanmak istenen aþama: {stageNumber}"
            );

            return;
        }

        PlayerPrefs.SetInt(
            CompletedStageKey,
            stageNumber
        );

        PlayerPrefs.Save();

        Debug.Log(
            $"{stageNumber}. aþama tamamlandý ve kaydedildi."
        );
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CompletedStageKey);
        PlayerPrefs.Save();

        Debug.Log("Aþama ilerlemesi sýfýrlandý.");
    }
}