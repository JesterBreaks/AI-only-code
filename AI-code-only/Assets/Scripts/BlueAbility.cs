using UnityEngine;

public class BlueAbility : MonoBehaviour
{
    public float radius = 5f;
    public int damage = 50;

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

        Destroy(gameObject, 1f);
    }
}