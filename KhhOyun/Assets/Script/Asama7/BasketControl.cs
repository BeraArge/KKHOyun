using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class BasketControl : MonoBehaviour
{

    

    [Header("Görev Ayarlarý")]
    [SerializeField] private int targetAppleNum = 5;
    private int countApple = 0;
    private bool isMissionFinished = false;

    [Header("UI Ayarlarý")]
    [SerializeField] private Image progressBarFill;

    private float minX, maxX;

    void Start()
    {
        //sepetin ekranin sag soluna tasmamasi icin, kamera sinirlari manuel hesaplaniyor
        Vector3 edges = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        float basketWidth = GetComponent<SpriteRenderer>().bounds.extents.x;

        maxX = edges.x - basketWidth;
        minX = basketWidth - edges.x;

        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = 0f;
        }
    }

    public void ObjectDropped(GameObject droppedObject)
    {
        if (isMissionFinished) return;

        if (droppedObject.CompareTag("Apple"))
        {
            countApple++;
            UpdateProgressBar();
            Debug.Log($"Elma sepete koyuldu. Durum{countApple}/{targetAppleNum}");
            Destroy(droppedObject);
        }
        else if(droppedObject.CompareTag("Banana") || droppedObject.CompareTag("Melon")){
            countApple = Mathf.Max(0, countApple - 1);
            UpdateProgressBar();
            Debug.Log($"Yanlýþ meyveyi sepete attýn! Durum:{countApple}/{targetAppleNum}");
            Destroy(droppedObject);
        }
        if (countApple >= targetAppleNum)
        {
            isMissionFinished = true;
            Debug.Log("Görev 1 tamamlandý.");
            GameEvents.OnTaskCompleted?.Invoke(1);
        }
    }

   
    private void UpdateProgressBar()
    {
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = (float)countApple / targetAppleNum;
        }
    }
}
