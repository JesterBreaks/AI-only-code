using UnityEngine;
using UnityEngine.Events;

public class CursedEnergySystem : MonoBehaviour
{
    [Header("Cursed Energy")]
    public float maxCE = 200f;
    public float currentCE;
    public float regenRate = 8f;        // CE per second when not casting
    public float regenDelay = 2f;       // Seconds after last spend before regen starts

    [Header("Events")]
    public UnityEvent<float, float> onCEChanged;  // current, max

    private float regenDelayTimer;

    void Awake()
    {
        currentCE = maxCE;
    }

    void Update()
    {
        regenDelayTimer -= Time.deltaTime;

        if (regenDelayTimer <= 0f && currentCE < maxCE)
        {
            currentCE = Mathf.Min(maxCE, currentCE + regenRate * Time.deltaTime);
            onCEChanged?.Invoke(currentCE, maxCE);
        }
    }

    /// <summary>
    /// Attempts to spend the given amount of CE.
    /// Returns true if successful, false if not enough CE.
    /// </summary>
    public bool TrySpend(float amount)
    {
        if (currentCE < amount) return false;

        currentCE -= amount;
        regenDelayTimer = regenDelay;
        onCEChanged?.Invoke(currentCE, maxCE);
        return true;
    }

    public void Restore(float amount)
    {
        currentCE = Mathf.Min(maxCE, currentCE + amount);
        onCEChanged?.Invoke(currentCE, maxCE);
    }

    public float GetCENormalized() => currentCE / maxCE;
    public bool HasEnough(float amount) => currentCE >= amount;
}