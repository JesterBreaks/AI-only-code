using UnityEngine;

/// <summary>
/// Hollow Purple – A massive erasure beam that destroys everything in its path.
/// </summary>
public class PurpleProjectile : MonoBehaviour
{
    [Header("Stats")]
    public Vector2 direction;
    public float speed = 10f;
    public float damage = 150f;
    public float erasureRadius = 2.5f;
    public float lifetime = 6f;

    [Header("Beam")]
    public float beamLength = 20f;

    [Header("Effects")]
    public GameObject trailEffectPrefab;
    public GameObject erasureEffectPrefab;

    private Rigidbody2D rb;
    private float distanceTraveled;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        distanceTraveled += speed * Time.deltaTime;

        // Continuous wide damage along path
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, erasureRadius, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(damage * Time.deltaTime);
        }

        if (distanceTraveled >= beamLength)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        EnemyHealth eh = other.GetComponent<EnemyHealth>();
        if (eh != null) eh.TakeDamage(damage);

        if (erasureEffectPrefab != null)
            Instantiate(erasureEffectPrefab, other.transform.position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.6f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, erasureRadius);
    }
}