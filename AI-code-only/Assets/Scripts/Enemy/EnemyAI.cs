using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Stunned }

    [Header("Stats")]
    public float baseSpeed = 2.5f;
    public float attackDamage = 15f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.2f;
    // Removed chaseRange since it's no longer needed for distance checks

    private float speedMultiplier = 1f;
    private float attackTimer;
    private EnemyState state;
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;
        attackTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // State Determination: Always chase if outside attack range
        if (distToPlayer <= attackRange)
        {
            state = EnemyState.Attack;
        }
        else
        {
            state = EnemyState.Chase; // Unlimited vision! Always chases if not attacking.
        }

        switch (state)
        {
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
            case EnemyState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void ChasePlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * (baseSpeed * speedMultiplier);

        if (anim != null) anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;

            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if (playerHealth != null)
                playerHealth.TakeDamage(attackDamage);

            if (anim != null) anim.SetTrigger("Attack");
        }
    }

    public void SetSpeedMultiplier(float mult)
    {
        speedMultiplier = mult;
    }
}