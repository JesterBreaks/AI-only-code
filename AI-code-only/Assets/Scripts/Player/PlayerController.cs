using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;

    [Header("References")]
    public GojoAbilities abilities;
    public CursedEnergySystem cursedEnergy;
    public HealthSystem health;
    public InfinityShield infinity;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 dashDirection;
    private bool isDashing;
    private float dashTimer;

    // Dash cooldown
    private float dashCooldown = 1f;
    private float dashCooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing) return;

        HandleMovementInput();
        HandleFacing();
        HandleAbilityInput();
        HandleDashInput();

        dashCooldownTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;
            }

            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    void HandleMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(x, y).normalized;
    }

    void HandleFacing()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void HandleAbilityInput()
    {
        if (Input.GetMouseButtonDown(0))
            abilities.CursedStrike();

        if (Input.GetKeyDown(KeyCode.E))
            abilities.CastBlue();

        if (Input.GetKeyDown(KeyCode.R))
            abilities.CastRed();

        if (Input.GetKeyDown(KeyCode.T))
            abilities.CastHollowPurple();

        if (Input.GetKeyDown(KeyCode.F))
            abilities.ActivateDomainExpansion();

        if (Input.GetKeyDown(KeyCode.Q))
            infinity.ToggleInfinity();
    }

    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
        {
            if (cursedEnergy.TrySpend(10f))
            {
                dashDirection = moveInput == Vector2.zero
                    ? (Vector2)transform.up
                    : moveInput;

                isDashing = true;
                dashTimer = dashDuration;
                dashCooldownTimer = dashCooldown;
            }
        }
    }

    public float GetDashCooldownNormalized()
        => Mathf.Clamp01(1f - (dashCooldownTimer / dashCooldown));
}