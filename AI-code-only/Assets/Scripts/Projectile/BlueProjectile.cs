using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class BlueProjectile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 12f;
    public float damage = 40f;
    public float attractionForce = 12f;
    public float attractionRadius = 4f;
    public float attractionDuration = 1.2f;
    public float lifetime = 4f;

    public GameObject impactEffectPrefab;

    private Rigidbody2D rb;
    private bool hasImpacted;
    private Vector2 direction;

    private bool initialized; // ✅ ADDED

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D myCol = GetComponent<Collider2D>();
            Collider2D playerCol = player.GetComponent<Collider2D>();

            if (myCol != null && playerCol != null)
                Physics2D.IgnoreCollision(myCol, playerCol);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime);

        // ✅ ADDED SAFETY FALLBACK (prevents "no movement" bug)
        if (!initialized)
        {
            direction = transform.right; // fallback direction
            rb.linearVelocity = direction * speed;
        }
    }

    public void Initialize(Vector2 shootDirection)
    {
        initialized = true; // ✅ ADDED

        direction = shootDirection.normalized;

        rb.linearVelocity = direction * speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasImpacted) return;
        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Projectile")) return;

        hasImpacted = true;

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(AttractionPulse());
    }

    IEnumerator AttractionPulse()
    {
        if (impactEffectPrefab != null)
            Instantiate(impactEffectPrefab, transform.position, Quaternion.identity);

        float elapsed = 0f;
        bool dealtInitialDamage = false;

        while (elapsed < attractionDuration)
        {
            elapsed += Time.deltaTime;

            Collider2D[] hits =
                Physics2D.OverlapCircleAll(transform.position, attractionRadius);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player")) continue;
                if (hit.CompareTag("Projectile")) continue;

                EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    Vector2 pullDir =
                        ((Vector2)transform.position -
                        (Vector2)hit.transform.position).normalized;

                    enemyRb.AddForce(pullDir * attractionForce, ForceMode2D.Force);
                }

                if (!dealtInitialDamage && eh != null)
                    eh.TakeDamage(damage);
            }

            dealtInitialDamage = true;
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}