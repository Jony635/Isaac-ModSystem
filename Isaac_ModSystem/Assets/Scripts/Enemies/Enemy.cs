using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public LuaScriptController luaScript = null;
    
    public struct EnemyStats
    {
        bool constant_damage;

        public float maxHP;
        public float hp;
        public float damage;
        public float tick_damage;
        public float speed;
        public float runSpeed;
        public float attackSpeed;

        public EnemyStats(string useless)
        {
            constant_damage = false;

            maxHP = hp = 2f;
            damage = tick_damage = 0.5f;
            speed = 30f;
            runSpeed = speed * 1.5f;
            attackSpeed = 1f;
        }
    };

    public EnemyStats enemyStats = new EnemyStats("");

    public float difficulty = 1f;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D capsule;

    [HideInInspector]
    public Room currentRoom;

    protected virtual void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        if (!animator)
            animator = gameObject.AddComponent<Animator>();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Monsters/Squirt");

        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!rb)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if(!spriteRenderer)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        capsule = gameObject.GetComponent<CapsuleCollider2D>();
        if (!capsule)
            capsule = gameObject.AddComponent<CapsuleCollider2D>();
        capsule.direction = CapsuleDirection2D.Horizontal;
        capsule.offset = new Vector2(0.01248912f, 0.4911186f);
        capsule.size = new Vector2(1.338645f, 0.909272f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (LayerMask.LayerToName(collider.gameObject.layer) == "PlayerTear")
        {
            TakeDamage(PlayerController.Instance.stats.plainDamage * PlayerController.Instance.stats.factorDamage);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    public void TakeDamage(float damage)
    {
        enemyStats.hp -= damage;

        if(enemyStats.hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {

    }
}
