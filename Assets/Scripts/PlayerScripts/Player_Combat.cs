using UnityEngine;
using System.Collections;

public class Player_Combat : MonoBehaviour
{
    public Animator anim;
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 50;
    public float stunTime = 0.3f;
    public float knockbackTime = .15f; 
    public LayerMask enemyLayer;
    public int damage = 1;
    public float cooldown = 2;
    public float timer;

    private Rigidbody2D rb;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;
    public AudioClip playerHitSFX;

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
            // Play attack sound
            if (attackSFX != null) audioSource.PlayOneShot(attackSFX);
        }
    }

    
    public void DealDamage()
    {
        
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            
            if (enemy.TryGetComponent(out Enemy_Health health))
            {
                health.ChangeHealth(-damage);
            }

            
            if (enemy.TryGetComponent(out Enemy_Movement move))
            {
                
            }

           
            if (EffectPooler.instance != null)
            {
                EffectPooler.instance.PlayEffect(enemy.transform.position);
            }
        }
    }


   
    public void GetKnockedBack(Transform enemyTransform, float force, float stun)
    {
        StopAllCoroutines();
        Vector2 direction = (transform.position - enemyTransform.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(StunTimer(stun));
        if (playerHitSFX != null) audioSource.PlayOneShot(playerHitSFX);
    }

    IEnumerator StunTimer(float time)
    {
        yield return new WaitForSeconds(time);
        rb.linearVelocity = Vector2.zero;
    }

    public void FinishAttacking() { anim.SetBool("isAttacking", false); }
}
