using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat playerCombat;

    private Vector2 moveInput;
    private bool isKnockedBack;

    void Update()
    {
        // Collect input in Update for responsiveness
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Slash"))
        {
            playerCombat.Attack();
        }
    }

    void FixedUpdate()
    {
        // Stop movement if attacking OR knocked back
        if (isKnockedBack || anim.GetBool("isAttacking"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

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
