using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public LuaScriptController luaScript = null;

    public string name = "Default";
    public string pickUpSprite = "Default";
    public Sprite sprite = null;

    protected virtual void Awake() { }

    protected virtual void Start() { }

    protected virtual void Update() { }

    protected virtual void FixedUpdate() { }

    protected virtual void LateUpdate() { }

    public virtual void OnCollisionEnter2D(Collision2D collision) { }

    public virtual void OnTriggerEnter2D(Collider2D collider) { }

    public virtual void OnCollisionStay2D(Collision2D collision) { }

    public virtual void OnTriggerStay2D(Collider2D collider) { }

    public virtual void OnCollisionExit2D(Collision2D collision) { }

    public virtual void OnTriggerExit2D(Collider2D collider) { }

    public virtual void OnEnable() { }

    public virtual void OnDisable() { }

    public virtual void OnEquipped() 
    { 
        if(luaScript != null)
        {
            luaScript.enabled = true;
            luaScript.OnEquipped();
        }
    }

    public virtual void OnUnequipped() { }

    public virtual void OnNewRoomEntered(bool alreadyDefeated) { }

    public virtual void OnNewRoomCleared() { }

    public virtual void OnMonsterHittedByTear(Enemy enemy) { if (luaScript) luaScript.OnMonsterHittedByTear(enemy); }
}

public class PassiveItem : Item
{
    
}

public class ActiveItem : Item
{
    public virtual void OnUsed() { }

    public uint numCharges = 1u;
    public uint currentCharges = 1u;

    public bool equiped = false;
}
