﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        public float plainDamage;
        public float factorDamage;

        public float speed;
        public float hp;
        public float maxHp;

        public bool invincible;

        public Stats(string useless)
        {
            plainDamage = 5f;
            factorDamage = 1f;
            speed = 7f;
            maxHp = hp = 6f;

            invincible = false;
        }
    }

    public static PlayerController Instance = null;
 
    public GameObject head;
    public GameObject body;

    [HideInInspector]
    public Animator headCtrl;
    [HideInInspector]
    public Animator bodyCtrl;

    private CircleCollider2D col = null;

    private Gamepad gamepad;
    public float stick_threshold = 0.1f;

    public Transform[] tearPositions;
    public GameObject tearPrefab;
    public float tearImpulse = 15f;
    public float tearMaxInertia = 0.4f;

    public GameObject tearsContainer;
    public SpriteRenderer pickedItemRenderer;

    private Rigidbody2D rb;

    private Vector2 move = Vector2.zero;

    private bool detectPickUp = true;

    private bool detectControls = true;

    private float takeDamageCD = 0.5f;
    private bool takeDamage = true;

    private AudioSource audioSource;

    [HideInInspector]
    public float difficulty = 1f;

    [Header("Character Stats")] [SerializeField]
    private Stats _stats = new Stats("");

    public Stats stats { get { return _stats; } set { ModifyStats(value); _stats = value; } }

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
        col = GetComponent<CircleCollider2D>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        #region CONTROLS
        if (detectControls)
        {
            if (gamepad == null)
            {
                gamepad = Gamepad.current;

                #region SHOOTING
                if (Keyboard.current.rightArrowKey.isPressed)
                {
                    headCtrl.SetBool("shootRight", true);
                    headCtrl.SetBool("shootLeft", false);
                    headCtrl.SetBool("shootDown", false);
                    headCtrl.SetBool("shootUp", false);
                }
                else if (Keyboard.current.downArrowKey.isPressed)
                {
                    headCtrl.SetBool("shootRight", false);
                    headCtrl.SetBool("shootLeft", false);
                    headCtrl.SetBool("shootDown", true);
                    headCtrl.SetBool("shootUp", false);
                }
                else if (Keyboard.current.leftArrowKey.isPressed)
                {
                    headCtrl.SetBool("shootRight", false);
                    headCtrl.SetBool("shootLeft", true);
                    headCtrl.SetBool("shootDown", false);
                    headCtrl.SetBool("shootUp", false);
                }
                else if (Keyboard.current.upArrowKey.isPressed)
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

                move = Vector2.zero;

                if(Keyboard.current.aKey.isPressed)
                    move.x -= 1f;

                if (Keyboard.current.dKey.isPressed)
                    move.x += 1f;

                if (Keyboard.current.wKey.isPressed)
                    move.y += 1f;

                if (Keyboard.current.sKey.isPressed)
                    move.y -= 1f;

                if(move == Vector2.zero)
                {
                    bodyCtrl.SetBool("walking_horizontal", false);
                    bodyCtrl.SetBool("walking_upwards", false);
                    bodyCtrl.SetBool("walking_downwards", false);
                }
                else
                {
                    if (Mathf.Abs(move.x) >= Mathf.Abs(move.y))
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

                #region ACTIVE ITEM USED
                if (Keyboard.current.qKey.wasPressedThisFrame)
                    ItemManager.Instance.OnActiveItemButtonPressed();
                #endregion
            }
            else
            {
                #region SHOOTING
                if (gamepad.buttonEast.isPressed)
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
                else if (gamepad.buttonWest.isPressed)
                {
                    headCtrl.SetBool("shootRight", false);
                    headCtrl.SetBool("shootLeft", true);
                    headCtrl.SetBool("shootDown", false);
                    headCtrl.SetBool("shootUp", false);
                }
                else if (gamepad.buttonNorth.isPressed)
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
                if (move.magnitude < stick_threshold)
                {
                    bodyCtrl.SetBool("walking_horizontal", false);

                    bodyCtrl.SetBool("walking_upwards", false);
                    bodyCtrl.SetBool("walking_downwards", false);
                }
                else
                {
                    if (Mathf.Abs(move.x) >= Mathf.Abs(move.y))
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

                #region ACTIVE ITEM USED
                if(gamepad.leftTrigger.wasPressedThisFrame)
                    ItemManager.Instance.OnActiveItemButtonPressed();
                #endregion
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //Apply actual movement
        rb.MovePosition((Vector2)(transform.position) + (move * stats.speed * Time.fixedDeltaTime));
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {      
        Door door = collider.GetComponentInParent<Door>();
        if(door)
            RoomManager.Instance.DoorTrespassed(door);      
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        #region PICK UP ITEMS
        if (collision.gameObject.layer == LayerMask.NameToLayer("ItemAltar"))
        {
            ItemAltar altar = collision.gameObject.GetComponent<ItemAltar>();
            
            if (detectPickUp && altar.holdedItem != null)
            {
                detectPickUp = false;

                head.SetActive(false);
                bodyCtrl.SetTrigger("ItemPickup");

                pickedItemRenderer.sprite = altar.ItemHolderRenderer.sprite;

                ItemManager.Instance.EquipItem(altar.holdedItem, altar);

                int rand = UnityEngine.Random.Range(0, FXReferences.Instance.itemPickup.Length);
                audioSource.clip = FXReferences.Instance.itemPickup[rand];
                audioSource.Play();
            }
        }
        #endregion

        #region TAKE DAMAGE
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Monster") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                if(!stats.invincible && takeDamage)
                {
                    StartCoroutine(TakeDamage(enemy.enemyStats.damage));
                }

                ItemManager.Instance.OnCharacterCollidedWithMonster(enemy);
            }

            rb.velocity = Vector2.zero;
        }
        #endregion


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        #region TAKE DAMAGE
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster") || collision.gameObject.layer == LayerMask.NameToLayer("EnemyAttack"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!stats.invincible && takeDamage)
                {
                    StartCoroutine(TakeDamage(enemy.enemyStats.damage));
                }

                ItemManager.Instance.OnCharacterCollidingWithMonster(enemy);
            }
        }

        rb.velocity = Vector2.zero;

        #endregion
    }

    public IEnumerator TakeDamage(float damage)
    {
        if(takeDamage)
        {
            takeDamage = false;

            Stats newStats = stats;
            newStats.hp -= damage;
            stats = newStats;

            if (stats.hp <= 0)
            {
                StartCoroutine(Die());
            }
            else
            {
                head.SetActive(false);
                bodyCtrl.SetTrigger("Damaged");

                int rand = UnityEngine.Random.Range(0, FXReferences.Instance.playerHurtFX.Length);
                audioSource.clip = FXReferences.Instance.playerHurtFX[rand];
                audioSource.Play();
            }

            HeartsManager.Instance.OnCharacterHealthChanged();

            yield return new WaitForSeconds(takeDamageCD);
            takeDamage = true;
        }
    }

    private void ModifyStats(Stats newStats)
    {
        int changes = 0;

        if(stats.factorDamage != newStats.factorDamage)
        {
            changes |= 1 << 0;
        }

        if(stats.hp != newStats.hp)
        {
            changes |= 1 << 1;
        }

        if(stats.maxHp != newStats.maxHp)
        {
            changes |= 1 << 2;
        }

        if(stats.plainDamage != newStats.plainDamage)
        {
            changes |= 1 << 3;
        }

        if(stats.speed != newStats.speed)
        {
            changes |= 1 << 4;
        }

        if(stats.invincible)
        {
            changes |= 1 << 5;
        }

        _stats = newStats;

        if((changes >> 0 & 1) == 1)
        {
            //Factor damage changed
        }

        if ((changes >> 1 & 1) == 1)
        {
            //Hp changed
            HeartsManager.Instance.OnCharacterHealthChanged();
        }

        if ((changes >> 2 & 1) == 1)
        {
            //Max hp changed
            HeartsManager.Instance.OnCharacterMaxHealthChanged();
        }

        if ((changes >> 3 & 1) == 1)
        {
            //Plain damage changed
        }

        if ((changes >> 4 & 1) == 1)
        {
            //Speed changed
        }

        if ((changes >> 5 & 1) == 1)
        {
            //Invincible changed
        }
    }

    private IEnumerator Die()
    {
        move = Vector2.zero;

        detectControls = false;

        head.SetActive(false);
        bodyCtrl.SetTrigger("Death");

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        int rand = UnityEngine.Random.Range(0, FXReferences.Instance.playerDeadFX.Length);
        audioSource.clip = FXReferences.Instance.playerDeadFX[rand];
        audioSource.Play();

        yield return null;
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
        tear.GetComponent<Rigidbody2D>().AddForce(direction * tearImpulse + inertia * stats.speed, ForceMode2D.Impulse);
        tear.transform.SetParent(tearsContainer.transform);

        ItemManager.Instance.OnPlayerShoot(direction * tearImpulse + inertia * stats.speed);
    }

    public void ResetHeadAnimator()
    {
        headCtrl.SetBool("shootRight", false);
        headCtrl.SetBool("shootLeft", false);
        headCtrl.SetBool("shootDown", false);
        headCtrl.SetBool("shootUp", false);
        headCtrl.SetBool("facingRight", false);
        headCtrl.SetBool("facingLeft", false);
        headCtrl.SetBool("facingDown", false);
        headCtrl.SetBool("facingUp", false);
        head.SetActive(true);
    }

    public void ResetBodyAnimator()
    {
        bodyCtrl.SetBool("walking_upwards", false);
        bodyCtrl.SetBool("walking_downwards", false);
        bodyCtrl.SetBool("walking_horizontal", false);
        body.SetActive(true);
    }

    public void ResetAnimators(bool isDeath)
    {
        ResetHeadAnimator();
        ResetBodyAnimator();

        detectPickUp = true;
        pickedItemRenderer.sprite = null;

        col.enabled = true;

        if (isDeath)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
