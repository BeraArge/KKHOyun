using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    [Header("Oyunun Orijinal Oraný (1920x1080 = 16:9)")]
    public float targetAspect = 16f / 9f;

    void Start()
    {
        LockAspectRatio();
    }

    // Simülatörde anlýk görebilmek için Update'e ekliyoruz
    void Update()
    {
#if UNITY_EDITOR
        LockAspectRatio();
#endif
    }

    void LockAspectRatio()
    {
        // Cihazýn mevcut ekran oranýný hesapla
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera cam = GetComponent<Camera>();

        // Eðer ekran oyundan daha geniþse (Note 20 Ultra gibi)
        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        // Eðer ekran daha kareyse (Z Fold, iPad gibi)
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}