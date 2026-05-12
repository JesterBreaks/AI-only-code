using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public AbilityManager abilityManager;

    public void ShowUpgradeMenu(int level)
    {
        Time.timeScale = 0f;

        Debug.Log("Choose upgrade");

        if (level == 5)
        {
            abilityManager.unlockRed = true;
        }

        if (level == 8)
        {
            abilityManager.unlockRCT = true;
        }

        if (level == 12)
        {
            abilityManager.unlockPurple = true;
        }

        if (level == 18)
        {
            abilityManager.unlockInfinity = true;
        }

        if (level == 25)
        {
            abilityManager.unlockDomain = true;
        }

        Time.timeScale = 1f;
    }
}