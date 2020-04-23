using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour
{
    public Door[] doors = { };
    public GameObject itemAltar;
    public GameObject monsters;

    public bool alreadyDefeated = false;

    public void SetUpRoom()
    { 
        //Spawn Monsters
        if(this != RoomManager.Instance.initialRoom && !alreadyDefeated)
        {
            if(monsters.transform.childCount > 0)
            {
                foreach(Door door in doors)
                {
                    door.LockDoor();
                }

                for (int i = 0; i < monsters.transform.childCount; ++i)
                {
                    monsters.transform.GetChild(i).gameObject.SetActive(true);
                }
            }           
        }
    }

    public void OnMonsterDied()
    {
        bool unlockDoors = true;

        foreach(Enemy enemy in monsters.GetComponentsInChildren<Enemy>())
        {
            if (enemy.gameObject.activeInHierarchy)
                unlockDoors = false;
        }

        if(unlockDoors)
        {
            alreadyDefeated = true;
            foreach(Door door in doors)
            {
                if (door.connectedDoor != null)
                    door.UnLockDoor();
            }

            itemAltar.SetActive(true);

            RoomManager.Instance.OnNewRoomCleared();
            ItemManager.Instance.OnNewRoomCleared();
        }
    }
}
