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

        ItemHolderRenderer.sprite = (Sprite)AssetDatabase.LoadAssetAtPath<Sprite>(holdedItem.pickUpSprite);
    }
}
