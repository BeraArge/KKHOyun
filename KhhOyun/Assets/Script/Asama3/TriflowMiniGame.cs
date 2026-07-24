using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriflowMiniGame : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("GIF Kare Dizisi")]
    public Image frameImage;
    public Sprite[] frames;

    [Header("Tuning")]
    public float fillRate = 0.6f;
    public float drainRate = 0.3f;
    public int repsRequired = 3;
    public float restDuration = 0.6f;

    [Header("References")]
    public PostOpActivityManager roomManager;

    [Header("Optional UI")]
    public TMP_Text repsCounterText;

    private bool isHeld = false;
    private bool isResting = false;
    private bool isComplete = false;
    private float currentFill = 0f;
    private int currentReps = 0;
    private float restTimer = 0f;

    private void Start()
    {
        UpdateFrame();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isComplete && !isResting)
        {
            isHeld = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHeld = false;
    }

    private void Update()
    {
        if (isComplete)
        {
            return;
        }

        if (isResting)
        {
            restTimer -= Time.deltaTime;
            if (restTimer <= 0f)
            {
                isResting = false;
            }

            return;
        }

        currentFill += (isHeld ? fillRate : -drainRate) * Time.deltaTime;
        currentFill = Mathf.Clamp01(currentFill);

        UpdateFrame();

        if (currentFill >= 1f)
        {
            CompleteRep();
        }
    }

    private void UpdateFrame()
    {
        if (frameImage == null || frames == null || frames.Length == 0)
        {
            return;
        }

        int frameIndex = Mathf.Clamp(Mathf.RoundToInt(currentFill * (frames.Length - 1)), 0, frames.Length - 1);
        frameImage.sprite = frames[frameIndex];
    }

    private void CompleteRep()
    {
        currentReps++;
        currentFill = 0f;
        isHeld = false;
        isResting = true;
        restTimer = restDuration;

        UpdateFrame();
        UpdateRepsText();

        if (currentReps >= repsRequired)
        {
            isComplete = true;

            if (roomManager != null)
            {
                roomManager.OnTriflowComplete();
            }
            else
            {
                Debug.LogError("TriflowMiniGame: roomManager atanmadı.");
            }
        }
    }

    private void UpdateRepsText()
    {
        if (repsCounterText != null)
        {
            repsCounterText.text = currentReps + "/" + repsRequired;
        }
    }
}
