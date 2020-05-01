using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField]
    private GameObject itemsGO;

    private List<Item> availableItems = new List<Item>();
    private List<Item> equippedItems = new List<Item>();
    private List<ActiveItem> discardedItems = new List<ActiveItem>();

    private ActiveItem activeItemEquipped = null;

    [SerializeField]
    private GameObject mods;

    private void Awake()
    {
        Instance = this;
        InstantiateAllAvailableItems();
        LoadMods();
    }

    public Item GetRandomAvailableItem()
    {
        if (availableItems.Count <= 0)
            return null;

        int random = UnityEngine.Random.Range(0, availableItems.Count);
        return availableItems[random];
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
                    newGO.transform.SetParent(itemsGO.transform);

                    Item item = (Item)newGO.AddComponent(type);
                    item.sprite = Resources.Load<Sprite>(item.pickUpSprite);
                    availableItems.Add(item);
                }
            }
        }
    }

    private void LoadMods()
    {
        if (Directory.Exists(Application.dataPath + "/Mods/"))
        {
            string[] modFolders = Directory.GetDirectories(Application.dataPath + "/Mods/");
            foreach (string modFolder in modFolders)
            {
                string modName = modFolder.Substring(modFolder.LastIndexOf('/') + 1);

                if (File.Exists(modFolder + "/main.lua"))
                {
                    GameObject modGO = new GameObject();
                    modGO.name = modName;
                    modGO.transform.SetParent(mods.transform);

                    LuaScriptController scriptController = modGO.AddComponent<LuaScriptController>();
                    scriptController.basePath = modFolder;
                    scriptController.LuaScriptFile = modFolder + "/main.lua";
                    scriptController.isMain = true;
                    scriptController.Initialize();
                }           
            }
        }      
    }

    //Events
    public void EquipItem(Item item, ItemAltar altar = null)
    {
        if (item.GetType() == typeof(ActiveItem) || item.GetType().IsSubclassOf(typeof(ActiveItem)))
        {
            if(activeItemEquipped != null)
            {
                if(discardedItems.Contains((ActiveItem)item))               
                    discardedItems.Remove((ActiveItem)item);               

                discardedItems.Add(activeItemEquipped);
                altar.ChangeHoldedItem(activeItemEquipped);
            }

            activeItemEquipped = (ActiveItem)item;
            ActiveItemContainer.Instance.ActiveItemEquipped((ActiveItem)item);

            
        }
        else if (availableItems.Contains(item))
        {
            equippedItems.Add(item);
        }

        if (altar.holdedItem == item)
            altar.ChangeHoldedItem(null);

        availableItems.Remove(item);
        item.OnEquipped();
    }

    public void OnNewRoomCleared()
    {
        foreach(Item item in equippedItems)
        {
            item.OnNewRoomCleared();
        }

        if (activeItemEquipped)
        {
            ActiveItemContainer.Instance.OnNewRoomCleared();
            activeItemEquipped.OnNewRoomCleared();
        }
    }

    public void OnNewRoomEntered(bool alreadyDefeated = false)
    {
        foreach(Item item in equippedItems)
        {
            item.OnNewRoomEntered(alreadyDefeated);
        }

        if(activeItemEquipped != null)
        {
            activeItemEquipped.OnNewRoomEntered(alreadyDefeated);
        }
    }

    public void OnMonsterDied()
    {

    }

    public void OnActiveItemButtonPressed()
    {
        if(activeItemEquipped != null)
        {
            ActiveItemContainer.Instance.ActiveItemUsed();          
        }
    }

    public void OnMonsterHittedByTear(Enemy enemy)
    {
        foreach(Item item in equippedItems)
        {
            item.OnMonsterHittedByTear(enemy);
        }

        if(activeItemEquipped)
            activeItemEquipped.OnMonsterHittedByTear(enemy);
    }

    public void AddItem(Item item)
    {
        availableItems.Add(item);
    }
}
