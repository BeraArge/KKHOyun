using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int completedTasks = 0; //biten gorevleri saymak icin degisken
    private readonly int totalTasks = 5; //total gorev sayisi degiskeni
    

    private void OnEnable() //obje aktifken gameEventi dinlemeye baslar
    {
        GameEvents.OnTaskCompleted += HandleTaskCompletion;
    }
    private void OnDisable() //obje deaktive olursa dinlemeyi birak
    {
        GameEvents.OnTaskCompleted -= HandleTaskCompletion;
    }

    private void HandleTaskCompletion(int taskId) 
    {
        completedTasks++;
        
        Debug.Log($"Harika! Gorev {taskId} bitti. Ýlerleme durumu:{completedTasks}/{totalTasks}.");

        if (completedTasks >= totalTasks) 
        {
            Debug.Log("Tebrikler tüm görevleri tamamladýn!!");
            GameEvents.OnGameWon?.Invoke();
            
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;


        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            GameEvents.OnTaskCompleted?.Invoke(1);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            GameEvents.OnTaskCompleted?.Invoke(2);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            GameEvents.OnTaskCompleted?.Invoke(3);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            GameEvents.OnTaskCompleted?.Invoke(4);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            GameEvents.OnTaskCompleted?.Invoke(5);
        }
    }

}
