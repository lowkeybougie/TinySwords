using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int damage = 1; // Changed to positive for clarity (subtraction happens in health script)
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float knockbackForce = 10f;
    public float stunTime = 0.2f;
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;

    public void Attack()
    {
        if (audioSource != null && attackSFX != null)
        {
            audioSource.PlayOneShot(attackSFX);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        if (hits.Length > 0)
        {
            // 1. Play the hit effect at the player's position
            if (EffectPooler.instance != null)
            {
                if (EffectPooler.instance != null)
                    EffectPooler.instance.PlayPlayerHit(hits[0].transform.position); // Plays BLUE

            }

            // 2. Apply Health and Knockback
            if (hits[0].TryGetComponent(out PlayerHealth health))
                health.ChangeHealth(-damage);

            if (hits[0].TryGetComponent(out PlayerMovement pm))
                pm.KnockBack(transform, knockbackForce, stunTime);
        }
    }

}
