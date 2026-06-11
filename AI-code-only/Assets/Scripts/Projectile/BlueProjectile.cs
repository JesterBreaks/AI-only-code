using UnityEngine;

public class BlueProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 40f;
    public float hitRadius = 0.3f;

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
            EnemyHealth eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
            {
                eh.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
    }
}