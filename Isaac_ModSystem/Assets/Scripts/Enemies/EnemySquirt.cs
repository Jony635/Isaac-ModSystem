using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquirt : Enemy
{
    private float attackTimer = 0.0f;
    private bool attacking = false;
    private bool attackFinished = true;

    protected CapsuleCollider2D capsule;

    public EnemySquirt()
    {
        enemyStats.damage = 1f;
        enemyStats.hp = enemyStats.maxHP = 20;
        enemyStats.attackSpeed = 3f;

        attackTimer = 1 / enemyStats.attackSpeed;

        difficulty = 1;
    }

    protected override void Awake()
    {
        base.Awake();
        animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/Monsters/Squirt");

        capsule = gameObject.GetComponent<CapsuleCollider2D>();
        if (!capsule)
            capsule = gameObject.AddComponent<CapsuleCollider2D>();
        capsule.direction = CapsuleDirection2D.Horizontal;
        capsule.offset = new Vector2(0.01248912f, 0.4911186f);
        capsule.size = new Vector2(1.338645f, 0.909272f);
    }

    // Update is called once per frame
    void Update()
    {
        #region LOOK AT PLAYER
        if (PlayerController.Instance.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        #endregion

        #region ATTACK
        if (attackFinished)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                attackFinished = false;
                StartCoroutine(AttackCoroutine());
                attackTimer = 1 / enemyStats.attackSpeed;
            }            
        }
        #endregion
    }

    IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("Attack");

        Vector2 target = PlayerController.Instance.body.transform.position;
        Vector2 direction = Vector3.Normalize(target - (Vector2)transform.position);

        while(!attacking)
        {
            yield return null;
        }

        while(attacking)
        {
            rb.MovePosition(rb.position + direction * enemyStats.runSpeed * enemyStats.speedFactor * Time.fixedDeltaTime);

            yield return null;
        }    
    }

    protected override void Die()
    {
        //TODO: Animations, delete/disable gameObjects, play FX, etc.
        gameObject.SetActive(false);
        currentRoom.OnMonsterDied();
    }

    public void AttackMovementStarted()
    {
        attacking = true;
    }

    public void AttackMovementFinished()
    {
        attacking = false;
        attackFinished = true;
        rb.velocity = Vector2.zero;
    }
}
