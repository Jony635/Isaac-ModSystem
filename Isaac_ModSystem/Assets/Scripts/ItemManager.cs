using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField]
    private GameObject availableItems;
    [SerializeField]
    private GameObject equippedItems;

    private void Awake()
    {
        Instance = this;
        InstantiateAllAvailableItems();
    }

    public Item GetRandomAvailableItem()
    {
        return availableItems.transform.GetChild(UnityEngine.Random.Range(0, availableItems.transform.childCount)).GetComponent<Item>();
    }

    private void InstantiateAllAvailableItems()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsSubclassOf(typeof(Item)))
            {
                if (type != typeof(ActiveItem) && type != typeof(PassiveItem))
                {
                    GameObject newGO = Instantiate(new GameObject());
                    newGO.name = type.ToString();
                    newGO.transform.SetParent(availableItems.transform);
                    newGO.AddComponent(type);
                }
            }
        }
    }
}
