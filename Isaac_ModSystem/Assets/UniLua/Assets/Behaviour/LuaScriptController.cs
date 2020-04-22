using System;
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

                LoadPassiveItems();
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
        try
        {
            ret = Lua.L_CheckString(-1);
        }
        catch 
        { 
            Lua.Pop(-1);
            return null;
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

    private void LoadPassiveItems()
    {
        List<string> items = new List<string>();

        //Extract lua files from the global variable
        Lua.GetGlobal("passiveItems");
        for (int i = 1; true; ++i)
        {
            try
            {
                Lua.RawGetI(-1, i);
            }
            catch
            {
                break;
            }

            if (Lua.IsNoneOrNil(-1))
            {
                Lua.Pop(2);
                break;
            }               

            items.Add(Lua.L_CheckString(-1));
            Lua.Pop(1);
        }

        foreach(string item in items)
        {
            string itemPath = basePath + "/" + item;
            if(File.Exists(itemPath))
            {
                string itemName = item.Substring(item.LastIndexOf('/') + 1);
                itemName = itemName.Remove(itemName.IndexOf('.'));

                GameObject passiveItem = new GameObject();
                passiveItem.name = itemName;
                passiveItem.transform.SetParent(passiveItems.transform);

                LuaScriptController controller = passiveItem.AddComponent<LuaScriptController>();
                controller.LuaScriptFile = itemPath;
                controller.basePath = itemPath.Substring(0, itemPath.LastIndexOf('/'));
                controller.Initialize();           
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

