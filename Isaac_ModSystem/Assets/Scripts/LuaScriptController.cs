﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLua;
using System.IO;

using UnityEngine.UI;

public class LuaScriptController : MonoBehaviour 
{
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

    private int			AwakeRef = -1;
	private int			StartRef = -1;
	private int			UpdateRef = -1;
	private int			LateUpdateRef = -1;
	private int			FixedUpdateRef = -1;

    public void Initialize()
    {
        if (Lua == null)
        {
            Lua = LuaAPI.NewState();
            Lua.L_OpenLibs();

            var status = Lua.L_DoFile(LuaScriptFile);
            if (status != ThreadStatus.LUA_OK)
            {
                throw new Exception(Lua.ToString(-1));
            }

            AwakeRef = StoreMethod("Awake");
            StartRef = StoreMethod("Start");
            UpdateRef = StoreMethod("Update");
            LateUpdateRef = StoreMethod("LateUpdate");
            FixedUpdateRef = StoreMethod("FixedUpdate");

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

    private void LateUpdate() 
    {
		CallMethod( LateUpdateRef );
	}

    private void FixedUpdate() 
    {
		CallMethod( FixedUpdateRef );
	}

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
		    Lua.GetField( -1, name );
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

	private void CallMethod( int funcRef )
	{
        if (funcRef == -1) return;

		Lua.RawGetI( LuaDef.LUA_REGISTRYINDEX, funcRef );

		// insert `traceback' function
		var b = Lua.GetTop();
		Lua.PushCSharpFunction( Traceback );
		Lua.Insert(b);

		var status = Lua.PCall( 0, 0, b );
		if( status != ThreadStatus.LUA_OK )
		{
			Debug.LogError( Lua.ToString(-1) );
		}

		// remove `traceback' function
		Lua.Remove(b);
	}

	private static int Traceback(ILuaState lua) {
		var msg = lua.ToString(1);
		if(msg != null) {
			lua.L_Traceback(lua, msg, 1);
		}
		// is there an error object?
		else if(!lua.IsNoneOrNil(1)) {
			// try its `tostring' metamethod
			if(!lua.L_CallMeta(1, "__tostring")) {
				lua.PushString("(no error message)");
			}
		}
		return 1;
	}

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

                    switch (step)
                    {
                        case 0:
                            element.transform.SetParent(passiveItems.transform);
                            break;
                        case 1:
                            element.transform.SetParent(activeItems.transform);
                            break;
                        case 2:
                            element.transform.SetParent(monsters.transform);
                            break;
                    }
                  
                    LuaScriptController controller = element.AddComponent<LuaScriptController>();
                    controller.LuaScriptFile = path;
                    controller.basePath = path.Substring(0, path.LastIndexOf('/'));
                    controller.Initialize();
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

        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    } 
}
