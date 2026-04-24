using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler Instance; // Singleton so any script can find it

    public GameObject enemyPrefab;
    public int poolSize = 10;
    private List<GameObject> pooledEnemies;

    void Awake()
    {
        Instance = this;
        pooledEnemies = new List<GameObject>();

        // Pre-create the enemies and hide them
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            pooledEnemies.Add(obj);
        }
    }

    public GameObject GetPooledEnemy()
    {
        // Find an enemy that isn't currently in use
        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }

        // Optional: If pool is empty, create a new one (safety net)
        GameObject newObj = Instantiate(enemyPrefab);
        newObj.SetActive(false);
        pooledEnemies.Add(newObj);
        return newObj;
    }
}
