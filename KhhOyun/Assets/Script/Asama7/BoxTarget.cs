using UnityEngine;

public class BoxTarget : MonoBehaviour
{
    public string boxType;
    public BreakfastControl manager;
    public void ObjectDropped(GameObject droppedObject)
    {
        manager.ObjectDropped(droppedObject, boxType);
    }

}
