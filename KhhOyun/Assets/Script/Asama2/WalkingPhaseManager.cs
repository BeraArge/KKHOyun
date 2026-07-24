using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WalkingPhaseManager : MonoBehaviour
{
    [Header("References")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;

    [Header("Karakter Yürüme Animasyonu")]
    public Image characterImage;
    public Sprite[] walkFrames;
    public float frameDuration = 0.12f;
    public float walkDuration = 5f;
    public float walkDistance = 300f;

    [Header("Gaz Cutscene")]
    public GameObject gasBubble;
    public GameObject happyHearts;

    [Header("Zamanlama (sn)")]
    public float gasDelay = 1f;
    public float heartsDelay = 1f;
    public float completeDelay = 2f;

    public void StartPhase()
    {
        if (gasBubble != null) gasBubble.SetActive(false);
        if (happyHearts != null) happyHearts.SetActive(false);

        StartCoroutine(PhaseFRoutine());
    }

    private IEnumerator PhaseFRoutine()
    {
        yield return WalkAnimationRoutine();

        yield return new WaitForSeconds(gasDelay);

        if (gasBubble != null) gasBubble.SetActive(true);
        yield return new WaitForSeconds(heartsDelay);

        if (happyHearts != null) happyHearts.SetActive(true);
        yield return new WaitForSeconds(completeDelay);

        if (stage2Manager != null) stage2Manager.OnPhaseFComplete();
    }

    private IEnumerator WalkAnimationRoutine()
    {
        RectTransform rt = characterImage != null ? characterImage.rectTransform : null;
        Vector2 startPos = rt != null ? rt.anchoredPosition : Vector2.zero;
        Vector2 targetPos = startPos + Vector2.left * walkDistance;
        bool hasFrames = characterImage != null && walkFrames != null && walkFrames.Length > 0;

        if (hasFrames) characterImage.sprite = walkFrames[0];

        float elapsed = 0f;
        float frameTimer = 0f;
        int frameIndex = 0;

        while (elapsed < walkDuration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            frameTimer += dt;

            if (hasFrames && frameTimer >= frameDuration)
            {
                frameTimer -= frameDuration;
                frameIndex++;
                characterImage.sprite = walkFrames[frameIndex % walkFrames.Length];
            }

            if (rt != null)
                rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.Clamp01(elapsed / walkDuration));

            yield return null;
        }

        if (hasFrames) characterImage.sprite = walkFrames[walkFrames.Length - 1];
        if (rt != null) rt.anchoredPosition = targetPos;
    }
}
