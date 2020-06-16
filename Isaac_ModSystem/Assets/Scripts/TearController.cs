using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearController : MonoBehaviour
{
    public float lifeTime = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private static uint nextTearShootIndex = 0u;

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

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;

        audioSource.clip = FXReferences.Instance.tearShootClips[nextTearShootIndex];
        audioSource.Play();
        nextTearShootIndex = nextTearShootIndex + 1 < FXReferences.Instance.tearShootClips.Length ? nextTearShootIndex + 1 : 0;
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

        audioSource.clip = FXReferences.Instance.tearDestroyed;
        audioSource.Play();
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

        audioSource.clip = FXReferences.Instance.tearDestroyed;
        audioSource.Play();
    }
}
