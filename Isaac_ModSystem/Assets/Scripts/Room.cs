using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour
{
    public Door[] doors = { };
    public GameObject itemAltar;
    public GameObject monstersParent;

    [HideInInspector]
    public Enemy[] monsters;

    public bool alreadyDefeated = false;

    private AudioSource audioSource;

    public void SetUpRoom()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
            audioSource.clip = FXReferences.Instance.roomCleared;

        //Spawn Monsters
        if (this != RoomManager.Instance.initialRoom && !alreadyDefeated)
        {
            monsters = MonsterManager.Instance.GetEnemiesWith(PlayerController.Instance.difficulty);

            if(monsters.Length > 0 )
            {
                foreach (Door door in doors)
                {
                    door.LockDoor();
                }

                foreach(Enemy enemy in monsters)
                {
                    enemy.transform.SetParent(monstersParent.transform);
                    enemy.transform.position = transform.position;
                    enemy.currentRoom = this;
                    enemy.gameObject.SetActive(true);
                }
            }
        }
    }

    public void OnMonsterDied()
    {
        bool unlockDoors = true;

        foreach(Enemy enemy in monsters)
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
            MonsterManager.Instance.ClearEnemiesRef();

            if(audioSource)           
                audioSource.Play();      
        }
    }
}
