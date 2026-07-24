using UnityEngine;
using UnityEngine.EventSystems;

public class Asama3ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Asama3StageManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (manager == null)
        {
            Debug.LogError("Manager atanmadı: " + gameObject.name);
            return;
        }

        manager.HandleClick(itemName, gameObject);
    }
}
