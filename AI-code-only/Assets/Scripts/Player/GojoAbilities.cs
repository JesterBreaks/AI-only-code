using System.Collections;
using UnityEngine;

public class GojoAbilities : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject blueProjectilePrefab;
    public GameObject redProjectilePrefab;
    public GameObject purpleProjectilePrefab;
    public GameObject cursedStrikeEffect;
    public GameObject domainExpansionPrefab;
    public GameObject infiniteVoidOverlay;

    [Header("Spawn Point")]
    public Transform projectileSpawnPoint;

    [Header("Cursed Strike")]
    public float strikeRange = 1.8f;
    public float strikeDamage = 25f;
    public float strikeCECost = 5f;
    public float strikeCooldown = 0.4f;

    [Header("Lapse Blue")]
    public float blueCECost = 20f;
    public float blueCooldown = 3f;

    [Header("Reversal Red")]
    public float redCECost = 20f;
    public float redCooldown = 3f;
    public float redForce = 14f;

    [Header("Hollow Purple")]
    public float purpleCECost = 60f;
    public float purpleCooldown = 10f;

    [Header("Domain Expansion")]
    public float domainCECost = 100f;
    public float domainCooldown = 30f;
    public float domainRadius = 12f;
    public float domainDuration = 8f;
    public float domainDamagePerSecond = 15f;

    private float strikeTimer;
    private float blueTimer;
    private float redTimer;
    private float purpleTimer;
    private float domainTimer;

    private CursedEnergySystem cursedEnergy;

    void Awake()
    {
        cursedEnergy = GetComponent<CursedEnergySystem>();
        if (projectileSpawnPoint == null)
            projectileSpawnPoint = transform;
    }

    void Update()
    {
        strikeTimer -= Time.deltaTime;
        blueTimer -= Time.deltaTime;
        redTimer -= Time.deltaTime;
        purpleTimer -= Time.deltaTime;
        domainTimer -= Time.deltaTime;
    }

    Vector2 FacingDirection()
    {
        return projectileSpawnPoint.up;
    }

    // ── CURSED STRIKE ─────────────────────────────────────────────────────────
    public void CursedStrike()
    {
        if (strikeTimer > 0f) return;
        if (cursedEnergy != null && !cursedEnergy.TrySpend(strikeCECost)) return;
        strikeTimer = strikeCooldown;

        if (cursedStrikeEffect != null)
            Instantiate(cursedStrikeEffect, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        Collider2D[] hits = Physics2D.OverlapCircleAll(projectileSpawnPoint.position, strikeRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) continue;
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(strikeDamage);
        }
    }

    // ── LAPSE BLUE (COOLDOWN REMOVED) ─────────────────────────────────────────
    public void CastBlue()
    {
        if (cursedEnergy != null && !cursedEnergy.TrySpend(blueCECost)) return;

        if (blueProjectilePrefab == null) { Debug.LogError("Blue prefab missing!"); return; }

        GameObject proj = Instantiate(blueProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        BlueProjectile bp = proj.GetComponent<BlueProjectile>();
        if (bp != null) bp.direction = FacingDirection();
    }

    // ── REVERSAL RED ──────────────────────────────────────────────────────────
    public void CastRed()
    {
        if (redTimer > 0f) return;
        if (cursedEnergy != null && !cursedEnergy.TrySpend(redCECost)) return;
        redTimer = redCooldown;

        if (redProjectilePrefab == null) { Debug.LogError("Red prefab missing!"); return; }

        GameObject proj = Instantiate(redProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        RedProjectile rp = proj.GetComponent<RedProjectile>();
        if (rp != null)
        {
            rp.direction = FacingDirection();
            rp.repulsionForce = redForce;
        }
    }

    // ── HOLLOW PURPLE ─────────────────────────────────────────────────────────
    public void CastHollowPurple()
    {
        if (purpleTimer > 0f) return;
        if (cursedEnergy != null && !cursedEnergy.TrySpend(purpleCECost)) return;
        purpleTimer = purpleCooldown;

        StartCoroutine(HollowPurpleSequence());
    }

    IEnumerator HollowPurpleSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (purpleProjectilePrefab == null) yield break;

        GameObject proj = Instantiate(purpleProjectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
        PurpleProjectile pp = proj.GetComponent<PurpleProjectile>();
        if (pp != null) pp.direction = FacingDirection();
    }

    // ── DOMAIN EXPANSION ──────────────────────────────────────────────────────
    public void ActivateDomainExpansion()
    {
        if (domainTimer > 0f) return;
        if (cursedEnergy != null && !cursedEnergy.TrySpend(domainCECost)) return;
        domainTimer = domainCooldown;

        StartCoroutine(DomainExpansionSequence());
    }

    IEnumerator DomainExpansionSequence()
    {
        if (infiniteVoidOverlay != null)
            infiniteVoidOverlay.SetActive(true);

        if (domainExpansionPrefab != null)
        {
            GameObject domain = Instantiate(domainExpansionPrefab, transform.position, Quaternion.identity);
            DomainExpansion de = domain.GetComponent<DomainExpansion>();
            if (de != null)
            {
                de.radius = domainRadius;
                de.damagePerSecond = domainDamagePerSecond;
                de.duration = domainDuration;
            }
        }

        yield return new WaitForSeconds(domainDuration);

        if (infiniteVoidOverlay != null)
            infiniteVoidOverlay.SetActive(false);
    }

    public float GetStrikeTimer() => Mathf.Max(0f, strikeTimer);
    public float GetBlueTimer() => Mathf.Max(0f, blueTimer);
    public float GetRedTimer() => Mathf.Max(0f, redTimer);
    public float GetPurpleTimer() => Mathf.Max(0f, purpleTimer);
    public float GetDomainTimer() => Mathf.Max(0f, domainTimer);

    public float GetStrikeCDNorm() => Mathf.Clamp01(strikeTimer / strikeCooldown);
    public float GetBlueCDNorm() => Mathf.Clamp01(blueTimer / blueCooldown);
    public float GetRedCDNorm() => Mathf.Clamp01(redTimer / redCooldown);
    public float GetPurpleCDNorm() => Mathf.Clamp01(purpleTimer / purpleCooldown);
    public float GetDomainCDNorm() => Mathf.Clamp01(domainTimer / domainCooldown);

    void OnDrawGizmosSelected()
    {
        if (projectileSpawnPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(projectileSpawnPoint.position, strikeRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(projectileSpawnPoint.position,
            projectileSpawnPoint.position + projectileSpawnPoint.up * 2f);
    }
}