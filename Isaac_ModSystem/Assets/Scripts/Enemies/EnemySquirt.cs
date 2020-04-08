using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquirt : Enemy
{
    public Room currentRoom;

    private bool attacked = false;

    public EnemySquirt()
    {
        enemyStats.damage = 3;
        enemyStats.hp = enemyStats.maxHP = 10;
        enemyStats.runSpeed = 30f * 2;
    }

    private void Awake()
    {
        
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


    }

    protected override void Die()
    {
        //TODO: Animations, delete/disable gameObjects, play FX, etc.
    }


}
