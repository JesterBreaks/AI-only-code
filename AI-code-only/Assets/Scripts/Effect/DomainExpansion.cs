using System.Collections;
using UnityEngine;

/// <summary>
/// Domain Expansion: Infinite Void
/// Traps all enemies within range, dealing continuous damage and slowing them.
/// Inside the domain, Gojo's attacks are guaranteed to hit (no miss chance).
/// </summary>
public class DomainExpansion : MonoBehaviour
{
    [HideInInspector] public float radius = 12f;
    [HideInInspector] public float damagePerSecond = 15f;
    [HideInInspector] public float duration = 8f;

    [Header("Visual")]
    public GameObject domainVisualPrefab;  // Black sphere / void effect
    public float slowMultiplier = 0.3f;    // Enemies move at 30% speed inside

    private float elapsed;
    private Collider2D[] hits;

    void Start()
    {
        // Scale visual to match radius
        transform.localScale = Vector3.one * radius * 2f;
        StartCoroutine(DomainRoutine());
    }

    IEnumerator DomainRoutine()
    {
        Debug.Log("Domain Expansion: Infinite Void — Activated!");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            hits = Physics2D.OverlapCircleAll(
                transform.position, radius, LayerMask.GetMask("Enemy"));

            foreach (var hit in hits)
            {
                EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                EnemyAI ai = hit.GetComponent<EnemyAI>();

                // Damage per frame
                if (eh != null)
                    eh.TakeDamage(damagePerSecond * Time.deltaTime);

                // Slow effect
                if (ai != null)
                    ai.SetSpeedMultiplier(slowMultiplier);
            }

            yield return null;
        }

        // Restore enemy speeds
        hits = Physics2D.OverlapCircleAll(
            transform.position, radius, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            EnemyAI ai = hit.GetComponent<EnemyAI>();
            if (ai != null) ai.SetSpeedMultiplier(1f);
        }

        Debug.Log("Domain Expansion: Collapsed.");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}