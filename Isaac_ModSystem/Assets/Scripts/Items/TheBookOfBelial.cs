using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBookOfBelial : ActiveItem
{
    private bool activatedThisRoom = false;

    public TheBookOfBelial()
    {
        name = "TheBookOfBelial";
        pickUpSprite = "Textures/Items/collectibles_034_thebookofbelial";

        currentCharges = numCharges = 3u;
    }

    public override void OnUsed()
    {
        activatedThisRoom = true;
        PlayerController.Instance.stats.plainDamage += 2;
    }

    public override void OnNewRoomEntered(bool alreadyDefeated)
    {
        if(activatedThisRoom)
        {
            PlayerController.Instance.stats.plainDamage -= 2;
            activatedThisRoom = false;
        }
    }
}
