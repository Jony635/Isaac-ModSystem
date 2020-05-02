using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOfTheMartyr : Item
{
    public BloodOfTheMartyr()
    {
        name = "BloodOfTheMartyr";
        pickUpSprite = "Textures/Items/collectibles_007_bloodofthemartyr";
    }

    public override void OnEquipped()
    {
        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.plainDamage += 1;

        PlayerController.Instance.stats = newStats;
    }
}
