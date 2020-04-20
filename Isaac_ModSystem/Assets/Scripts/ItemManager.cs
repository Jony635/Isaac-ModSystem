using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField]
    private GameObject availableItems;
    [SerializeField]
    private GameObject equippedItems;
    [SerializeField]
    private GameObject activeItem;
    [SerializeField]
    private GameObject discardedItems;

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

    private void LoadMods()
    {
        if (Directory.Exists(Application.dataPath + "/Mods/"))
        {
            string[] modFolders = Directory.GetDirectories(Application.dataPath + "/Mods/");
            foreach (string modFolder in modFolders)
            {
                //TODO: Load the resources, create the GameObjects, etc.

                //STEPS:
                // 1. Create 1 Empty gameobject named as the folder.                                        v
                // 2. Add a LuaScriptController Component.                                                  v
                // 3. Look for a main.lua inside the folder.
                // 4. Extract the string stored in the global variable "itemicon"
                // 5. Import at runtime the .png file as a Texture/Sprite
                // 6. Store the Sprite inside the LuaScriptController component.
                // 7. Try to show the icon in the active item icon sprite in order to see the results.
                // 8. Store the created gameobject in its respective place depending of the item.

                string modName = modFolder.Substring(modFolder.LastIndexOf('/') + 1);

                GameObject modGO = new GameObject();
                modGO.name = modName;
                modGO.transform.SetParent(mods.transform);

                LuaScriptController scriptController = modGO.AddComponent<LuaScriptController>();

                if(File.Exists(modFolder + "/main.lua"))
                {
                    scriptController.LuaScriptFile = modFolder + "/main.lua";
                    scriptController.Initialize();


                }

            }
        }      
    }

    public void EquipItem(Item item, ItemAltar altar = null)
    {
        if (item.GetType().IsSubclassOf(typeof(ActiveItem)))
        {
            if(activeItem.transform.childCount > 0)
            {
                Transform equipedItem = activeItem.transform.GetChild(0);
                equipedItem.SetParent(discardedItems.transform);

                altar.ChangeHoldedItem(equipedItem.GetComponent<Item>());
            }

            item.transform.SetParent(activeItem.transform);

            ActiveItemContainer.Instance.ActiveItemEquipped((ActiveItem)item);
        }

        else if (item.transform.parent == availableItems.transform)
        {
            item.transform.SetParent(equippedItems.transform);
        }   
        
        item.OnItemEquipped();
    }
}
