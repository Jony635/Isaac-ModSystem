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
        public float speed;
        public float speedFactor;
        public float runSpeed;
        public float attackSpeed;

        public EnemyStats(string useless)
        {
            constant_damage = false;

            maxHP = hp = 2f;
            damage = 1f;
            speed = 20f;
            speedFactor = 1f;
            runSpeed = speed * 1.5f;
            attackSpeed = 1f;
        }
    };

    public EnemyStats enemyStats = new EnemyStats("");

    public float difficulty = 1f;

    protected Animator animator;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;

    [HideInInspector]
    public Room currentRoom;

    protected virtual void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Monster");

        animator = gameObject.GetComponent<Animator>();
        if (!animator)
            animator = gameObject.AddComponent<Animator>();

        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!rb)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if(!spriteRenderer)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        spriteRenderer.sortingOrder = 9;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (LayerMask.LayerToName(collider.gameObject.layer) == "PlayerTear")
        {
            Rigidbody2D tearRB = collider.GetComponent<Rigidbody2D>();

            ItemManager.Instance.OnMonsterHittedByTear(this);
            MonsterManager.Instance.OnMonsterHittedByTear(this, tearRB.velocity);
            TakeDamage(PlayerController.Instance.stats.plainDamage * PlayerController.Instance.stats.factorDamage);
        }
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
        if (luaScript != null) luaScript.OnEnemyDie();
    }

    public virtual void OnMonsterHittedByTear(Enemy enemy, Vector2 velocity)
    {
        if (luaScript)
            luaScript.OnMonsterHittedByTear(enemy, velocity);
    }
}
