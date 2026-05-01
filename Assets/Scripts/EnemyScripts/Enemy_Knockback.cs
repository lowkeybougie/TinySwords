using UnityEngine;
using System.Collections;

public class Enemy_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockedBack;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Knockback(Transform attacker, float force, float stunTime, float delay)
    {
        // Safety check to prevent the "Inactive" error
        if (gameObject.activeInHierarchy && !isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(attacker, force, stunTime));
        }
    }

    private IEnumerator KnockbackRoutine(Transform attacker, float force, float stunTime)
    {
        isKnockedBack = true;

        // Calculate direction and apply force
        Vector2 direction = (transform.position - attacker.position).normalized;
        rb.linearVelocity = direction * force;

        yield return new WaitForSeconds(stunTime);

        rb.linearVelocity = Vector2.zero; // Stop the sliding
        isKnockedBack = false;
    }

    // This property lets your Enemy_Movement script know to pause chasing
    public bool IsBeingKnockedBack() => isKnockedBack;
}
