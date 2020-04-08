using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Room : MonoBehaviour
{
    public Door[] doors = { };
    public GameObject itemAltar;
    public GameObject monsters;

    private void Awake()
    {
        //Spawn the Item Altar
        if(itemAltar != null)
        {
            itemAltar.SetActive(true);
        }

        //Spawn Monsters
        if(this != RoomManager.Instance.initialRoom)
        {
            for(int i = 0; i < monsters.transform.childCount; ++i)
            {
                monsters.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
