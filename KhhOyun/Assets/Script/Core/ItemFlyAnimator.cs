using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemFlyAnimator : MonoBehaviour
{
    public RectTransform flyingLayer;

    [Header("Animation")]
    public float duration = 0.55f;
    public float arcHeight = 90f;
    public float endScaleValue = 0.85f;

    public void FlyToSlot(Image sourceImage, RectTransform targetSlot, Action onComplete = null)
    {
        if (sourceImage == null || targetSlot == null || flyingLayer == null)
            return;

        StartCoroutine(FlyRoutine(sourceImage, targetSlot, onComplete));
    }

    private IEnumerator FlyRoutine(Image sourceImage, RectTransform targetSlot, Action onComplete)
    {
        GameObject clone = new GameObject("Flying_" + sourceImage.name);
        clone.transform.SetParent(flyingLayer, false);

        Image cloneImage = clone.AddComponent<Image>();
        cloneImage.sprite = sourceImage.sprite;
        cloneImage.preserveAspect = true;
        cloneImage.raycastTarget = false;

        RectTransform cloneRect = clone.GetComponent<RectTransform>();
        RectTransform sourceRect = sourceImage.GetComponent<RectTransform>();

        cloneRect.position = sourceRect.position;

        // Ekranda görünen gerçek boyutu alýr.
        Vector3[] corners = new Vector3[4];
        sourceRect.GetWorldCorners(corners);

        float worldWidth = Vector3.Distance(corners[0], corners[3]);
        float worldHeight = Vector3.Distance(corners[0], corners[1]);

        cloneRect.sizeDelta = new Vector2(worldWidth, worldHeight);
        cloneRect.localScale = Vector3.one;

        Vector3 startPos = cloneRect.position;
        Vector3 endPos = targetSlot.position;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * endScaleValue;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float eased = Mathf.SmoothStep(0f, 1f, p);

            Vector3 pos = Vector3.Lerp(startPos, endPos, eased);
            pos.y += Mathf.Sin(p * Mathf.PI) * arcHeight;

            cloneRect.position = pos;
            cloneRect.localScale = Vector3.Lerp(startScale, endScale, eased);

            yield return null;
        }

        Destroy(clone);
        onComplete?.Invoke();
    }
}