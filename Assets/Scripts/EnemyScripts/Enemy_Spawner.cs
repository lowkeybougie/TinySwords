using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Activation")]
    public float activationRange = 10f;
    private Transform playerTransform;
    private bool isActivated = false;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 30f;
    public int enemiesPerWave = 3;

    [Header("Limit Settings")]
    public int minTotalEnemies = 10;
    public int maxTotalEnemies = 20;
    private int totalToSpawn;
    private int enemiesSpawnedSoFar = 0;
    public float spawnRadius = 3.0f;

    [Header("Spawn Safety")]
    [Tooltip("Size of the area to check for obstacles. Set this slightly larger than your enemy.")]
    public float safetyRadius = 0.8f;
    [Tooltip("Set this to the Layers used by your Mountains, Walls, and Elevations.")]
    public LayerMask obstacleLayer;
    [Tooltip("How many times to try finding a clear spot before giving up for this attempt.")]
    public int maxAttempts = 10;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        totalToSpawn = Random.Range(minTotalEnemies, maxTotalEnemies + 1);
    }

    void Update()
    {
        if (playerTransform == null) return;

        if (!isActivated && Vector2.Distance(transform.position, playerTransform.position) <= activationRange)
        {
            isActivated = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (enemiesSpawnedSoFar < totalToSpawn)
        {
            int remaining = totalToSpawn - enemiesSpawnedSoFar;
            int spawnThisTime = Mathf.Min(enemiesPerWave, remaining);

            for (int i = 0; i < spawnThisTime; i++)
            {
                SpawnFromPool();
            }

            if (enemiesSpawnedSoFar >= totalToSpawn) yield break;

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnFromPool()
    {
        GameObject enemy = EnemyPooler.Instance.GetPooledEnemy();
        if (enemy == null) return;

        Vector2 spawnPos = Vector2.zero;
        bool validSpotFound = false;

        // Try to find a clear spot that isn't overlapping an obstacle
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            // Check for any colliders on the obstacle layer within the safety radius
            Collider2D hit = Physics2D.OverlapCircle(randomPos, safetyRadius, obstacleLayer);

            if (hit == null)
            {
                spawnPos = randomPos;
                validSpotFound = true;
                break;
            }
        }

        // If we found a good spot, spawn the enemy. Otherwise, they skip this attempt to avoid getting stuck.
        if (validSpotFound)
        {
            enemy.transform.position = spawnPos;
            enemy.SetActive(true);
            enemiesSpawnedSoFar++;

            if (enemy.TryGetComponent(out Enemy_Health health)) health.ResetEnemy();
            if (enemy.TryGetComponent(out Enemy_Movement move)) move.SetInitialAggro(playerTransform);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} could not find a clear spot to spawn an enemy after {maxAttempts} tries.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
