using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public GameObject roomPrefab;

    public GameObject roomsContainer;

    [HideInInspector]
    public Room currentRoom;

    public Room initialRoom;

    private Camera mainCamera;
    private List<Room> rooms = new List<Room>();

    [Header("Room COnfiguration")]
    public int amountRooms = 6;
    float chanceToGetARoom = 0.1f;

    private void Awake()
    {
        Instance = this;
        currentRoom = initialRoom;
        mainCamera = Camera.main;
        rooms.Add(initialRoom);

        GenerateRoomLayout();
    }

    private void GenerateRoomLayout()
    {
        uint generatedRooms = 0;

        while(generatedRooms != amountRooms)
        {
            int storedRooms = rooms.Count;
            for (int i = 0; i < storedRooms; ++i)
            {
                Room room = rooms[i];
                foreach (Door door in room.doors)
                {
                    float random = UnityEngine.Random.Range(0f, 1f);
                    if (random <= chanceToGetARoom)
                    {
                        if (door.connectedDoor != null)
                            continue;

                        Vector3 newRoomPosition = Vector3.zero;

                        switch (door.doorLocation)
                        {
                            case DoorPosition.TOP:
                                {
                                    newRoomPosition = door.thisRoom.transform.position + new Vector3(0, 10.36f, 0);
                                    break;
                                }

                            case DoorPosition.BOTTOM:
                                {
                                    newRoomPosition = door.thisRoom.transform.position + new Vector3(0, -10.36f, 0);
                                    break;
                                }
                            case DoorPosition.LEFT:
                                {
                                    newRoomPosition = door.thisRoom.transform.position + new Vector3(-15.36f, 0, 0);
                                    break;
                                }
                            case DoorPosition.RIGHT:
                                {
                                    newRoomPosition = door.thisRoom.transform.position + new Vector3(15.36f, 0, 0);
                                    break;
                                }
                            default:
                                break;
                        }

                        if (Physics2D.OverlapCircle(newRoomPosition, 1, LayerMask.GetMask("RoomIndicator")) != null)                       
                            continue;                     

                        generatedRooms++;
                     
                        GameObject newRoom = Instantiate(roomPrefab, newRoomPosition, Quaternion.identity, roomsContainer.transform);
                        newRoom.SetActive(false);

                        rooms.Add(newRoom.GetComponent<Room>());
                        storedRooms++;

                        switch (door.doorLocation)
                        {
                            case DoorPosition.TOP:
                                {
                                    Door newDoor = newRoom.GetComponent<Room>().doors[(int)DoorPosition.BOTTOM];
                                    door.connectedDoor = newDoor;
                                    newDoor.connectedDoor = door;
                                    break;
                                }

                            case DoorPosition.BOTTOM:
                                {
                                    Door newDoor = newRoom.GetComponent<Room>().doors[(int)DoorPosition.TOP];
                                    door.connectedDoor = newDoor;
                                    newDoor.connectedDoor = door; break;
                                }
                            case DoorPosition.LEFT:
                                {
                                    Door newDoor = newRoom.GetComponent<Room>().doors[(int)DoorPosition.RIGHT];
                                    door.connectedDoor = newDoor;
                                    newDoor.connectedDoor = door; break;
                                }
                            case DoorPosition.RIGHT:
                                {
                                    Door newDoor = newRoom.GetComponent<Room>().doors[(int)DoorPosition.LEFT];
                                    door.connectedDoor = newDoor;
                                    newDoor.connectedDoor = door; break;
                                }
                            default:
                                break;
                        }

                        door.UnLockDoor();
                        door.connectedDoor.UnLockDoor();

                        if (generatedRooms == amountRooms)
                            return;
                    }               
                }
            }
        }
    }

    public void DoorTrespassed(Door door)
    {
        Vector3 characterOffset = Vector3.zero;

        switch (door.doorLocation)
        {
            case DoorPosition.TOP:
                {
                    characterOffset = new Vector3(0, 0.7f, 0);
                    break;
                }

            case DoorPosition.BOTTOM:
                {
                    characterOffset = new Vector3(0, -0.7f, 0);
                    break;
                }
            case DoorPosition.LEFT:
                {
                    characterOffset = new Vector3(-0.7f, 0, 0);
                    break;
                }
            case DoorPosition.RIGHT:
                {
                    characterOffset = new Vector3(0.7f, 0, 0);
                    break;
                }
            default:
                break;
        }

        door.connectedDoor.thisRoom.gameObject.SetActive(true);

        PlayerController.Instance.enabled = false;

        PlayerController.Instance.transform.position = door.connectedDoor.transform.position + characterOffset;
        PlayerController.Instance.ResetBodyAnimator();

        //Camera.main.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y, -1);
        StartCoroutine(CameraTranslation(mainCamera, new Vector3(door.connectedDoor.thisRoom.transform.position.x, door.connectedDoor.thisRoom.transform.position.y, -1), 0.1f, () =>
        {
            door.thisRoom.gameObject.SetActive(false);
            PlayerController.Instance.enabled = true;
            currentRoom = door.connectedDoor.thisRoom;

            currentRoom.GetComponent<Room>().SetUpRoom();
        }));       
    }

    private IEnumerator CameraTranslation(Camera camera, Vector3 desiredPosition, float time, Action callback)
    {
        Vector3 initialPosition = camera.transform.position;
        float timePassed = 0f;

        while(camera.transform.position != desiredPosition)
        {
            camera.transform.position = Vector3.Lerp(initialPosition, desiredPosition, timePassed / time);
            
            if(camera.transform.position != desiredPosition)
                yield return null;

            timePassed += Time.deltaTime;
        }

        callback.Invoke();
    }  

    public void RoomCleared()
    {
        ActiveItemContainer.Instance.RoomCleared();

        bool levelFinished = true;
        foreach(Room room in rooms)
        {
            if(!room.alreadyDefeated)
            {
                levelFinished = false;
            }
        }

        if(levelFinished)
        {
            //Temp solution: Restart the game
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
