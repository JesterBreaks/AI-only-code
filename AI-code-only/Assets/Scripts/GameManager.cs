using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Enemy Spawning")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxEnemies = 15;
    public int enemiesPerWave = 3;
    public int waveCount = 0;

    [Header("Wave UI")]
    public TMPro.TextMeshProUGUI waveText;
    public TMPro.TextMeshProUGUI killCountText;

    private int killCount;
    private bool gameRunning;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        gameRunning = true;
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2f);

        while (gameRunning)
        {
            waveCount++;
            if (waveText != null)
                waveText.text = $"Wave {waveCount}";

            int toSpawn = enemiesPerWave + (waveCount - 1) * 2;

            for (int i = 0; i < toSpawn; i++)
            {
                int currentCount = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;
                if (currentCount < maxEnemies)
                    SpawnEnemy();
            }

            yield return new WaitForSeconds(spawnInterval + waveCount * 1.5f);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Transform spawnPt = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(prefab, spawnPt.position, Quaternion.identity);
    }

    public void OnEnemyKilled()
    {
        killCount++;
        if (killCountText != null)
            killCountText.text = $"Kills: {killCount}";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}