using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject lockedSprite;

    public void LockDoor()
    {
        lockedSprite.SetActive(true);
    }

    public void UnLockDoor()
    {
        lockedSprite.SetActive(false);
    }

}
