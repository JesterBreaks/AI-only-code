using UnityEngine;

/// <summary>
/// Attaches to particles/objects to simulate being pulled toward a point.
/// Used by the Blue ability impact.
/// </summary>
public class SuctionEffect : MonoBehaviour
{
    public Transform attractorPoint;
    public float force = 10f;
    public float duration = 1f;

    private Rigidbody2D rb;
    private float elapsed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (attractorPoint == null || elapsed >= duration)
        {
            Destroy(gameObject);
            return;
        }

        elapsed += Time.fixedDeltaTime;
        Vector2 dir = ((Vector2)attractorPoint.position - rb.position).normalized;
        rb.AddForce(dir * force, ForceMode2D.Force);
    }
}       