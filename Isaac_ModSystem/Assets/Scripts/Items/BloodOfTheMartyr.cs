using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodOfTheMartyr : Item
{
    public BloodOfTheMartyr()
    {
        name = "BloodOfTheMartyr";
        pickUpSprite = "Assets/Textures/collectibles_007_bloodofthemartyr.png";
    }

    public override void OnItemEquipped()
    {
        PlayerController.Instance.stats.damage += 1;
    }
}
