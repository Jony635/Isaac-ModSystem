using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    public GameObject roomPrefab;

    public GameObject roomsContainer;

    [HideInInspector]
    public Room currentRoom;

    public Room initialRoom;
    public List<Room> rooms = new List<Room>();

    private Camera mainCamera;

    private void Awake()
    {
        Instance = this;
        currentRoom = initialRoom;
        mainCamera = Camera.main;
    }

    public void DoorTrespassed(Door door)
    {
        Vector3 newRoomPosition = Vector3.zero;
        Vector3 characterOffset = Vector3.zero;

        switch(door.doorLocation)
        {
            case DoorPosition.TOP:
                {
                    newRoomPosition = door.thisRoom.transform.position + new Vector3(0, 10.36f, 0);
                    characterOffset = new Vector3(0, 0.7f, 0);
                    break;
                }

            case DoorPosition.BOTTOM:
                {
                    newRoomPosition = door.thisRoom.transform.position + new Vector3(0, -10.36f, 0);
                    characterOffset = new Vector3(0, -0.7f, 0);
                    break;
                }
            case DoorPosition.LEFT:
                {
                    newRoomPosition = door.thisRoom.transform.position + new Vector3(-15.36f, 0, 0);
                    characterOffset = new Vector3(-0.7f, 0, 0);
                    break;
                }
            case DoorPosition.RIGHT:
                {
                    newRoomPosition = door.thisRoom.transform.position + new Vector3(15.36f, 0, 0);
                    characterOffset = new Vector3(0.7f, 0, 0);
                    break;
                }
            default:
                break;
        }

        #region INSTANTIATE NEW ROOM
        if(door.connectedDoor == null)
        {
            GameObject newRoom = Instantiate(roomPrefab, newRoomPosition, Quaternion.identity, roomsContainer.transform);

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
        }
        #endregion

        #region CAMERA TRANSITION
        PlayerController.Instance.enabled = false;

        PlayerController.Instance.transform.position = door.connectedDoor.transform.position + characterOffset;
        PlayerController.Instance.ResetBodyAnimator();

        //Camera.main.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y, -1);
        StartCoroutine(CameraTranslation(mainCamera, new Vector3(newRoomPosition.x, newRoomPosition.y, -1), 0.1f, () =>
        {
            PlayerController.Instance.enabled = true;
        }));
        #endregion

        currentRoom = door.connectedDoor.thisRoom;
    }

    private IEnumerator CameraTranslation(Camera camera, Vector3 desiredPosition, float time, Action callback)
    {
        Vector3 currentVelocity = Vector3.zero;
        Vector3 initialPos = camera.transform.position;

        while (camera.transform.position != desiredPosition)
        {
            camera.transform.position = Vector3.SmoothDamp(camera.transform.position, desiredPosition, ref currentVelocity, time);
            
            if(camera.transform.position != desiredPosition)
                yield return null;
        }

        callback.Invoke();
    }
}
