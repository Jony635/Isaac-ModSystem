using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySquirt : Enemy
{
    public EnemySquirt()
    {
        enemyStats.damage = 3;
        enemyStats.hp = enemyStats.maxHP = 10;
        enemyStats.runSpeed = 30f * 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Monster logic
    }

    protected override void Die()
    {
        //TODO: Animations, delete/disable gameObjects, play FX, etc.
    }
}
