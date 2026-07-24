using UnityEngine;
using System; //action kullanabilmek icin

public class GameEvents
{
    public static Action<int> OnTaskCompleted; //hangi gorevin bittigini haber verir


    public static Action OnGameWon; //oyunun bittigini haber verir
}
