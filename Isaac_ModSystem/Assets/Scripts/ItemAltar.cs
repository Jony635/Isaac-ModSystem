using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ItemAltar : MonoBehaviour
{
    [HideInInspector]
    public Item holdedItem;

    public SpriteRenderer ItemHolderRenderer;

    private void Awake()
    {
        holdedItem = ItemManager.Instance.GetRandomAvailableItem();

        if(holdedItem == null)
        {
            gameObject.SetActive(false);
            return;
        }

        ItemHolderRenderer.sprite = Resources.Load<Sprite>(holdedItem.pickUpSprite);
    }

    public void ChangeHoldedItem(Item item)
    {
        holdedItem = item;
        ItemHolderRenderer.sprite = holdedItem != null ? Resources.Load<Sprite>(holdedItem.pickUpSprite) : null;
    }
}
