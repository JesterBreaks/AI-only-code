using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AbilityManager : MonoBehaviour
{
    [Header("Abilities")]
    public GameObject bluePrefab;
    public GameObject redPrefab;
    public GameObject purplePrefab;

    [Header("Unlocks")]
    public bool unlockRed;
    public bool unlockRCT;
    public bool unlockPurple;
    public bool unlockInfinity;
    public bool unlockDomain;

    [Header("Cooldowns")]
    public float blueCooldown = 3f;
    public float redCooldown = 5f;
    public float purpleCooldown = 10f;
    public float infinityCooldown = 20f;
    public float domainCooldown = 30f;

    [Header("Cooldown UI")]
    public TextMeshProUGUI blueCooldownText;
    public TextMeshProUGUI redCooldownText;
    public TextMeshProUGUI purpleCooldownText;
    public TextMeshProUGUI infinityCooldownText;
    public TextMeshProUGUI domainCooldownText;

    [Header("Cursed Energy Costs")]
    public float blueCost = 20;
    public float redCost = 30;
    public float purpleCost = 60;
    public float infinityCost = 40;
    public float domainCost = 100;

    private bool canBlue = true;
    private bool canRed = true;
    private bool canPurple = true;
    private bool canInfinity = true;
    private bool canDomain = true;

    private PlayerHealth playerHealth;
    private TargetingSystem targetingSystem;
    private CursedEnergySystem energySystem;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        targetingSystem = GetComponent<TargetingSystem>();
        energySystem = GetComponent<CursedEnergySystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            UseBlue();

        if (Input.GetKeyDown(KeyCode.E))
            UseRed();

        if (Input.GetKeyDown(KeyCode.F))
            UsePurple();

        if (Input.GetKeyDown(KeyCode.R))
            UseRCT();

        if (Input.GetKeyDown(KeyCode.C))
            UseInfinity();

        if (Input.GetKeyDown(KeyCode.V))
            UseDomain();
    }

    void UseBlue()
    {
        if (!canBlue || energySystem.currentEnergy < blueCost) return;

        Transform target = targetingSystem.GetNearestEnemy();
        if (target == null) return;

        GameObject proj = Instantiate(bluePrefab, transform.position, Quaternion.identity);

        Vector2 dir = target.position - transform.position;

        proj.GetComponent<Projectile>().SetDirection(dir);

        energySystem.UseEnergy(blueCost);

        StartCoroutine(CooldownRoutine(blueCooldown, blueCooldownText, () => canBlue = true));
        canBlue = false;
    }

    void UseRed()
    {
        if (!unlockRed || !canRed || energySystem.currentEnergy < redCost) return;

        Transform target = targetingSystem.GetNearestEnemy();

        if (target != null)
        {
            Instantiate(redPrefab, target.position, Quaternion.identity);
            energySystem.UseEnergy(redCost);
            StartCoroutine(CooldownRoutine(redCooldown, redCooldownText, () => canRed = true));
            canRed = false;
        }
    }

    void UsePurple()
    {
        if (!unlockPurple || !canPurple || energySystem.currentEnergy < purpleCost) return;

        Transform target = targetingSystem.GetNearestEnemy();

        if (target != null)
        {
            Instantiate(purplePrefab, target.position, Quaternion.identity);
            energySystem.UseEnergy(purpleCost);
            StartCoroutine(CooldownRoutine(purpleCooldown, purpleCooldownText, () => canPurple = true));
            canPurple = false;
        }
    }

    void UseRCT()
    {
        if (!unlockRCT) return;

        playerHealth.Heal(40);
    }

    void UseInfinity()
    {
        if (!unlockInfinity || !canInfinity) return;

        StartCoroutine(InfinityRoutine());
    }

    void UseDomain()
    {
        if (!unlockDomain || !canDomain) return;

        StartCoroutine(DomainRoutine());
    }

    IEnumerator CooldownRoutine(float cooldown, TextMeshProUGUI textUI, System.Action reset)
    {
        float timer = cooldown;

        while (timer > 0)
        {
            textUI.text = timer.ToString("F1");
            timer -= Time.deltaTime;
            yield return null;
        }

        textUI.text = "Ready";
        reset.Invoke();
    }

    IEnumerator InfinityRoutine()
    {
        canInfinity = false;
        playerHealth.infinityActive = true;

        yield return new WaitForSeconds(5f);

        playerHealth.infinityActive = false;

        yield return new WaitForSeconds(infinityCooldown);

        canInfinity = true;
    }

    IEnumerator DomainRoutine()
    {
        canDomain = false;

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (EnemyAI enemy in enemies)
        {
            enemy.TakeDamage(99999);
        }

        yield return new WaitForSeconds(domainCooldown);

        canDomain = true;
    }
}