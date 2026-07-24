using UnityEngine;
using UnityEngine.InputSystem;
public class PatientControl : MonoBehaviour
{
    [Header("Görev Ayarlarý")]
    [SerializeField] private string drinkableMedTag = "Drinkable";
    [SerializeField] private GameObject task3;
    [SerializeField] private GameObject task4;
    private bool isMissionFinished = false;
    private int medCount = 0;
    

    public void ObjectDropped(GameObject droppedObject)
    {
        if (isMissionFinished) return;

        if (droppedObject.CompareTag(drinkableMedTag))
        {
            Debug.Log($"Doðru ilacý verdin!");
            Destroy(droppedObject);
            medCount++;
            if (medCount == 3)
            {
                GameEvents.OnTaskCompleted?.Invoke(3);
                task3.SetActive(false);
                task4.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Yanlýþ ilaç");
            droppedObject.GetComponent<DraggableObject>().ResetPosition();
        }
    }
    
   
}
