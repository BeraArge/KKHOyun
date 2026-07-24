using UnityEngine;
using UnityEngine.UI; //ui elemanlari kullaniyoruz
public class PuzzleManager : MonoBehaviour
{
    [Header("Renkli Yapboz Parçaları(1'den 5'e sırayla ekleyin)")]
    public GameObject[] puzzlePieces;

    private void OnEnable()
    {
        GameEvents.OnTaskCompleted += ShowPuzzlePiece;
    }


    private void OnDisable()
    {
        GameEvents.OnTaskCompleted -= ShowPuzzlePiece;
    }

    private void ShowPuzzlePiece(int taskId) 
    {
        int index = taskId - 1;
        if (index >= 0 && index < puzzlePieces.Length)
        {
            puzzlePieces[index].SetActive(true);
            Debug.Log($"Yapbozun {taskId}. parçası eklenmiştir.");
        }
    }
}
