using UnityEngine;

public class PurpleAbility : MonoBehaviour
{
    public float radius = 10f;
    public int damage = 9999;

    void Start()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyAI>().TakeDamage(damage);
            }
        }

        Destroy(gameObject, 2f);
    }
}