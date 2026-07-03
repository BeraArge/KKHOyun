using UnityEngine;
using UnityEngine.EventSystems;

public class OrderableItem : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Stage2OrderManager manager;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Tıklandı: " + gameObject.name + " | itemName: " + itemName);
        if (manager == null)
        {
            Debug.LogError("Manager atanmadı: " + gameObject.name);
            return;
        }
        manager.HandleClick(itemName, gameObject);
    }
}
