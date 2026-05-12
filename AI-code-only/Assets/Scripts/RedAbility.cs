using UnityEngine;

public class RedAbility : MonoBehaviour
{
    public float radius = 6f;
    public int damage = 80;

    void Start()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyAI>().TakeDamage(damage);

                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    Vector2 dir = (enemy.transform.position - transform.position).normalized;
                    rb.AddForce(dir * 500f);
                }
            }
        }

        Destroy(gameObject, 1f);
    }
}