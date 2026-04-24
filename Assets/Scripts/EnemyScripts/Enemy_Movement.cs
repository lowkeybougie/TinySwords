using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float attackRange = 2;
    public float speed;
    public float attackCooldown = 2;
    public float playerDetectionRange = 5;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    private float attackCooldownTimer;
    private Rigidbody2D rb;
    private Transform player;
    private int facingDirection = 1;
    private Animator anim;
    private EnemyState enemyState;

    
    void Awake()
    {
        // Getting components should stay in Awake or Start
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Add this new function:
    void OnEnable()
    {
        // This runs every time the enemy is "spawned" from the pool
        attackCooldownTimer = 0; // Reset their attack brain

        if (player == null)
        {
            ChangeState(EnemyState.Idle);
        }
    }


    void Update()
    {
        // Don't process movement/AI logic if we are being knocked back
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
                attackCooldownTimer -= Time.deltaTime;

            if (enemyState == EnemyState.Chasing)
                Chase();
            else if (enemyState == EnemyState.Attacking || enemyState == EnemyState.Idle)
                rb.linearVelocity = Vector2.zero; // Abrupt stop
        }
    }

    void Chase()
    {
        if (player == null) return;

        // Handle Flipping Sprite
        if (player.position.x > transform.position.x && facingDirection == -1 || player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        // Direct movement toward target
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectionRange, playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;
        }

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= attackRange && attackCooldownTimer <= 0 && enemyState != EnemyState.Attacking)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (distance > attackRange && enemyState != EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }
    }

    public void SetInitialAggro(Transform targetTransform)
    {
        this.player = targetTransform;
        ChangeState(EnemyState.Chasing);
        Chase();
    }

    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return;

        enemyState = newState;
        anim.SetBool("isIdle", newState == EnemyState.Idle);
        anim.SetBool("isChasing", newState == EnemyState.Chasing);

        if (newState == EnemyState.Attacking)
        {
            anim.SetTrigger("attackTrigger");
        }
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
    }
}

// THIS PART FIXES YOUR ERRORS: It must be outside the class brackets
public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
    Knockback
}
