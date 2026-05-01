using UnityEngine;

public class EffectPooler : MonoBehaviour
{
    public static EffectPooler instance;
    public GameObject playerHitPrefab; // Blue
    public GameObject enemyHitPrefab;  // Red

    void Awake() => instance = this;

    public void PlayPlayerHit(Vector3 pos) => Instantiate(playerHitPrefab, pos, Quaternion.identity);
    public void PlayEnemyHit(Vector3 pos) => Instantiate(enemyHitPrefab, pos, Quaternion.identity);
}
