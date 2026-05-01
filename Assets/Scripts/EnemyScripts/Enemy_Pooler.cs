using System.Collections.Generic;
using UnityEngine;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler Instance; 

    public GameObject enemyPrefab;
    public int poolSize = 10;
    private List<GameObject> pooledEnemies;

    void Awake()
    {
        Instance = this;
        pooledEnemies = new List<GameObject>();

        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false);
            pooledEnemies.Add(obj);
        }
    }

    public GameObject GetPooledEnemy()
    {
       
        for (int i = 0; i < pooledEnemies.Count; i++)
        {
            if (!pooledEnemies[i].activeInHierarchy)
            {
                return pooledEnemies[i];
            }
        }

        
        GameObject newObj = Instantiate(enemyPrefab);
        newObj.SetActive(false);
        pooledEnemies.Add(newObj);
        return newObj;
    }
}
