using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public float cooldown = 0.5f; // Reduced for better feel
    private float timer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;

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
            if (attackSFX != null) audioSource.PlayOneShot(attackSFX);

            // Failsafe: Reset attack bool after 0.5s in case Animation Event fails
            Invoke(nameof(FinishAttacking), 0.5f);
        }
    }

    // Called via Animation Event
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.TryGetComponent(out Enemy_Health health)) health.ChangeHealth(-damage);
            if (enemy.TryGetComponent(out KnockBack kb)) kb.Knockback(transform, 10f, 0.2f, 0.1f);

            if (EffectPooler.instance != null) EffectPooler.instance.PlayEffect(enemy.transform.position);
        }
    }

    public void FinishAttacking()
    {
        CancelInvoke(nameof(FinishAttacking)); // Clear failsafe
        anim.SetBool("isAttacking", false);
    }
}
