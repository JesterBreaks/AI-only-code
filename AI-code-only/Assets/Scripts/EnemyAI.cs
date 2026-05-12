using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    public int maxHealth = 50;
    public int currentHealth;
    public int damage = 10;

    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Give XP BEFORE destroying
        PlayerLevelSystem xp = FindObjectOfType<PlayerLevelSystem>();

        if (xp != null)
        {
            xp.GainXP(20);
        }

        Destroy(gameObject);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();

            if (ph != null)
            {
                ph.TakeDamage(damage * Time.deltaTime > 1 ? 1 : 0);
            }
        }
    }
}