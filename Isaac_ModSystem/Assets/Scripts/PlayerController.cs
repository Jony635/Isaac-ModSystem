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

    private void Awake()
    {
        if(head != null)
        {
            headCtrl = head.GetComponent<Animator>();
        }

        if(bodyCtrl != null)
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
        }
    }
}
