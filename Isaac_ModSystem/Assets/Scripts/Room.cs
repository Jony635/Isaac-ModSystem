using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour
{
    public Door[] doors = { };

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if(Keyboard.current.digit1Key.isPressed)
            {
                foreach (Door door in doors)
                    door.LockDoor();
            }
            if(Keyboard.current.digit2Key.isPressed)
            {
                foreach (Door door in doors)
                    door.UnLockDoor();
            }
        }
    }
}
