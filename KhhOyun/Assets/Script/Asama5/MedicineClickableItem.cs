using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MedicineClickableItem : MonoBehaviour, IPointerClickHandler
{
    public string medicineName;
    public Stage5MedicineManager medicineManager;

    [Header("Animation Image")]
    public Image flyImage;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (medicineManager == null)
        {
            Debug.LogError("MedicineManager atanmadý: " + gameObject.name);
            return;
        }

        medicineManager.SelectMedicine(medicineName, flyImage);
    }
}