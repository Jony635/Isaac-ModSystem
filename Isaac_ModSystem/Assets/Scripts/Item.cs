using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string name = "Default";
    public string pickUpSprite = "Default";

    public virtual void Update() { }

    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnTriggerEnter2D(Collider2D collider) { }

    public virtual void OnActiveButtonPressed() { }

    public virtual void OnItemEquipped() { }

    public virtual void OnItemUnEquipped() { }

    public virtual void OnNewRoomEntered() { }

    public virtual void OnNewRoomCleared() { }
}

public class PassiveItem : Item
{
    
}

public class ActiveItem : Item
{
    
}
