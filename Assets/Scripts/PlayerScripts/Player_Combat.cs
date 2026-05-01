using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float cooldown = 0.5f;
    private float timer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;

    private void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            timer = cooldown;

          
            if (attackSFX != null && audioSource != null)
                audioSource.PlayOneShot(attackSFX);

      
            Invoke(nameof(FinishAttacking), 0.4f);
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            if (enemy.TryGetComponent(out Enemy_Health health))
            {
                if (health.currentHealth <= 0) continue; 

                
                if (EffectPooler.instance != null)
                    EffectPooler.instance.PlayEnemyHit(enemy.bounds.center);

                health.ChangeHealth(-damage);
            }

            if (enemy.TryGetComponent(out Enemy_Knockback kb))
            {
                kb.Knockback(transform, 10f, 0.2f, 0.1f);
            }
        }
    }



    public void FinishAttacking()
    {
        CancelInvoke(nameof(FinishAttacking));
        anim.SetBool("isAttacking", false);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}
