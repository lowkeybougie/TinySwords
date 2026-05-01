using UnityEngine;
using System.Collections;

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
            
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            enemy.transform.position = randomPos;

            enemy.SetActive(true);

            if (enemy.TryGetComponent(out Enemy_Health health)) health.ResetEnemy();
            if (enemy.TryGetComponent(out Enemy_Movement move)) move.SetInitialAggro(playerTransform);
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
