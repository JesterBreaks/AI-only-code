using UnityEngine;

public class PurpleProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 150f;
    public float hitRadius = 0.8f;

    [HideInInspector] public Vector2 direction;

    private float aliveTime;

    void Start()
    {
        Destroy(gameObject, 5f);
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
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                // Keep going, do NOT destroy on hit
            }
        }
    }
}