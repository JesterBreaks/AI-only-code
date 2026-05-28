using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 200f;
    public float currentHealth;

    [Header("Events")]
    public UnityEvent<float, float> onHealthChanged;  // current, max
    public UnityEvent onDeath;
    public UnityEvent<float> onDamaged;               // damage amount

    [Header("Invincibility Frames")]
    public float invincibilityDuration = 0.5f;
    private float invincibilityTimer;

    private InfinityShield infinityShield;
    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        infinityShield = GetComponent<InfinityShield>();
    }

    void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public void TakeDamage(float rawDamage)
    {
        if (isDead) return;
        if (invincibilityTimer > 0f) return;

        // Apply Infinity filter
        float damage = rawDamage;
        if (infinityShield != null)
            damage = infinityShield.FilterDamage(rawDamage);

        currentHealth = Mathf.Max(0, currentHealth - damage);

        invincibilityTimer = invincibilityDuration;
        onDamaged?.Invoke(damage);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f && !isDead)
            Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    void Die()
    {
        isDead = true;
        onDeath?.Invoke();
        // Play death animation, show game over screen, etc.
        Debug.Log($"{gameObject.name} has died.");
    }

    public float GetHealthNormalized() => currentHealth / maxHealth;
    public bool IsDead() => isDead;
}