﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLua;
using System.IO;

using UnityEngine.InputSystem;

using UnityEngine.UI;

public class LuaScriptController : MonoBehaviour 
{
    const bool GEN_LIB = false;

	private ILuaState 	Lua;

    [HideInInspector]
    public bool isMain = false;

    [HideInInspector]
    public string basePath = null;

    [HideInInspector]
    public string LuaScriptFile = null;

    [HideInInspector]
    public Sprite sprite = null;

    private GameObject passiveItems = null;
    private GameObject activeItems = null;
    private GameObject monsters = null;

    #region LUA FUNCTION REFERENCES
    private int			AwakeRef = -1;
	private int			StartRef = -1;
	private int			UpdateRef = -1;
	private int			FixedUpdateRef = -1;
	private int			LateUpdateRef = -1;
    private int         OnCollisionEnter2DRef = -1;
    private int         OnTriggerEnter2DRef = -1;
    private int         OnCollisionStay2DRef = -1;
    private int         OnTriggerStay2DRef = -1;
    private int         OnCollisionExit2DRef = -1;
    private int         OnTriggerExit2DRef = -1;
    private int         OnEnableRef = -1;
    private int         OnDisableRef = -1;
    private int         OnEquippedRef = -1;
    #endregion 

    //C# Functions container
    private NameFuncPair[] lib;

    private Dictionary<UInt32, GameObject> childs = new Dictionary<uint, GameObject>();

    public void Initialize()
    {
        if (Lua == null)
        {
            Lua = LuaAPI.NewState();

            Lua.L_OpenLibs();

            #region Push C# Functions to Lua

            lib = new NameFuncPair[]
            {
                new NameFuncPair("AddDamage", AddDamage),
                new NameFuncPair("SubstractDamage", SubstractDamage),
                new NameFuncPair("GetPlainDamage", GetPlainDamage),
                new NameFuncPair("AddFactorDamage", AddFactorDamage),
                new NameFuncPair("SubstractFactorDamage", SubstractFactorDamage),
                new NameFuncPair("GetFactorDamage", GetFactorDamage),
                new NameFuncPair("GetDamage", GetDamage),
                new NameFuncPair("AddChild", AddChild),
                new NameFuncPair("DeleteChild", DeleteChild),
                new NameFuncPair("Wait", Wait),
                new NameFuncPair("GetDT", GetDT),
                new NameFuncPair("GetPosition", GetPosition),
                new NameFuncPair("SetPosition", SetPosition),
                new NameFuncPair("SetRotation", SetRotation),
                new NameFuncPair("SetComponent", SetComponent),
            };

            childs.Add(0, PlayerController.Instance.gameObject);
            
            if (GEN_LIB)
                Lua.L_RequireF("ModSystem", OpenModSystemLib, false);
            else
                PushCSharpFunctions();

            #endregion

            var status = Lua.L_DoFile(LuaScriptFile);
            if (status != ThreadStatus.LUA_OK)
            {
                throw new Exception(Lua.ToString(-1));
            }

            #region Get Lua Function References
            AwakeRef = StoreMethod("Awake");
            StartRef = StoreMethod("Start");
            UpdateRef = StoreMethod("Update");
            LateUpdateRef = StoreMethod("LateUpdate");
            FixedUpdateRef = StoreMethod("FixedUpdate");

            OnEquippedRef = StoreMethod("OnEquipped");
            #endregion         

            Lua.Pop(-1);

            string spritePath = GetGlobalString("sprite");
            if(spritePath != null)
                sprite = ImportSprite(basePath + "/" + spritePath);

            if(isMain)
            {           
                passiveItems = new GameObject();
                passiveItems.name = "PassiveItems";
                passiveItems.transform.SetParent(transform);

                activeItems = new GameObject();
                activeItems.name = "ActiveItems";
                activeItems.transform.SetParent(transform);

                monsters = new GameObject();
                monsters.name = "Monsters";
                monsters.transform.SetParent(transform);

                LoadItemsAndMonsters();
            }           
        }
    }

    #region C#->Lua FUNCTIONS
    private void Awake() 
    {
		CallMethod( AwakeRef );
	}

    private void Start() 
    {
        CallMethod( StartRef );
    }

    private void Update() 
    {
        CallMethod( UpdateRef );
	}

    private void FixedUpdate()
    {
        CallMethod(FixedUpdateRef);
    }

    private void LateUpdate() 
    {
		CallMethod( LateUpdateRef );
	}

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        //CallMethod(OnCollisionEnter2DRef); 
    }

    private void OnTriggerEnter2D(Collider2D collider) { }

    private void OnCollisionStay2D(Collision2D collision) { }

    private void OnTriggerStay2D(Collider2D collider) { }

    private void OnCollisionExit2D(Collision2D collision) { }

    private void OnTriggerExit2D(Collider2D collider) { }

    public void OnEnable() { }

    public void OnDisable() { }

    public void OnEquipped()
    {
        CallMethod(OnEquippedRef);
    }
    #endregion

    #region Lua->C# FUNCTIONS
    private int AddDamage(ILuaState lua)
    {
        float sum = (float)lua.L_CheckNumber(1);
        PlayerController.Instance.stats.plainDamage += sum;
        return 0;
    }

    private int SubstractDamage(ILuaState lua)
    {
        float sub = (float)lua.L_CheckNumber(1);
        PlayerController.Instance.stats.plainDamage -= sub;
        return 0;
    }

    private int AddFactorDamage(ILuaState lua)
    {
        float sum = (float)lua.L_CheckNumber(1);
        PlayerController.Instance.stats.factorDamage += sum;
        return 0;
    }

    private int SubstractFactorDamage(ILuaState lua)
    {
        float sub = (float)lua.L_CheckNumber(1);
        PlayerController.Instance.stats.factorDamage -= sub;
        return 0;
    }

    private int GetPlainDamage(ILuaState lua)
    {
        lua.PushNumber(PlayerController.Instance.stats.plainDamage);
        return 1;
    }

    private int GetFactorDamage(ILuaState lua)
    {
        lua.PushNumber(PlayerController.Instance.stats.factorDamage);
        return 1;
    }

    private int GetDamage(ILuaState lua)
    {
        lua.PushNumber(PlayerController.Instance.stats.plainDamage * PlayerController.Instance.stats.factorDamage);
        return 1;
    }

    private int AddChild(ILuaState lua)
    {
        GameObject newChild = new GameObject();
        newChild.transform.SetParent(transform);

        uint rand = 0;
        do
        {
            rand = (uint)(UnityEngine.Random.value * uint.MaxValue);

        } while(childs.ContainsKey(rand) || rand == 0);

        childs.Add(rand, newChild);

        lua.PushUnsigned(rand);

        return 1;
    }

    private int DeleteChild(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if(childs.ContainsKey(key) && key != 0)
        {
            GameObject destroyed = childs[key];
            Destroy(destroyed);
            childs.Remove(key);
        }

        return 0;
    }

    private int Wait(ILuaState lua)
    {
        StartCoroutine(WaitCoroutine(lua));
        return 0;
    }

    IEnumerator WaitCoroutine(ILuaState lua)
    {
        ILuaState luaThread = lua.NewThread();
        lua.PushValue(2);
        lua.XMove(luaThread, 1);

        yield return new WaitForSeconds((float)lua.L_CheckNumber(1));

        luaThread.Resume(null, 0);      
    }

    private int GetDT(ILuaState lua)
    {
        lua.PushNumber(Time.deltaTime);
        return 1;
    }

    private int GetPosition(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if(childs.ContainsKey(key))
        {
            GameObject child = childs[key];

            lua.NewTable();

            lua.PushString("x");
            lua.PushNumber(child.transform.position.x);
            lua.SetTable(-3);

            lua.PushString("y");
            lua.PushNumber(child.transform.position.y);
            lua.SetTable(-3);
        }     

        return 1;
    }

    private int SetPosition(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if(childs.ContainsKey(key))
        {
            GameObject child = childs[key];

            Vector2 newPosition = new Vector2(0, 0);

            if(lua.IsTable(2))
            {
                lua.Insert(2);

                lua.PushString("x");
                lua.GetTable(-2);
                double x = lua.L_CheckNumber(-1);
                lua.Pop(1);

                lua.PushString("y");
                lua.GetTable(-2);
                double y = lua.L_CheckNumber(-1);
                lua.Pop(1);

                newPosition = new Vector2((float)x, (float)y);
            }

            child.transform.position = newPosition;
        }

        return 0;
    }

    private int SetRotation(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if(childs.ContainsKey(key))
        {
            GameObject child = childs[key];
            float angle = (float)lua.L_CheckNumber(2);

            child.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }     

        return 0;
    }

    private int SetComponent(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if(childs.ContainsKey(key))
        {
            GameObject child = childs[key];
                        switch(lua.L_CheckString(2))
            {
                case "SpriteRenderer":

                    if (lua.IsTable(3))
                    {
                        lua.Insert(3);

                        lua.PushString("sprite");
                        lua.GetTable(-2);
                        if (!lua.IsNoneOrNil(-1))
                        {
                            int spriteIndex = lua.L_CheckInteger(-1);

                            SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                            if (renderer == null)
                                renderer = child.AddComponent<SpriteRenderer>();

                            renderer.sortingOrder = 10;
                            renderer.sprite = spriteIndex == 0 ? sprite : null; //TODO: extraSprites[spriteIndex - 1]
                        }
                       
                        lua.Pop(1);
                    }
                    break;
            }

        }
        return 0;
    }

    #endregion

    #region Utils
    private string GetGlobalString(string name)
    {
        string ret = null;
        
        Lua.GetGlobal(name);
        if(!Lua.IsNoneOrNil(-1))
        {
            ret = Lua.L_CheckString(-1);
        }           
        Lua.Pop(1);

        return ret;
    }

	private int StoreMethod( string name )
	{
        try
        {
		    Lua.GetField(-1, name);
        }
        catch
        {
            return -1;
        }

		if( !Lua.IsFunction( -1 ) )
		{
			//throw new Exception( string.Format(
			//	"method {0} not found!", name ) );
		}

		return Lua.L_Ref( LuaDef.LUA_REGISTRYINDEX );
	}

	private void CallMethod( int funcRef, int numArguments = 0, int numResults = 0, params object[] arguments)
	{
        if (funcRef == -1) return;

        Lua.RawGetI( LuaDef.LUA_REGISTRYINDEX, funcRef );

        // insert `traceback' function
        var b = Lua.GetTop();
        Lua.PushCSharpFunction(Traceback);
        Lua.Insert(b);

        for (int i = 0; i < numArguments; ++i)
        {
            object argument = arguments[i];
            Type type = argument.GetType();

            if (type == typeof(bool))
            {
                Lua.PushBoolean((bool)argument);
            }
            else if (type == typeof(string))
            {
                Lua.PushString((string)argument);
            }
            else if (type == typeof(float) || type == typeof(double))
            {
                Lua.PushNumber(type == typeof(float) ? (float)argument : (double)argument);
            }
            else if (type == typeof(int))
            {
                Lua.PushInteger((int)argument);
            }
        }

        var status = Lua.PCall( numArguments, numResults, b );
		if( status != ThreadStatus.LUA_OK )
		{
			Debug.LogError( Lua.ToString(-1) );
            Lua.Pop(1);
		}

        // remove `traceback' function
        Lua.Remove(b);
    }

	private static int Traceback(ILuaState lua) 
    {
        var msg = lua.ToString(1);
        if (msg != null)
        {
            lua.L_Traceback(lua, msg, 1);
        }
        // is there an error object?
        else if (!lua.IsNoneOrNil(1))
        {
            // try its `tostring' metamethod
            if (!lua.L_CallMeta(1, "__tostring"))
            {
                lua.PushString("(no error message)");
            }
        }

        return 1;
	}

    private int OpenModSystemLib(ILuaState lua)
    {        
        lua.L_NewLib(lib);
        return 1;
    }

    private void PushCSharpFunctions()
    {
        foreach(NameFuncPair pair in lib)
        {
            Lua.PushCSharpFunction(pair.Func);
            Lua.SetGlobal(pair.Name);
        }
    }
    #endregion

    private void LoadItemsAndMonsters()
    {
        for(int step = 0; step < 3; ++step)
        {
            List<string> elements = new List<string>();

            switch(step)
            {
                case 0:
                    Lua.GetGlobal("passiveItems");
                    break;
                case 1:
                    Lua.GetGlobal("activeItems");
                    break;
                case 2:
                    Lua.GetGlobal("monsters");
                    break;
            }
            
            if (Lua.IsNoneOrNil(-1))
            {
                Lua.Pop(1);
                continue;
            }

            for (int i = 1; true; ++i)
            {
                Lua.RawGetI(-1, i);
                if (Lua.IsNoneOrNil(-1))
                {
                    Lua.Pop(2);
                    break;
                }

                elements.Add(Lua.L_CheckString(-1));
                Lua.Pop(1);
            }

            foreach (string item in elements)
            {
                string path = basePath + "/" + item;
                if (File.Exists(path))
                {
                    string name = item.Substring(item.LastIndexOf('/') + 1);
                    name = name.Remove(name.IndexOf('.'));

                    GameObject element = new GameObject();
                    element.name = name;

                    LuaScriptController controller = element.AddComponent<LuaScriptController>();
                    controller.enabled = false;
                    controller.LuaScriptFile = path;
                    controller.basePath = path.Substring(0, path.LastIndexOf('/'));
                    controller.Initialize();

                    switch (step)
                    {
                        case 0:
                            element.transform.SetParent(passiveItems.transform);
                            PassiveItem pItem = element.AddComponent<PassiveItem>();
                            pItem.luaScript = controller;
                            pItem.name = name;
                            pItem.sprite = controller.sprite;
                            ItemManager.Instance.AddItem(pItem);                          
                            break;
                        case 1:
                            element.transform.SetParent(activeItems.transform);
                            ActiveItem aItem = element.AddComponent<ActiveItem>();
                            aItem.luaScript = controller;
                            aItem.sprite = controller.sprite;
                            ItemManager.Instance.AddItem(aItem);
                            break;
                        case 2:
                            element.transform.SetParent(monsters.transform);
                            //element.AddComponent<Monster>().luaScript = controller;
                            break;
                    }               
                }
            }
        }    
    }

    public static Sprite ImportSprite(string path)
    {
        byte[] textureBytes = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(textureBytes);

        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 30);
    } 
}