using UnityEngine;

public static class StageProgress
{
    private const string ProgressKey =
        "HighestCompletedStage";

    public const int TotalStageCount = 7;

    public static int HighestCompletedStage
    {
        get
        {
            return Mathf.Clamp(
                PlayerPrefs.GetInt(ProgressKey, 0),
                0,
                TotalStageCount
            );
        }
    }

    public static bool CanOpenStage(int stageNumber)
    {
        return stageNumber <= HighestCompletedStage + 1;
    }

    public static void CompleteStage(int stageNumber)
    {
        int completed = HighestCompletedStage;

        if (stageNumber <= completed)
        {
            return;
        }

        // Aþama atlanmasýný engelle.
        if (stageNumber != completed + 1)
        {
            Debug.LogWarning(
                $"Aþama atlanamaz. Beklenen: {completed + 1}, " +
                $"Týklanan: {stageNumber}"
            );

            return;
        }

        PlayerPrefs.SetInt(ProgressKey, stageNumber);
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(ProgressKey);
        PlayerPrefs.Save();
    }
}