using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSFX; 

    private void Start()
    {
        ResetEnemy();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

       
        if (amount < 0 && audioSource != null && hitSFX != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        {
            Die();
        }
    }

    
    void Die()
    {
        
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        
        GetComponent<Collider2D>().enabled = false;

       
        gameObject.SetActive(false);
    }


    public void ResetEnemy()
    {
        currentHealth = maxHealth;
    }
}
