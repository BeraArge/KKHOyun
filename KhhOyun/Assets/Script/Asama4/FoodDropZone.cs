using UnityEngine;
using UnityEngine.EventSystems;

public class FoodDropZone : MonoBehaviour, IDropHandler
{
    public string acceptedCategory;
    public bool isFilled = false;

    public Stage4PlateManager plateManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (isFilled)
            return;

        FoodDrag food = eventData.pointerDrag.GetComponent<FoodDrag>();

        if (food == null)
            return;

        if (food.foodCategory == acceptedCategory)
        {
            isFilled = true;

            food.SnapTo(GetComponent<RectTransform>());

            if (plateManager != null)
                plateManager.CorrectPlacement();
        }
        else
        {
            if (plateManager != null)
                plateManager.WrongPlacement(food.foodName);
        }
    }
}