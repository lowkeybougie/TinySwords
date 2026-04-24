using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSFX; // The "Enemy Ouch" sound

    private void Start()
    {
        ResetEnemy();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // If taking damage (negative amount), play the sound
        if (amount < 0 && audioSource != null && hitSFX != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    public void ResetEnemy()
    {
        currentHealth = maxHealth;
    }
}
