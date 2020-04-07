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
            maxHP = hp = 10f;
            damage = 3f;
            speed = 30f;
            runSpeed = speed * 1.5f;
            attackSpeed = 1f;
        }
    };

    protected EnemyStats enemyStats = new EnemyStats("");

    protected Animator animator;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
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
