using UnityEngine;
using UnityEngine.InputSystem;
public class BreakfastControl : MonoBehaviour
{
    [Header("Görev Ayarlarý")]
    [SerializeField] private string eatableTag = "Eatable";
    [SerializeField] private string trashTag = "Trash";
    [SerializeField] private GameObject task4;
    [SerializeField] private GameObject task5;
    private bool isMissionFinished = false;
    private int foodBagCount = 0;
    private int trashBinCount = 0;

    public void ObjectDropped(GameObject droppedObject,string boxType)
    {
        if (isMissionFinished) return;
        if (droppedObject.CompareTag(eatableTag)&&boxType=="FoodBag")
        {
            foodBagCount++;
            Destroy(droppedObject);
            
        }
        else if(droppedObject.CompareTag(trashTag)&&boxType=="TrashBin")
        {
            trashBinCount++;
            Destroy(droppedObject);
        }
        else
        {
            droppedObject.GetComponent<DraggableObject>().ResetPosition();
        }
        if (foodBagCount == 3 && trashBinCount == 2)
        {
            isMissionFinished = true;
            GameEvents.OnTaskCompleted?.Invoke(4);
            task4.SetActive(false);
            task5.SetActive(true);
        }
    }
}
