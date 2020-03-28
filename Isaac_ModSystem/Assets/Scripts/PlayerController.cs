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

    public static PlayerController Instance = null;

    public Transform[] tearPositions;
    public GameObject tearPrefab;
    public float tearImpulse = 15f;
    public float tearMaxInertia = 0.4f;

    public float moveSpeed = 30f;

    public GameObject tearsContainer;

    private Rigidbody2D rb;

    private Vector2 move = Vector2.zero;

    private void Awake()
    {
        Instance = this;

        if(head != null)
        {
            headCtrl = head.GetComponent<Animator>();
        }

        if(body != null)
        {
            bodyCtrl = body.GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();
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
            move = gamepad.leftStick.ReadValue();
            if(move.magnitude < stick_threshold)
            {
                bodyCtrl.SetBool("walking_horizontal", false);

                bodyCtrl.SetBool("walking_upwards", false);
                bodyCtrl.SetBool("walking_downwards", false);
            }
            else
            {
                if(Mathf.Abs(move.x) >= Mathf.Abs(move.y))
                {
                    //Move horizontal
                    bodyCtrl.SetBool("walking_horizontal", true);

                    bodyCtrl.SetBool("walking_upwards", false);
                    bodyCtrl.SetBool("walking_downwards", false);

                    if (move.x >= 0)
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

                    bodyCtrl.SetBool("walking_upwards", move.y > 0 ? true : false);
                    bodyCtrl.SetBool("walking_downwards", move.y > 0 ? false : true);                                   
                }
                
                //Movement is being applied on FixedUpdate
            }

            #endregion
        }
    }

    private void FixedUpdate()
    {
        //Apply actual movement
        rb.MovePosition((Vector2)(transform.position) + (move * moveSpeed * Time.fixedDeltaTime));
    }

    public void OnShootEvent()
    {
        Vector2 direction = Vector2.zero;
        int index = -1;

        AnimatorStateInfo info = headCtrl.GetCurrentAnimatorStateInfo(0);

        if(info.IsName("Isaac_Shoot_Front"))
        {
            //Debug.Log("Shooting Bottom");
            direction = new Vector2(0, -1);
            index = 2;
        }
        if (info.IsName("Isaac_Shoot_Right"))
        {
            //Debug.Log("Shooting Right");
            direction = new Vector2(1, 0);
            index = 1;
        }
        if (info.IsName("Isaac_Shoot_Left"))
        {
            //Debug.Log("Shooting Left");
            direction = new Vector2(-1, 0);
            index = 0;
        }
        if (info.IsName("Isaac_Shoot_Back"))
        {
            //Debug.Log("Shooting Up");
            direction = new Vector2(0, 1);
            index = 3;
        }

        Vector2 inertia = Vector2.zero;

        if (Mathf.Abs(direction.y) > 0) //If shooting vertical
        {
            if (Mathf.Abs(move.x) > stick_threshold) //If moving horizontal
            {
                //Apply inertia
                inertia.x = move.x * tearMaxInertia;
            }        
        }  
        else if (Mathf.Abs(direction.x) > 0) //If shooting horizontal
        {
            if (Mathf.Abs(move.y) > stick_threshold) //If moving vertical
            {
                //Apply inertia
                inertia.y = move.y * tearMaxInertia;
            }           
        }
       
        GameObject tear = Instantiate(tearPrefab, tearPositions[index]);
        tear.GetComponent<Rigidbody2D>().AddForce(direction * tearImpulse + inertia * moveSpeed, ForceMode2D.Impulse);
        tear.transform.SetParent(tearsContainer.transform);
    }
}
