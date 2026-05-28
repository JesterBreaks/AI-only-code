using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class RedProjectile : MonoBehaviour
{
    [Header("Stats")]
    public Vector2 direction;
    public float speed = 14f;
    public float damage = 50f;
    public float repulsionForce = 14f;
    public float explosionRadius = 3.5f;
    public float lifetime = 4f;

    public GameObject explosionEffectPrefab;

    private Rigidbody2D rb;
    private bool hasExploded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Ignore player collider so it doesn't explode on spawn
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
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;
        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Projectile")) return;

        Explode();
    }

    void Explode()
    {
        hasExploded = true;

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) continue;
            if (hit.CompareTag("Projectile")) continue;

            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            float falloff = 1f - Mathf.Clamp01(dist / explosionRadius);

            if (eh != null)
                eh.TakeDamage(damage * falloff);

            if (enemyRb != null)
            {
                Vector2 pushDir = (hit.transform.position - transform.position).normalized;
                enemyRb.AddForce(pushDir * repulsionForce * falloff, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    internal void Initialize(Vector2 dir)
    {
        throw new NotImplementedException();
    }
}