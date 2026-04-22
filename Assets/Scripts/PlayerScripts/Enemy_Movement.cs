using Unity.Cinemachine;
using UnityEditor.Tilemaps;
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
    
    //private bool isChasing;
    private int facingDirection = 1;
    private Animator anim;
    private EnemyState enemyState;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayer();

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        if (enemyState == EnemyState.Chasing)
        {
            Chase();
        }
        else if (enemyState == EnemyState.Attacking)
        {
            rb.linearVelocity = Vector2.zero;
        }
      
    }

    void Chase()
    {
        
        //I think the problem code is in this section - Enemy faces the right direction initially then messes up
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
        player = hits[0].transform;
        float distance = Vector2.Distance(transform.position, player.position);

        // FIX: Check 'attackCooldownTimer' instead of the 'attackCooldown' stat
        // Also check if we are NOT already in the attacking state
        if (distance <= attackRange && attackCooldownTimer <= 0 && enemyState != EnemyState.Attacking)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(EnemyState.Attacking);
            
            // Trigger the actual combat code here
            //GetComponent<Enemy_Combat>().Attack();
        }
        else if (distance > attackRange && enemyState != EnemyState.Attacking)
        {
            ChangeState(EnemyState.Chasing);
        }
    }
    else
    {
        rb.linearVelocity = Vector2.zero;
        ChangeState(EnemyState.Idle);
    }
}

       void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void ChangeState(EnemyState newState)
    {
        if (enemyState == newState) return; // Important: prevents restarting the animation every frame

        enemyState = newState;

        // Reset bools for Move/Idle
        anim.SetBool("isIdle", newState == EnemyState.Idle);
        anim.SetBool("isChasing", newState == EnemyState.Chasing);

        // Fire trigger for Attack
        if (newState == EnemyState.Attacking)
        {
            anim.SetTrigger("attackTrigger");
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectionRange);
    }
}

public enum EnemyState
{
    Idle,
    Chasing,
    Attacking,
}