using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodClickableItem : MonoBehaviour, IPointerClickHandler
{
    public string foodName;
    public Stage4FarmManager farmManager;

    [Header("Animation Image")]
    public Image flyImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (farmManager == null)
        {
            Debug.LogError("FarmManager atanmadý: " + gameObject.name);
            return;
        }

        farmManager.SelectFood(foodName, flyImage);
    }
}