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

    [Header("Spawn Points")]
    public Transform projectileSpawnPoint;

    [Header("Cursed Strike")]
    public float strikeRange = 1.8f;
    public float strikeDamage = 25f;
    public float strikeCECost = 5f;
    public float strikeCooldown = 0.4f;

    [Header("Blue - Lapse")]
    public float blueCECost = 20f;
    public float blueCooldown = 3f;
    public float blueForce = 12f;

    [Header("Red - Reversal")]
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
    }

    void Update()
    {
        strikeTimer -= Time.deltaTime;
        blueTimer -= Time.deltaTime;
        redTimer -= Time.deltaTime;
        purpleTimer -= Time.deltaTime;
        domainTimer -= Time.deltaTime;
    }

    // ─── CURSED STRIKE ────────────────────────────────────────────────────────
    public void CursedStrike()
    {
        if (strikeTimer > 0f || !cursedEnergy.TrySpend(strikeCECost)) return;
        strikeTimer = strikeCooldown;

        if (cursedStrikeEffect != null)
            Instantiate(cursedStrikeEffect, projectileSpawnPoint.position, transform.rotation);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            projectileSpawnPoint.position,
            strikeRange,
            LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(strikeDamage);
        }
    }

    // ─── BLUE ────────────────────────────────────────────────────────────────
    public void CastBlue()
    {
        if (blueTimer > 0f || !cursedEnergy.TrySpend(blueCECost)) return;
        blueTimer = blueCooldown;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - projectileSpawnPoint.position).normalized;

        GameObject proj = Instantiate(
            blueProjectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        BlueProjectile bp = proj.GetComponent<BlueProjectile>();

        if (bp != null)
        {
            bp.Initialize(dir);
            bp.attractionForce = blueForce;
        }
    }

    // ─── RED ────────────────────────────────────────────────────────────────
    public void CastRed()
    {
        if (redTimer > 0f || !cursedEnergy.TrySpend(redCECost)) return;
        redTimer = redCooldown;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - projectileSpawnPoint.position).normalized;

        GameObject proj = Instantiate(
            redProjectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        RedProjectile rp = proj.GetComponent<RedProjectile>();

        if (rp != null)
        {
            rp.Initialize(dir);
            rp.repulsionForce = redForce;
        }
    }

    // ─── HOLLOW PURPLE ───────────────────────────────────────────────────────
    public void CastHollowPurple()
    {
        if (purpleTimer > 0f || !cursedEnergy.TrySpend(purpleCECost)) return;
        purpleTimer = purpleCooldown;

        StartCoroutine(HollowPurpleSequence());
    }

    IEnumerator HollowPurpleSequence()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 dir = (mousePos - projectileSpawnPoint.position).normalized;

        GameObject proj = Instantiate(
            purpleProjectilePrefab,
            projectileSpawnPoint.position,
            Quaternion.identity
        );

        PurpleProjectile pp = proj.GetComponent<PurpleProjectile>();
        if (pp != null)
            pp.direction = dir;
    }

    // ─── DOMAIN EXPANSION ────────────────────────────────────────────────────
    public void ActivateDomainExpansion()
    {
        if (domainTimer > 0f || !cursedEnergy.TrySpend(domainCECost)) return;
        domainTimer = domainCooldown;

        StartCoroutine(DomainExpansionSequence());
    }

    IEnumerator DomainExpansionSequence()
    {
        if (infiniteVoidOverlay != null)
            infiniteVoidOverlay.SetActive(true);

        GameObject domain = Instantiate(domainExpansionPrefab, transform.position, Quaternion.identity);

        DomainExpansion de = domain.GetComponent<DomainExpansion>();
        if (de != null)
        {
            de.radius = domainRadius;
            de.damagePerSecond = domainDamagePerSecond;
            de.duration = domainDuration;
        }

        yield return new WaitForSeconds(domainDuration);

        if (infiniteVoidOverlay != null)
            infiniteVoidOverlay.SetActive(false);
    }

    // ─── COOLDOWNS ───────────────────────────────────────────────────────────
    public float GetStrikeCDNorm() => Mathf.Clamp01(strikeTimer / strikeCooldown);
    public float GetBlueCDNorm() => Mathf.Clamp01(blueTimer / blueCooldown);
    public float GetRedCDNorm() => Mathf.Clamp01(redTimer / redCooldown);
    public float GetPurpleCDNorm() => Mathf.Clamp01(purpleTimer / purpleCooldown);
    public float GetDomainCDNorm() => Mathf.Clamp01(domainTimer / domainCooldown);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (projectileSpawnPoint != null)
            Gizmos.DrawWireSphere(projectileSpawnPoint.position, strikeRange);
    }
}