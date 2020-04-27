using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public struct EnemyStats
    {
        public float maxHP;
        public float hp;
        public float damage;
        public float speed;
        public float runSpeed;
        public float attackSpeed;

        public EnemyStats(string useless)
        {
            maxHP = hp = 2f;
            damage = 0.5f;
            speed = 30f;
            runSpeed = speed * 1.5f;
            attackSpeed = 1f;
        }
    };

    public EnemyStats enemyStats = new EnemyStats("");

    public float difficulty = 1f;

    protected Animator animator;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if(LayerMask.LayerToName(collider.gameObject.layer) == "PlayerTear")
        {
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

    protected abstract void Die();
}
