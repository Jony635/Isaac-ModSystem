using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquirt : Enemy
{
    public Room currentRoom;

    private float attackTimer = 0.0f;
    private bool attacking = false;
    private bool attackFinished = true;

    public EnemySquirt()
    {
        enemyStats.damage = 3;
        enemyStats.hp = enemyStats.maxHP = 20;
        enemyStats.runSpeed = 1f;
        enemyStats.attackSpeed = 3f;

        attackTimer = 1 / enemyStats.attackSpeed;
    }

    private void Awake()
    {
        base.Awake();
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
        Vector3 target = PlayerController.Instance.body.transform.position;
        Vector3 direction = Vector3.Normalize(target - transform.position);

        while(!attacking)
        {
            yield return null;
        }

        while(attacking)
        {
            rb.MovePosition(transform.position + direction * 0.5f);

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
