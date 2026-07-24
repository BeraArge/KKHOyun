using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class EducationVideoManager : MonoBehaviour
{
    [Header("References")]
    public Stage2OrderManager stage2Manager;

    [Header("Video")]
    public VideoPlayer videoPlayer;
    public VideoClip clip;

    [Header("UI")]
    public TMP_Text dayTitleText;

    private bool subscribed = false;
    private bool completed = false;

    public void StartPhase()
    {
        completed = false;

        if (dayTitleText != null)
            dayTitleText.text = "2. - 3. Gün";

        if (videoPlayer == null)
        {
            Debug.LogError("[EVM] videoPlayer NULL — Inspector'da bağla!");
            CompletePhase();
            return;
        }

        if (clip == null)
        {
            Debug.LogError("[EVM] video clip atanmamış — Inspector'da bağla!");
            CompletePhase();
            return;
        }

        if (!subscribed)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
            subscribed = true;
        }

        videoPlayer.clip = clip;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        CompletePhase();
    }

    private void CompletePhase()
    {
        if (completed) return;
        completed = true;
        if (stage2Manager != null) stage2Manager.OnPhaseGComplete();
    }

    private void OnDestroy()
    {
        if (subscribed && videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
