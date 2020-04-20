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

    public override void OnItemEquipped()
    {
        PlayerController.Instance.stats.damage += 1;
    }
}
