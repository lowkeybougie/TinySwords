using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("UI References")]
    public TMP_Text healthText; // Drag your TextMeshPro object here
    public Animator healthTextAnim;

    private void Start()
    {
        // Initialize health if not set
        if (currentHealth <= 0) currentHealth = maxHealth;
        UpdateUI();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateUI();
        if (healthTextAnim != null) healthTextAnim.Play("TextUpdate");

        if (currentHealth <= 0) Die();
    }

    private void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        }
    }

    private void Die()
    {
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowGameOver();
        }
        gameObject.SetActive(false);
    }
}
