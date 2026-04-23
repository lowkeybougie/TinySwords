using UnityEngine;
using System.Collections;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 50; // The force YOU exert on enemies
    public float stunTime = 0.2f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float cooldown = 2;
    public float timer;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (timer > 0) timer -= Time.deltaTime;
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
    }

    // Called by Animation Event when hitting an ENEMY
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            enemy.GetComponent<Enemy_Health>().ChangeHealth(-damage);
            // Push the enemy away using YOUR force
            enemy.GetComponent<KnockBack>().Knockback(transform, knockbackForce, stunTime);
        }
    }

    // NEW: Call this when an enemy hits the PLAYER
    public void GetKnockedBack(Transform enemyTransform, float force, float stun)
    {
        StopAllCoroutines();
        Vector2 direction = (transform.position - enemyTransform.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(StunTimer(stun));
    }

    IEnumerator StunTimer(float time)
    {
        yield return new WaitForSeconds(time);
        rb.linearVelocity = Vector2.zero; // Abrupt stop
    }

    public void FinishAttacking() { anim.SetBool("isAttacking", false); }
}
