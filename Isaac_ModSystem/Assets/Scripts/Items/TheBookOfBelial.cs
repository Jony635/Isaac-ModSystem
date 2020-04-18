using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBookOfBelial : ActiveItem
{
    private bool activatedThisRoom = false;

    public TheBookOfBelial()
    {
        name = "TheBookOfBelial";
        pickUpSprite = "Assets/Textures/collectibles_034_thebookofbelial.png";

        currentCharges = numCharges = 3u;
    }

    public override void OnActiveButtonPressed()
    {
        activatedThisRoom = true;
        PlayerController.Instance.stats.damage += 2;
    }

    public override void OnNewRoomCleared()
    {
        if(activatedThisRoom)
        {
            PlayerController.Instance.stats.damage -= 2;
            activatedThisRoom = false;
        }
    }
}
