using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableItem : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public Stage1RoomManager roomManager;

    [Header("Animation Image")]
    public Image flyImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Týklandý: " + itemName);

        if (roomManager == null)
        {
            Debug.LogError("RoomManager atanmadý: " + gameObject.name);
            return;
        }

        if (flyImage == null)
        {
            Debug.LogWarning("Fly Image atanmadý, bu obje uçmadan seçilecek: " + gameObject.name);
        }

        roomManager.SelectItem(itemName, flyImage);
    }
}