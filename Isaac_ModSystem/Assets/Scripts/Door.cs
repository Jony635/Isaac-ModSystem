using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorPosition { TOP, BOTTOM, LEFT, RIGHT };

public class Door : MonoBehaviour
{
    public GameObject lockedSprite;
    public DoorPosition doorLocation;

    public Room thisRoom;
    public Door connectedDoor = null;

    public void Awake()
    {
        thisRoom = GetComponentInParent<Room>();
    }

    public void LockDoor()
    {
        lockedSprite.SetActive(true);
    }

    public void UnLockDoor()
    {
        lockedSprite.SetActive(false);
    }
}
