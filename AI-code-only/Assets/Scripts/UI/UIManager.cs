using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player References")]
    public HealthSystem playerHealth;
    public CursedEnergySystem cursedEnergy;
    public GojoAbilities abilities;
    public InfinityShield infinity;
    public PlayerController playerController;

    [Header("Health Bar")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Color healthHighColor = new Color(0.2f, 0.9f, 0.3f);
    public Color healthLowColor = new Color(0.9f, 0.2f, 0.2f);

    [Header("Cursed Energy Bar")]
    public Image ceBarFill;
    public TextMeshProUGUI ceText;
    public Color ceHighColor = new Color(0.1f, 0.6f, 1f);
    public Color ceLowColor = new Color(0.5f, 0.0f, 0.9f);

    [Header("Infinity Indicator")]
    public Image infinityIndicator;
    public Color infinityActiveColor = new Color(0f, 0.8f, 1f, 0.9f);
    public Color infinityInactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public TextMeshProUGUI infinityText;

    [Header("Ability Cooldown Icons")]
    public Image strikeCDImage;
    public Image blueCDImage;
    public Image redCDImage;
    public Image purpleCDImage;
    public Image domainCDImage;
    public Image dashCDImage;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    void Awake()
    {
        // FORCE correct fill settings (prevents inspector mistakes)

        if (healthBarFill != null)
        {
            healthBarFill.type = Image.Type.Filled;
            healthBarFill.fillMethod = Image.FillMethod.Horizontal;
            healthBarFill.fillOrigin = (int)Image.OriginHorizontal.Right;
        }

        if (ceBarFill != null)
        {
            ceBarFill.type = Image.Type.Filled;
            ceBarFill.fillMethod = Image.FillMethod.Horizontal;
            ceBarFill.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
    }

    void Start()
    {
        playerHealth.onHealthChanged.AddListener(UpdateHealthBar);
        cursedEnergy.onCEChanged.AddListener(UpdateCEBar);
        playerHealth.onDeath.AddListener(ShowGameOver);

        UpdateHealthBar(playerHealth.currentHealth, playerHealth.maxHealth);
        UpdateCEBar(cursedEnergy.currentCE, cursedEnergy.maxCE);
    }

    void Update()
    {
        UpdateCooldownIcons();
        UpdateInfinityUI();
    }

    // ─── HEALTH BAR ───────────────────────────────────────────────────────────
    void UpdateHealthBar(float current, float max)
    {
        float normalized = current / max;

        healthBarFill.fillAmount = normalized;
        healthBarFill.color = Color.Lerp(healthLowColor, healthHighColor, normalized);

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    // ─── CURSED ENERGY BAR ────────────────────────────────────────────────────
    void UpdateCEBar(float current, float max)
    {
        float normalized = current / max;

        ceBarFill.fillAmount = normalized;
        ceBarFill.color = Color.Lerp(ceLowColor, ceHighColor, normalized);

        if (ceText != null)
            ceText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    // ─── COOLDOWN ICONS ───────────────────────────────────────────────────────
    void UpdateCooldownIcons()
    {
        if (abilities == null) return;

        SetCooldownIcon(strikeCDImage, abilities.GetStrikeCDNorm());
        SetCooldownIcon(blueCDImage, abilities.GetBlueCDNorm());
        SetCooldownIcon(redCDImage, abilities.GetRedCDNorm());
        SetCooldownIcon(purpleCDImage, abilities.GetPurpleCDNorm());
        SetCooldownIcon(domainCDImage, abilities.GetDomainCDNorm());

        if (playerController != null)
            SetCooldownIcon(dashCDImage, 1f - playerController.GetDashCooldownNormalized());
    }

    void SetCooldownIcon(Image icon, float cdNormalized)
    {
        if (icon == null) return;

        icon.fillAmount = cdNormalized;

        Color c = icon.color;
        c.a = Mathf.Lerp(0.15f, 0.85f, cdNormalized);
        icon.color = c;
    }

    // ─── INFINITY UI ──────────────────────────────────────────────────────────
    void UpdateInfinityUI()
    {
        if (infinity == null) return;

        if (infinityIndicator != null)
            infinityIndicator.color = infinity.IsActive()
                ? infinityActiveColor
                : infinityInactiveColor;

        if (infinityText != null)
            infinityText.text = infinity.IsActive() ? "∞  ON" : "∞  OFF";
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}