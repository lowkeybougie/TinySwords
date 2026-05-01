using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat playerCombat;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepSFX;
    public float footstepRate = 0.4f; // How fast footsteps play
    private float footstepTimer;

    private Vector2 moveInput;
    private bool isKnockedBack;

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Slash"))
        {
            playerCombat.Attack();
        }

        // Handle Footstep Audio
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        // Only play if moving, not attacking, and not knocked back
        bool isMoving = moveInput.sqrMagnitude > 0;
        bool canPlaySteps = !isKnockedBack && !anim.GetBool("isAttacking");

        if (isMoving && canPlaySteps)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                audioSource.PlayOneShot(footstepSFX);
                footstepTimer = footstepRate;
            }
        }
        else
        {
            footstepTimer = 0; // Reset so steps start immediately when you move again
        }
    }

    void FixedUpdate()
    {
        // 1. If knocked back, DON'T run movement logic (this lets the knockback force work)
        if (isKnockedBack) return;

        // 2. If attacking, stay still
        if (anim.GetBool("isAttacking"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 3. Normal movement logic
        rb.linearVelocity = moveInput.normalized * speed;

        if (moveInput.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), 1, 1);
        }

        anim.SetFloat("horizontal", Mathf.Abs(moveInput.x));
        anim.SetFloat("vertical", Mathf.Abs(moveInput.y));
    }


    public void KnockBack(Transform enemy, float force, float stunTime)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(enemy, force, stunTime));
    }

    private System.Collections.IEnumerator KnockbackRoutine(Transform enemy, float force, float stunTime)
    {
        isKnockedBack = true;
        Vector2 dir = (transform.position - enemy.position).normalized;
        rb.linearVelocity = dir * force;
        yield return new WaitForSeconds(stunTime);
        isKnockedBack = false;
    }

}
