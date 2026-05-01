using System.Collections.Generic;
using UnityEngine;

public class EffectPooler : MonoBehaviour
{
    public static EffectPooler instance;
    public GameObject effectPrefab; 
    public int poolSize = 20;
    private List<GameObject> pool;

    void Awake()
    {
        instance = this;
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(effectPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void PlayEffect(Vector3 position)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.SetActive(true);
                return;
            }
        }
    }
}
