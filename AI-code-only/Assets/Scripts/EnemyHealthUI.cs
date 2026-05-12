using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public EnemyAI enemy;

    void Update()
    {
        healthSlider.maxValue = enemy.maxHealth;
        healthSlider.value = enemy.currentHealth;
    }
}