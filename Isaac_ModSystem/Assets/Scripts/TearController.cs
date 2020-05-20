using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearController : MonoBehaviour
{
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        if (!GetComponent<SpriteRenderer>())
            gameObject.AddComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
        if (animator == null)
            animator = gameObject.AddComponent<Animator>();

        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Tears/Tear");

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.gravityScale = 0;

        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (!col)
            col = gameObject.AddComponent<CircleCollider2D>();

        col.radius = 0.2196912f;
        col.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer("PlayerTear");
    }

    private void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(lifeTime);

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Destroy");
    }

    public void DestroyAnimFinished()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Destroy");
    }
}
