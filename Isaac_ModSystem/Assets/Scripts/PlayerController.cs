using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject head;
    public GameObject body;

    private Animator headCtrl;
    private Animator bodyCtrl;

    private Gamepad gamepad;
    public float stick_threshold = 0.1f;

    private void Awake()
    {
        if(head != null)
        {
            headCtrl = head.GetComponent<Animator>();
        }

        if(body != null)
        {
            bodyCtrl = body.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
        }
        else
        {
            #region SHOOTING
            if(gamepad.buttonEast.isPressed)
            {
                headCtrl.SetBool("shootRight", true);
                headCtrl.SetBool("shootLeft", false);
                headCtrl.SetBool("shootDown", false);
                headCtrl.SetBool("shootUp", false);
            }
            else if (gamepad.buttonSouth.isPressed)
            {
                headCtrl.SetBool("shootRight", false);
                headCtrl.SetBool("shootLeft", false);
                headCtrl.SetBool("shootDown", true);
                headCtrl.SetBool("shootUp", false);
            }
            else if(gamepad.buttonWest.isPressed)
            {
                headCtrl.SetBool("shootRight", false);
                headCtrl.SetBool("shootLeft", true);
                headCtrl.SetBool("shootDown", false);
                headCtrl.SetBool("shootUp", false);
            }
            else if(gamepad.buttonNorth.isPressed)
            {
                headCtrl.SetBool("shootRight", false);
                headCtrl.SetBool("shootLeft", false);
                headCtrl.SetBool("shootDown", false);
                headCtrl.SetBool("shootUp", true);
            }
            else
            {
                headCtrl.SetBool("shootRight", false);
                headCtrl.SetBool("shootLeft", false);
                headCtrl.SetBool("shootDown", false);
                headCtrl.SetBool("shootUp", false);
            }
            #endregion

            #region MOVEMENT
            Vector2 move = gamepad.leftStick.ReadValue();
            if(move.magnitude < stick_threshold)
            {
                bodyCtrl.SetBool("walking_horizontal", false);
                bodyCtrl.SetBool("walking_vertical", false);
            }
            else
            {
                if(Mathf.Abs(move.x) >= Mathf.Abs(move.y))
                {
                    //Move horizontal
                    bodyCtrl.SetBool("walking_horizontal", true);
                    bodyCtrl.SetBool("walking_vertical", false);

                    if(move.x >= 0)
                    {
                        body.transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        body.transform.localScale = new Vector3(-1, 1, 1);
                    }
                }
                else
                {
                    //Move vertical
                    bodyCtrl.SetBool("walking_horizontal", false);
                    bodyCtrl.SetBool("walking_vertical", true);
                }

                //Apply actual movement
            }



            #endregion
        }
    }
}
