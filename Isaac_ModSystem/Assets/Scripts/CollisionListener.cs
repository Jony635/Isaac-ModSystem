using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionListener : MonoBehaviour
{
    [System.Serializable]
    public class CollisionEvent : UnityEvent<Collision2D> { }

    [System.Serializable]
    public class ColliderEvent : UnityEvent<Collider2D> { }

    public CollisionEvent OnCollisionEnter = new CollisionEvent();
    public CollisionEvent OnCollisionStay = new CollisionEvent();
    public CollisionEvent OnCollisionExit = new CollisionEvent();

    public ColliderEvent OnTriggerEnter = new ColliderEvent();
    public ColliderEvent OnTriggerStay = new ColliderEvent();
    public ColliderEvent OnTriggerExit = new ColliderEvent();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter.Invoke(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay.Invoke(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerStay.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnTriggerExit.Invoke(collision);
    }
}
