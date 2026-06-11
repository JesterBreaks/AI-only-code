using UnityEngine;

public class RedProjectile : MonoBehaviour
{
    public float speed = 16f;
    public float damage = 50f;
    public float hitRadius = 0.3f;
    public float explosionRadius = 3.5f;
    public float repulsionForce = 14f;

    [HideInInspector] public Vector2 direction;

    private float aliveTime;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        aliveTime += Time.deltaTime;

        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (aliveTime < 0.1f) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) continue;
            if (hit.GetComponent<EnemyHealth>() != null)
            {
                Explode();
                return;
            }
        }
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) continue;

            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            float falloff = 1f - Mathf.Clamp01(dist / explosionRadius);

            if (eh != null) eh.TakeDamage(damage * falloff);
            if (enemyRb != null)
            {
                Vector2 pushDir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
                enemyRb.AddForce(pushDir * repulsionForce * falloff, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject);
    }
}