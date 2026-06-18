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
    public float healthLerpSpeed = 5f;

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

    [Header("Cooldown Texts")]
    public TextMeshProUGUI strikeCDText;
    public TextMeshProUGUI blueCDText;
    public TextMeshProUGUI redCDText;
    public TextMeshProUGUI purpleCDText;
    public TextMeshProUGUI domainCDText;
    public TextMeshProUGUI dashCDText;

    [Header("Cooldown Text Colors")]
    public Color readyColor = new Color(1f, 0.9f, 0.2f); // Gold
    public Color cooldownColor = Color.white;

    [Header("Text Outline")]
    public Color outlineColor = Color.black;
    [Range(0f, 1f)] public float outlineWidth = 0.5f;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private float targetHealthFill;
    private float targetCEFill;

    void Start()
    {
        playerHealth.onHealthChanged.AddListener(OnHealthChanged);
        cursedEnergy.onCEChanged.AddListener(OnCEChanged);
        playerHealth.onDeath.AddListener(ShowGameOver);

        targetHealthFill = 1f;
        targetCEFill = 1f;

        healthBarFill.fillAmount = 1f;
        ceBarFill.fillAmount = 1f;

        ApplyOutline(healthText);
        ApplyOutline(ceText);
        ApplyOutline(infinityText);
        ApplyOutline(strikeCDText);
        ApplyOutline(blueCDText);
        ApplyOutline(redCDText);
        ApplyOutline(purpleCDText);
        ApplyOutline(domainCDText);
        ApplyOutline(dashCDText);
    }

    void ApplyOutline(TextMeshProUGUI label)
    {
        if (label == null) return;

        Material mat = new Material(label.fontMaterial);
        mat.SetColor("_OutlineColor", outlineColor);
        mat.SetFloat("_OutlineWidth", outlineWidth);
        label.fontMaterial = mat;
    }

    void Update()
    {
        healthBarFill.fillAmount = Mathf.Lerp(
            healthBarFill.fillAmount,
            targetHealthFill,
            Time.deltaTime * healthLerpSpeed
        );

        healthBarFill.color = Color.Lerp(
            healthLowColor,
            healthHighColor,
            healthBarFill.fillAmount
        );

        ceBarFill.fillAmount = targetCEFill;

        ceBarFill.color = Color.Lerp(
            ceLowColor,
            ceHighColor,
            targetCEFill
        );

        UpdateCooldownTexts();
        UpdateInfinityUI();
    }

    void OnHealthChanged(float current, float max)
    {
        targetHealthFill = current / max;

        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    void OnCEChanged(float current, float max)
    {
        targetCEFill = current / max;

        if (ceText != null)
            ceText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    void UpdateCooldownTexts()
    {
        if (abilities == null) return;

        SetCooldownText(
            strikeCDText,
            "Cursed Strike",
            abilities.GetStrikeTimer(),
            abilities.strikeCECost
        );

        SetCooldownText(
            blueCDText,
            "Lapse Blue",
            abilities.GetBlueTimer(),
            abilities.blueCECost
        );

        SetCooldownText(
            redCDText,
            "Reversal Red",
            abilities.GetRedTimer(),
            abilities.redCECost
        );

        SetCooldownText(
            purpleCDText,
            "Hollow Purple",
            abilities.GetPurpleTimer(),
            abilities.purpleCECost
        );

        SetCooldownText(
            domainCDText,
            "Domain Expansion",
            abilities.GetDomainTimer(),
            abilities.domainCECost
        );

        if (dashCDText != null && playerController != null)
        {
            float dashNorm = 1f - playerController.GetDashCooldownNormalized();

            string status = dashNorm <= 0f
                ? "Ready"
                : "...";

            dashCDText.text = $"Dash: {status} (0 CE)";
            dashCDText.color = dashNorm <= 0f
                ? readyColor
                : cooldownColor;
        }
    }

    void SetCooldownText(
        TextMeshProUGUI label,
        string abilityName,
        float timeRemaining,
        float ceCost)
    {
        if (label == null) return;

        if (timeRemaining <= 0f)
        {
            label.text = $"{abilityName}: Ready ({ceCost} CE)";
            label.color = readyColor;
        }
        else
        {
            label.text = $"{abilityName}: {timeRemaining:F1}s ({ceCost} CE)";
            label.color = cooldownColor;
        }
    }

    void UpdateInfinityUI()
    {
        if (infinity == null) return;

        if (infinityIndicator != null)
        {
            infinityIndicator.color = infinity.IsActive()
                ? infinityActiveColor
                : infinityInactiveColor;
        }

        if (infinityText != null)
        {
            infinityText.text = infinity.IsActive()
                ? "∞ ON"
                : "∞ OFF";
        }
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}