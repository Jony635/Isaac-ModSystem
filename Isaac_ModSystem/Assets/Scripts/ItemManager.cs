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
        if (availableItems.transform.childCount <= 0)
            return null;

        int random = UnityEngine.Random.Range(0, availableItems.transform.childCount);
        Item ret = availableItems.transform.GetChild(random).GetComponent<Item>();
        if(ret.gameObject.activeSelf)
        {
            ret.gameObject.SetActive(false);
            return ret;
        }
        
        return null;
    }

    private void InstantiateAllAvailableItems()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsSubclassOf(typeof(Item)))
            {
                if (type != typeof(ActiveItem) && type != typeof(PassiveItem))
                {
                    GameObject newGO = new GameObject();
                    newGO.name = type.ToString();
                    newGO.transform.SetParent(availableItems.transform);
                    newGO.AddComponent(type);
                }
            }
        }
    }

    public void EquipItem(Item item)
    {
        if(item.transform.parent == availableItems.transform)
        {
            item.transform.SetParent(equippedItems.transform);
            item.OnItemEquipped();
        }        
    }
}
