using System.Collections;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;
    public float knockbackForce = 50; // The force the ENEMY exerts on the player

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }

    public void Knockback(Transform attackerTransform, float force, float stunTime, float knockbackTime)
    {
        enemy_Movement.ChangeState(EnemyState.Knockback);
        StopAllCoroutines();

        Vector2 direction = (transform.position - attackerTransform.position).normalized;
        rb.linearVelocity = direction * force;

        StartCoroutine(StunTimer(knockbackTime, stunTime));
    }

    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero; // Abrupt stop
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);
    }
}
