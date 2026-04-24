using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Activation")]
    public float activationRange = 10f;
    public Transform playerTransform; // Assign in Inspector or find in Start
    private bool isActivated = false;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 30f;
    public int enemiesPerWave = 3;

    [Header("Limit Settings")]
    public int minTotalEnemies = 10;
    public int maxTotalEnemies = 20;

    private int totalToSpawn;
    private int enemiesSpawnedSoFar = 0;
    public float spawnRadius = 5.0f;

    void Start()
    {
        // Find the player in the scene by their tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Spawner could not find an object with the tag 'Player'!");
        }

        totalToSpawn = Random.Range(minTotalEnemies, maxTotalEnemies + 1);
    }


    void Update()
    {
        // Only start the routine once the player is in range
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
                enemiesSpawnedSoFar++;
            }

            if (enemiesSpawnedSoFar >= totalToSpawn) yield break;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnFromPool()
    {
        GameObject enemy = EnemyPooler.Instance.GetPooledEnemy();
        if (enemy != null)
        {
            // FIX: Define the random position variable here
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

            enemy.transform.position = randomPos;

            // 1. Activate first
            enemy.SetActive(true);

            // 2. Then reset health
            if (enemy.TryGetComponent(out Enemy_Health health))
                health.ResetEnemy();

            // 3. Force them to chase the player immediately
            if (enemy.TryGetComponent(out Enemy_Movement move))
                move.SetInitialAggro(playerTransform);
        }
    }




    private void OnDrawGizmosSelected() // Visual aid in editor
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
