using System.Collections;
using UnityEngine;

public class WalkingPhaseManager : MonoBehaviour
{
    [Header("References")]
    public Stage2OrderManager stage2Manager;
    public WarningPopupUI warningPopup;

    [Header("Gaz Cutscene")]
    public GameObject gasBubble;
    public GameObject happyHearts;

    [Header("Zamanlama (sn)")]
    public float walkDuration = 5f;
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
        // TODO: şimdilik sabit bekleme, yerine yürüme animasyonu eklenecek
        yield return new WaitForSeconds(walkDuration);

        if (gasBubble != null) gasBubble.SetActive(true);
        yield return new WaitForSeconds(heartsDelay);

        if (happyHearts != null) happyHearts.SetActive(true);
        yield return new WaitForSeconds(completeDelay);

        if (stage2Manager != null) stage2Manager.OnPhaseFComplete();
    }
}
