public static class LoadingSceneData
{
    public static string TargetSceneName { get; private set; }

    public static void SetTargetScene(string sceneName)
    {
        TargetSceneName = sceneName;
    }

    public static void Clear()
    {
        TargetSceneName = "";
    }
}