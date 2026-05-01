using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [Header("Combat Stats")]
    public float attackRange = 2f;
    public float speed = 3f;
    public float attackCooldown = 2f;
    public int damage = 1;
    public float knockbackForce = 15f;
    public float stunTime = 0.5f;

    [Header("Detection")]
    public float playerDetectionRange = 5f;
    public Transform detectionPoint;
    public LayerMask playerLayer; 
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;

    private float attackCooldownTimer;
    private Rigidbody2D rb;
    private Transform player;
    private int facingDirection = 1;
    private Animator anim;
    private EnemyState enemyState;
    private SpriteRenderer sr;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        attackCooldownTimer = 0;
       
        if (player == null) ChangeState(EnemyState.Idle);
    }

    void Update()
    {
      
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;

            if (enemyState == EnemyState.Chasing)
                Chase();
            else if (enemyState == EnemyState.Attacking || enemyState == EnemyState.Idle)
                rb.linearVelocity = Vector2.zero; 
        }
    }

    void Chase()
    {
        if (player == null) return;

       
        if (player.position.x > transform.position.x && facingDirection == -1 ||
            player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void CheckForPlayer()
    {
       
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);

        if (hits.Length > 0)
        {
            Transform target = hits[0].transform;

            if (target.TryGetComponent(out SpriteRenderer playerSR))
            {
                if (playerSR.sortingOrder != sr.sortingOrder)
                {
                    player = null;
                    ChangeState(EnemyState.Idle);
                    return;
                }
            }

            player = target;
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange && attackCooldownTimer <= 0 && enemyState != EnemyState.Attacking)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);

                if (audioSource != null && attackSFX != null)
                    audioSource.PlayOneShot(attackSFX);
            }
            else if (distance > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            player = null;
            ChangeState(EnemyState.Idle);
        }
    }

    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, attackRange, playerLayer);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Player_Combat pc))
            {
                pc.GetKnockedBack(transform, knockbackForce, stunTime);
            }

            if (hit.TryGetComponent(out PlayerHealth ph))
            {
                ph.ChangeHealth(-damage);
            }
        }
    }

    public void SetInitialAggro(Transform targetTransform)
    {
        this.player = targetTransform;
        ChangeState(EnemyState.Chasing);
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;
        enemyState = newState;

        anim.SetBool("isIdle", newState == EnemyState.Idle);
        anim.SetBool("isChasing", newState == EnemyState.Chasing);

        if (newState == EnemyState.Attacking)
            anim.SetTrigger("attackTrigger");
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, attackRange);
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}
