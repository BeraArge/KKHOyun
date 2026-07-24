using UnityEngine;
using UnityEngine.UI;

public class HeartDisplayUI : MonoBehaviour
{
    [Header("Kalp İkonları")]
    public Image[] hearts;
    public Sprite heartFilled;
    public Sprite heartEmpty;

    [Header("Puan Eşlemesi")]
    public int scorePerHeart = 20;

    public void UpdateHearts(int score)
    {
        int filledCount = Mathf.Clamp(score / scorePerHeart, 0, hearts.Length);

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].sprite = i < filledCount ? heartFilled : heartEmpty;
        }
    }
}
