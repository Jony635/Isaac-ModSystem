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
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerTear"))
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Destroy");
        }
    }
}
