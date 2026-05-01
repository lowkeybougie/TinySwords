using UnityEngine;

public class WinCollectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("<color=green>WIN ITEM COLLECTED!</color>");

            if (GameOverManager.instance != null)
            {
                
                GameOverManager.instance.ShowWinScreen();
                Debug.Log("Called ShowWinScreen on GameOverManager.");
            }
            else
            {
                Debug.LogError("GameOverManager Instance is MISSING from the scene!");
            }

            Destroy(gameObject);
        }
    }
}
