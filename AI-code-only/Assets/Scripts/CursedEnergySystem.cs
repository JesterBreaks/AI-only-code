using UnityEngine;
using UnityEngine.UI;

public class CursedEnergySystem : MonoBehaviour
{
    public float maxEnergy = 200f;
    public float currentEnergy;

    public float regenRate = 10f;

    public Image energyBarFill;

    void Start()
    {
        currentEnergy = maxEnergy;
        UpdateEnergyBar();
    }

    void Update()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += regenRate * Time.deltaTime;

            if (currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;
        }

        UpdateEnergyBar();
    }

    public void UseEnergy(float amount)
    {
        currentEnergy -= amount;

        if (currentEnergy < 0)
            currentEnergy = 0;

        UpdateEnergyBar();
    }

    void UpdateEnergyBar()
    {
        energyBarFill.fillAmount = currentEnergy / maxEnergy;
    }
}