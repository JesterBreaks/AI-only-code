using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 80f;
    public float currentHealth;

    [Header("Events")]
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDeath;

    [Header("Death")]
    public GameObject deathEffectPrefab;
    public float deathDelay = 0.3f;

    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f && !isDead)
            Die();
    }

    void Die()
    {
        isDead = true;
        onDeath?.Invoke();

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject, deathDelay);
    }

    public float GetHealthNormalized() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
}