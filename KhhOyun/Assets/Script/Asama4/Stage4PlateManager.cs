using UnityEngine;

public class Stage4PlateManager : MonoBehaviour
{
    public WarningPopupUI warningPopup;

    public int requiredPlacements = 7;
    public int currentPlacements = 0;

    public void CorrectPlacement()
    {
        currentPlacements++;

        if (currentPlacements >= requiredPlacements)
        {
            warningPopup.Show("Harika!\nDengeli tabaðýn hazýr.");
        }
    }

    public void WrongPlacement(string foodName)
    {
        warningPopup.Show("Bu besin bu bölüme ait deðil.\nBaþka bir bölümü dene.");
    }
}