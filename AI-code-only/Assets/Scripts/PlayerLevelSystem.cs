using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelSystem : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public Image xpBarFill;

    public UpgradeManager upgradeManager;

    void Update()
    {
        UpdateXPBar();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPBar();
    }

    void LevelUp()
    {
        level++;

        currentXP = 0;
        xpToNextLevel += 50;

        Debug.Log("Level Up!");

        upgradeManager.ShowUpgradeMenu(level);

        UpdateXPBar();
    }

    void UpdateXPBar()
    {
        xpBarFill.fillAmount = (float)currentXP / xpToNextLevel;
    }
}