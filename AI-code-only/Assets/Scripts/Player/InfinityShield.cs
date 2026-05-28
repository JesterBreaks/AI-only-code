using UnityEngine;

/// <summary>
/// Gojo's passive Infinity barrier.
/// When active, reduces all incoming damage by a percentage and drains CE.
/// </summary>
public class InfinityShield : MonoBehaviour
{
    [Header("Infinity Settings")]
    public bool infinityActive = true;
    public float damageReduction = 0.8f;   // 80% damage blocked
    public float ceDrainPerSecond = 2f;    // CE cost while active
    public float ceDrainOnHit = 5f;       // Extra CE cost per hit blocked

    [Header("Visual")]
    public GameObject infinityVisualEffect; // Particle / glow effect around player

    private CursedEnergySystem cursedEnergy;

    void Awake()
    {
        cursedEnergy = GetComponent<CursedEnergySystem>();
        UpdateVisual();
    }

    void Update()
    {
        if (infinityActive)
        {
            // Passive CE drain while Infinity is maintained
            bool hasCE = cursedEnergy.TrySpend(ceDrainPerSecond * Time.deltaTime);
            if (!hasCE)
            {
                // Not enough CE to maintain – auto disable
                infinityActive = false;
                UpdateVisual();
            }
        }
    }

    public void ToggleInfinity()
    {
        infinityActive = !infinityActive;
        UpdateVisual();
        Debug.Log($"Infinity: {(infinityActive ? "ON" : "OFF")}");
    }

    /// <summary>
    /// Call this from HealthSystem before applying damage.
    /// Returns the actual damage after Infinity mitigation.
    /// </summary>
    public float FilterDamage(float rawDamage)
    {
        if (!infinityActive) return rawDamage;

        // Drain CE per hit
        if (!cursedEnergy.TrySpend(ceDrainOnHit))
        {
            // Can't pay CE for this block – Infinity breaks
            infinityActive = false;
            UpdateVisual();
            return rawDamage;
        }

        float blocked = rawDamage * damageReduction;
        float remaining = rawDamage - blocked;
        return remaining;
    }

    void UpdateVisual()
    {
        if (infinityVisualEffect != null)
            infinityVisualEffect.SetActive(infinityActive);
    }

    public bool IsActive() => infinityActive;
}