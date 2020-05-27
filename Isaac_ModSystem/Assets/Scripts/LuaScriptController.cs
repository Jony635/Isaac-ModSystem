using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniLua;
using System.IO;

public class LuaScriptController : MonoBehaviour
{
    const bool GEN_LIB = false;

    private ILuaState Lua;

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
    private int AwakeRef = -1;
    private int StartRef = -1;
    private int UpdateRef = -1;
    private int FixedUpdateRef = -1;
    private int LateUpdateRef = -1;
    private int OnCollisionEnter2DRef = -1;
    private int OnTriggerEnter2DRef = -1;
    private int OnCollisionStay2DRef = -1;
    private int OnTriggerStay2DRef = -1;
    private int OnCollisionExit2DRef = -1;
    private int OnTriggerExit2DRef = -1;
    private int OnEnableRef = -1;
    private int OnDisableRef = -1;
    private int OnEquippedRef = -1;
    private int OnEnemyHitStartRef = -1;
    private int OnEnemyHitStayRef = -1;
    private int OnEnemyHitExitRef = -1;
    private int OnMonsterHittedByTearRef = -1;
    private int OnUsedRef = -1;
    private int OnNewRoomEnteredRef = -1;
    private int OnCharacterCollidedWithMonsterRef = -1;
    private int OnCharacterCollidingWithMonsterRef = -1;
    private int OnPlayerShootRef = -1;
    private int OnEnemyDieRef = -1;
    #endregion 

    //C# Functions container
    private NameFuncPair[] lib;

    private Dictionary<UInt32, GameObject> childs = new Dictionary<uint, GameObject>();

    private List<Texture2D> extraTextures = new List<Texture2D>();

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
                new NameFuncPair("GetScale", GetScale),
                new NameFuncPair("SetPosition", SetPosition),
                new NameFuncPair("SetScale", SetScale),
                new NameFuncPair("SetRotation", SetRotation),
                
                new NameFuncPair("GetLocalPosition", GetLocalPosition),
                new NameFuncPair("SetLocalPosition", SetLocalPosition),
                new NameFuncPair("SetLocalRotation", SetLocalRotation),

                new NameFuncPair("SetComponent", SetComponent),

                new NameFuncPair("Damage", Damage),
                new NameFuncPair("Slow", Slow),
                new NameFuncPair("ClearSlow", ClearSlow),
                new NameFuncPair("GetSlowFactor", GetSlowFactor),

                new NameFuncPair("GetRoomEnemies", GetRoomEnemies),

                new NameFuncPair("GetMaxHealth", GetMaxHealth),
                new NameFuncPair("SetMaxHealth", SetMaxHealth),
                new NameFuncPair("GetHealth", GetHealth),
                new NameFuncPair("SetHealth", SetHealth),
                new NameFuncPair("GetInvincible", GetInvincible),
                new NameFuncPair("SetInvincible", SetInvincible),

                new NameFuncPair("SetLayer", SetLayer),
                new NameFuncPair("AddForce", AddForce),

                new NameFuncPair("SetActivePercent", SetActivePercent),
                new NameFuncPair("GetCurrentCharges", GetCurrentCharges),

                new NameFuncPair("This", This),

                new NameFuncPair("GetActive", GetActive),
                new NameFuncPair("SetActive", SetActive),

                new NameFuncPair("Notify", Notify),
                new NameFuncPair("GetStats", GetStats),
                new NameFuncPair("SetStats", SetStats),

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
            OnEnemyHitStartRef = StoreMethod("OnEnemyHitStart");
            OnEnemyHitStayRef = StoreMethod("OnEnemyHitStay");
            OnEnemyHitExitRef = StoreMethod("OnEnemyHitExit");

            OnMonsterHittedByTearRef = StoreMethod("OnMonsterHittedByTear");
            OnUsedRef = StoreMethod("OnUsed");
            OnNewRoomEnteredRef = StoreMethod("OnNewRoomEntered");
            OnCharacterCollidedWithMonsterRef = StoreMethod("OnCharacterCollidedWithMonster");
            OnCharacterCollidingWithMonsterRef = StoreMethod("OnCharacterCollidingWithMonster");
            OnPlayerShootRef = StoreMethod("OnPlayerShoot");

            OnEnemyDieRef = StoreMethod("OnEnemyDie");
            #endregion

            Lua.Pop(-1);

            string spritePath = GetGlobalString("sprite");
            if (spritePath != null)
            {
                Texture2D texture = ImportTexture(basePath + "/" + spritePath);
                sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 30); 
            }

            ExtraTextures();

            if (isMain)
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
        CallMethod(AwakeRef);
    }

    private void Start()
    {
        CallMethod(StartRef);
    }

    private void Update()
    {
        CallMethod(UpdateRef);
    }

    private void FixedUpdate()
    {
        CallMethod(FixedUpdateRef);
    }

    private void LateUpdate()
    {
        CallMethod(LateUpdateRef);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            uint enemyId = MonsterManager.Instance.RefEnemy(collider.GetComponent<Enemy>());
            CallMethod(OnEnemyHitStartRef, 1, 0, null, enemyId);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) { }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            uint enemyId = MonsterManager.Instance.RefEnemy(collider.GetComponent<Enemy>());
            CallMethod(OnEnemyHitStayRef, 1, 0, null, enemyId);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) { }

    private void OnTriggerExit2D(Collider2D collider) 
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
        {
            uint enemyId = MonsterManager.Instance.RefEnemy(collider.GetComponent<Enemy>());
            CallMethod(OnEnemyHitExitRef, 1, 0, null, enemyId);
        }
    }

    public void OnEnable() { }

    public void OnDisable() { }

    public void OnEquipped()
    {
        CallMethod(OnEquippedRef);
    }

    public void OnMonsterHittedByTear(Enemy enemy, Vector2 velocity = new Vector2())
    {
        uint key = 0;

        Enemy thisEnemy = GetComponent<Enemy>();
        if (enemy != null)
            key = enemy == thisEnemy ? 1u : 0u;

        if(key == 0)
            key = MonsterManager.Instance.RefEnemy(enemy);

        CallMethod(OnMonsterHittedByTearRef, 1, 0, null, key);
    }

    public void OnUsed()
    {
        CallMethod(OnUsedRef);
    }

    public void OnNewRoomEntered(bool alreadyDefeated)
    {
        CallMethod(OnNewRoomEnteredRef, 1, 0, null, alreadyDefeated);
    }

    public void OnCharacterCollidedWithMonster(Enemy enemy)
    {
        CallMethod(OnCharacterCollidedWithMonsterRef, 1, 0, null, MonsterManager.Instance.RefEnemy(enemy));
    }

    public void OnCharacterCollidingWithMonster(Enemy enemy)
    {
        CallMethod(OnCharacterCollidingWithMonsterRef, 1, 0, null, MonsterManager.Instance.RefEnemy(enemy));
    }

    public void OnPlayerShoot(Vector2 direction)
    {
        CallMethod(OnPlayerShootRef, 1, 0, () =>
        {
            Lua.NewTable();

            Lua.PushString("x");
            Lua.PushNumber(direction.x);
            Lua.SetTable(-3);

            Lua.PushString("y");
            Lua.PushNumber(direction.y);

            Lua.SetTable(-3);
        });
    }

    public void OnEnemyDie()
    {
        CallMethod(OnEnemyDieRef);
    }

    #endregion

    #region Lua->C# FUNCTIONS
    private int AddDamage(ILuaState lua)
    {
        float sum = (float)lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.plainDamage += sum;

        PlayerController.Instance.stats = newStats;

        return 0;
    }

    private int SubstractDamage(ILuaState lua)
    {
        float sub = (float)lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.plainDamage -= sub;

        PlayerController.Instance.stats = newStats;

        return 0;
    }

    private int AddFactorDamage(ILuaState lua)
    {
        float factor = (float)lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.factorDamage *= factor;

        PlayerController.Instance.stats = newStats;

        return 0;
    }

    private int SubstractFactorDamage(ILuaState lua)
    {
        float factor = (float)lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.factorDamage /= factor;

        PlayerController.Instance.stats = newStats;

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
        newChild.layer = LayerMask.NameToLayer("Item");
        newChild.transform.SetParent(transform);
        newChild.transform.localPosition = Vector3.zero;

        uint rand = 0;
        do
        {
            rand = (uint)(UnityEngine.Random.value * uint.MaxValue);

        } while(childs.ContainsKey(rand) || rand == 0 || rand == 1);

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
        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

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

    private int GetScale(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

            lua.NewTable();

            lua.PushString("x");
            lua.PushNumber(child.transform.localScale.x);
            lua.SetTable(-3);

            lua.PushString("y");
            lua.PushNumber(child.transform.localScale.y);
            lua.SetTable(-3);
        }

        return 1;
    }

    private int SetPosition(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if(childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

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

        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;
            float angle = (float)lua.L_CheckNumber(2);

            child.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }     

        return 0;
    }    
    
    private int SetScale(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

            Vector2 newScale = new Vector2(0, 0);

            if (lua.IsTable(2))
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

                newScale = new Vector2((float)x, (float)y);
            }

            child.transform.localScale = newScale;
        }


        return 0;
    }

    private int GetLocalPosition(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

            lua.NewTable();

            lua.PushString("x");
            lua.PushNumber(child.transform.localPosition.x);
            lua.SetTable(-3);

            lua.PushString("y");
            lua.PushNumber(child.transform.localPosition.y);
            lua.SetTable(-3);
        }     

        return 1;
    }

    private int SetLocalPosition(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;

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

            child.transform.localPosition = newPosition;
        }

        return 0;
    }

    private int SetLocalRotation(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);

        if (childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;
            float angle = (float)lua.L_CheckNumber(2);

            child.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }     

        return 0;
    }

    private int SetComponent(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if(childs.ContainsKey(key) || key == 1)
        {
            GameObject child = key != 1 ? childs[key] : gameObject;
            switch(lua.L_CheckString(2))
            {
                case "SpriteRenderer":
                    {
                        if (lua.IsTable(3))
                        {
                            lua.Insert(3);

                            lua.PushString("enabled");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1))
                            {
                                bool enabled = lua.ToBoolean(-1);

                                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                                if (renderer == null)
                                    renderer = child.AddComponent<SpriteRenderer>();

                                renderer.enabled = enabled;
                            }
                            lua.Pop(1);

                            lua.PushString("sprite");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1))
                            {
                                int spriteIndex = lua.L_CheckInteger(-1);

                                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                                if (renderer == null)
                                    renderer = child.AddComponent<SpriteRenderer>();

                                renderer.sortingOrder = 10;

                                if (spriteIndex == 0)                                
                                    renderer.sprite = sprite;                               
                                else
                                {
                                    Texture2D texture = extraTextures[spriteIndex - 1];

                                    lua.PushString("rect");
                                    lua.GetTable(-3);
                                    if (lua.IsNoneOrNil(-1))
                                    {
                                        Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 30);
                                        renderer.sprite = newSprite;
                                    }
                                    else
                                    {                                       
                                        lua.Insert(-1);

                                        lua.PushString("x");
                                        lua.GetTable(-2);
                                        float x = (float)lua.L_CheckNumber(-1);
                                        lua.Pop(1);

                                        lua.PushString("y");
                                        lua.GetTable(-2);
                                        float y = (float)lua.L_CheckNumber(-1);
                                        lua.Pop(1);

                                        lua.PushString("w");
                                        lua.GetTable(-2);
                                        float w = (float)lua.L_CheckNumber(-1);
                                        lua.Pop(1);

                                        lua.PushString("h");
                                        lua.GetTable(-2);
                                        float h = (float)lua.L_CheckNumber(-1);
                                        lua.Pop(1);

                                        lua.Pop(1);

                                        Sprite newSprite = Sprite.Create(texture, new Rect(x, y, w, h), new Vector2(0.5f, 0.5f), 30);
                                        renderer.sprite = newSprite;
                                    }
                                }
                            }
                            lua.Pop(1);

                            lua.Pop(1);
                        }
                        break;
                    }
                case "BoxCollider":
                    {
                        if (lua.IsTable(3))
                        {
                            if(gameObject.GetComponent<Rigidbody2D>() == null)
                            {
                                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                                rb.isKinematic = true;
                            }

                            lua.Insert(3);

                            lua.PushString("enabled");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1))
                            {
                                bool enabled = lua.ToBoolean(-1);

                                BoxCollider2D box = child.GetComponent<BoxCollider2D>();
                                if (box == null)
                                    box = child.AddComponent<BoxCollider2D>();

                                box.enabled = enabled;
                            }
                            lua.Pop(1);

                            lua.PushString("isTrigger");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1))
                            {
                                bool isTrigger = lua.ToBoolean(-1);

                                BoxCollider2D box = child.GetComponent<BoxCollider2D>();
                                if (box == null)
                                    box = child.AddComponent<BoxCollider2D>();

                                box.isTrigger = isTrigger;                              
                            }
                            lua.Pop(1);

                            lua.PushString("center");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1) && lua.IsTable(-1))
                            {
                                lua.PushString("x");
                                lua.GetTable(-2);
                                Vector2 center;
                                center.x = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                lua.PushString("y");
                                lua.GetTable(-2);
                                center.y = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                BoxCollider2D box = child.GetComponent<BoxCollider2D>();
                                if (box == null)
                                    box = child.AddComponent<BoxCollider2D>();

                                box.offset = center;
                            }
                            lua.Pop(1);

                            lua.PushString("size");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1) && lua.IsTable(-1))
                            {
                                lua.PushString("x");
                                lua.GetTable(-2);
                                Vector2 size;
                                size.x = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                lua.PushString("y");
                                lua.GetTable(-2);
                                size.y = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                BoxCollider2D box = child.GetComponent<BoxCollider2D>();
                                if (box == null)
                                    box = child.AddComponent<BoxCollider2D>();

                                box.size = size;
                            }
                            lua.Pop(1);

                            lua.Pop(1);
                        }
                        break;
                    }
                case "CircleCollider":
                    {
                        if (lua.IsTable(3))
                        {
                            if (gameObject.GetComponent<Rigidbody2D>() == null)
                            {
                                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                                rb.isKinematic = true;
                            }

                            lua.Insert(3);

                            lua.PushString("enabled");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1))
                            {
                                bool enabled = lua.ToBoolean(-1);

                                CircleCollider2D circle = child.GetComponent<CircleCollider2D>();
                                if (circle == null)
                                    circle = child.AddComponent<CircleCollider2D>();

                                circle.enabled = enabled;
                            }
                            lua.Pop(1);

                            lua.PushString("isTrigger");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1))
                            {
                                bool isTrigger = lua.ToBoolean(-1);

                                CircleCollider2D circle = child.GetComponent<CircleCollider2D>();
                                if (circle == null)
                                    circle = child.AddComponent<CircleCollider2D>();

                                circle.isTrigger = isTrigger;
                            }
                            lua.Pop(1);

                            lua.PushString("center");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1) && lua.IsTable(-1))
                            {
                                lua.PushString("x");
                                lua.GetTable(-2);
                                Vector2 center;
                                center.x = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                lua.PushString("y");
                                lua.GetTable(-2);
                                center.y = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                CircleCollider2D circle = child.GetComponent<CircleCollider2D>();
                                if (circle == null)
                                    circle = child.AddComponent<CircleCollider2D>();

                                circle.offset = center;
                            }
                            lua.Pop(1);

                            lua.PushString("radius");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1))
                            {
                                float radius = (float)lua.L_CheckNumber(-1);

                                CircleCollider2D circle = child.GetComponent<CircleCollider2D>();
                                if (circle == null)
                                    circle = child.AddComponent<CircleCollider2D>();

                                circle.radius = radius;
                            }
                            lua.Pop(1);

                            lua.Pop(1);
                        }
                        break;
                    }
                case "CapsuleCollider":
                    {
                        if (lua.IsTable(3))
                        {
                            if (gameObject.GetComponent<Rigidbody2D>() == null)
                            {
                                Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
                                rb.isKinematic = true;
                            }

                            lua.Insert(3);

                            lua.PushString("enabled");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1))
                            {
                                bool enabled = lua.ToBoolean(-1);

                                CapsuleCollider2D capsule = child.GetComponent<CapsuleCollider2D>();
                                if (capsule == null)
                                    capsule = child.AddComponent<CapsuleCollider2D>();

                                capsule.enabled = enabled;
                            }
                            lua.Pop(1);

                            lua.PushString("isTrigger");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1))
                            {
                                bool isTrigger = lua.ToBoolean(-1);

                                CapsuleCollider2D capsule = child.GetComponent<CapsuleCollider2D>();
                                if (capsule == null)
                                    capsule = child.AddComponent<CapsuleCollider2D>();

                                capsule.isTrigger = isTrigger;
                            }
                            lua.Pop(1);

                            lua.PushString("center");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1) && lua.IsTable(-1))
                            {
                                lua.PushString("x");
                                lua.GetTable(-2);
                                Vector2 center;
                                center.x = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                lua.PushString("y");
                                lua.GetTable(-2);
                                center.y = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                CapsuleCollider2D capsule = child.GetComponent<CapsuleCollider2D>();
                                if (capsule == null)
                                    capsule = child.AddComponent<CapsuleCollider2D>();

                                capsule.offset = center;
                            }
                            lua.Pop(1);

                            lua.PushString("size");
                            lua.GetTable(-2);
                            if (!lua.IsNoneOrNil(-1) && lua.IsTable(-1))
                            {
                                lua.PushString("x");
                                lua.GetTable(-2);
                                Vector2 size;
                                size.x = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                lua.PushString("y");
                                lua.GetTable(-2);
                                size.y = (float)lua.L_CheckNumber(-1);
                                lua.Pop(1);

                                CapsuleCollider2D capsule = child.GetComponent<CapsuleCollider2D>();
                                if (capsule == null)
                                    capsule = child.AddComponent<CapsuleCollider2D>();

                                capsule.size = size;
                            }
                            lua.Pop(1);

                            lua.PushString("direction");
                            lua.GetTable(-2);
                            if(!lua.IsNoneOrNil(-1))
                            {
                                string direction = lua.L_CheckString(-1);

                                CapsuleCollider2D capsule = child.GetComponent<CapsuleCollider2D>();
                                if (capsule == null)
                                    capsule = child.AddComponent<CapsuleCollider2D>();

                                capsule.direction = direction == "Vertical" ? CapsuleDirection2D.Vertical : CapsuleDirection2D.Horizontal;
                            }
                            lua.Pop(1);

                            lua.Pop(1);
                        }
                        break;
                    }
                case "TearController":
                    {
                        child.AddComponent<TearController>();
                        break;
                    }
            }
        }
        return 0;
    }

    private int Damage(ILuaState lua)
    {
        uint monsterKey = lua.L_CheckUnsigned(1);

        Enemy enemy = MonsterManager.Instance.GetRefEnemy(monsterKey);
        if (enemy)
        {
            float number = (float)lua.L_CheckNumber(2);
            enemy.TakeDamage((float)lua.L_CheckNumber(2));
        }

        return 0;
    }

    private int Slow(ILuaState lua)
    {
        uint monsterKey = lua.L_CheckUnsigned(1);

        Enemy enemy = MonsterManager.Instance.GetRefEnemy(monsterKey);
        if (enemy)
        {
            float number = (float)lua.L_CheckNumber(2);
            enemy.enemyStats.speedFactor *= (1 - ((float)lua.L_CheckNumber(2)));
        }

        return 0;
    }

    private int ClearSlow(ILuaState lua)
    {
        uint monsterKey = lua.L_CheckUnsigned(1);

        Enemy enemy = MonsterManager.Instance.GetRefEnemy(monsterKey);
        if (enemy)
        {
            float number = (float)lua.L_CheckNumber(2);
            enemy.enemyStats.speedFactor /= (1 - ((float)lua.L_CheckNumber(2)));
        }

        return 0;
    }

    private int GetSlowFactor(ILuaState lua)
    {
        uint monsterKey = lua.L_CheckUnsigned(1);

        Enemy enemy = MonsterManager.Instance.GetRefEnemy(monsterKey);
        if (enemy)
            lua.PushNumber(enemy.enemyStats.speedFactor);

        return 1;
    }

    private int GetRoomEnemies(ILuaState lua)
    {
        if (RoomManager.Instance.currentRoom.monsters.Length > 0)
        {
            uint amountEnemies = 0;

            lua.NewTable();
            for (int i = 0; i < RoomManager.Instance.currentRoom.monsters.Length; ++i)
            { 
                Enemy monster = RoomManager.Instance.currentRoom.monsters[i];

                if (monster.gameObject.activeInHierarchy)
                {
                    amountEnemies++;
                    lua.PushInteger(i+1);
                    lua.PushUnsigned(MonsterManager.Instance.RefEnemy(monster));
                    lua.SetTable(-3);                    
                }
            }

            lua.PushUnsigned(amountEnemies);
            lua.Insert(-2);

            return 2;
        }
        else
        {
            lua.PushInteger(0);
            return 1;
        }
   
    }

    private int GetMaxHealth(ILuaState lua)
    {
        uint id = 0;

        if(!lua.IsNoneOrNil(1))
            id = lua.L_CheckUnsigned(1);

        if (id == 0)
            lua.PushNumber(PlayerController.Instance.stats.maxHp);
        else
            lua.PushNumber(MonsterManager.Instance.GetRefEnemy(id).enemyStats.maxHP);

        return 1;
    }

    private int SetMaxHealth(ILuaState lua)
    {
        double maxHealth = lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.maxHp = (float)maxHealth;

        PlayerController.Instance.stats = newStats;

        return 0;
    }

    private int GetHealth(ILuaState lua)
    {
        double health = lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.hp = (float)health;

        PlayerController.Instance.stats = newStats;
        return 0;
    }

    private int SetHealth(ILuaState lua)
    {
        double health = lua.L_CheckNumber(1);

        PlayerController.Stats newStats = PlayerController.Instance.stats;
        newStats.hp = (float)health;

        PlayerController.Instance.stats = newStats;
        return 0;
    }

    private int GetInvincible(ILuaState lua)
    {
        lua.PushBoolean(PlayerController.Instance.stats.invincible);
        return 1;
    }

    private int SetInvincible(ILuaState lua)
    {
        if(!lua.IsNoneOrNil(1))
        {
            PlayerController.Stats newStats = PlayerController.Instance.stats;
            newStats.invincible = lua.ToBoolean(1);
            PlayerController.Instance.stats = newStats;
        }

        return 0;
    }

    private int SetLayer(ILuaState lua)
    {
        uint id = lua.L_CheckUnsigned(1);

        if(childs.ContainsKey(id))
        {
            GameObject gameObject = childs[id];

            int layer = LayerMask.NameToLayer(lua.L_CheckString(2));
            if (layer == -1)
                return 0;

            gameObject.layer = layer;
        }

        return 0;
    }

    private int AddForce(ILuaState lua)
    {
        uint key = lua.L_CheckUnsigned(1);
        if(childs.ContainsKey(key))
        {
            GameObject child = childs[key];

            lua.PushValue(2);

            lua.PushString("x");
            lua.GetTable(-2);
            float x = (float)lua.L_CheckNumber(-1);
            lua.Pop(1);

            lua.PushString("y");
            lua.GetTable(-2);
            float y = (float)lua.L_CheckNumber(-1);
            lua.Pop(2);

            string forceMode2D = lua.L_CheckString(3);

            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            if (rb == null)
                rb = child.AddComponent<Rigidbody2D>();

            rb.isKinematic = false;
            rb.AddForce(new Vector2(x, y), forceMode2D == "Impulse" ? ForceMode2D.Impulse : ForceMode2D.Force);
        }

        return 0;
    }

    private int SetActivePercent(ILuaState lua)
    {
        if (ActiveItemContainer.Instance.GetActiveItem() != GetComponent<Item>())
            return 0;

        float percent = (float)lua.L_CheckNumber(1);

        ActiveItemContainer.Instance.SetActivePercent(percent);

        return 0;
    }

    private int GetCurrentCharges(ILuaState lua)
    {
        lua.PushInteger(ActiveItemContainer.Instance.GetCurrentCharges());
        return 1;
    }

    private int This(ILuaState lua)
    {
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
            lua.PushUnsigned(1);
        else
            lua.PushNil();

        return 1;
    }

    private int GetActive(ILuaState lua)
    {
        uint index = lua.L_CheckUnsigned(1);

        if(index == 1 || childs.ContainsKey(index))
        {
            GameObject child = index == 1 ? gameObject : childs[index];
            lua.PushBoolean(child.activeSelf);
        }
        else
            lua.PushNil();

        return 1;
    }

    private int SetActive(ILuaState lua)
    {
        uint index = lua.L_CheckUnsigned(1);

        if (index == 1 || childs.ContainsKey(index))
        {
            GameObject child = index == 1 ? gameObject : childs[index];

            bool active = lua.ToBoolean(2);

            if (!active)
                StopAllCoroutines();

            child.SetActive(active);
        }

        return 0;
    }

    private int Notify(ILuaState lua)
    {
        string notification = lua.L_CheckString(1);

        switch(notification)
        {
            case "OnMonsterDied":
                {
                    Enemy enemy = GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        gameObject.SetActive(false);
                        enemy.currentRoom.OnMonsterDied();
                    }
                    break;
                }
        }

        return 0;
    }

    private int GetStats(ILuaState lua)
    {
        Enemy enemy = GetComponent<Enemy>();
        if(!enemy)
        {
            lua.PushNil(); return 1;
        }

        lua.NewTable();

        lua.PushString("attackSpeed");
        lua.PushNumber(enemy.enemyStats.attackSpeed);
        lua.SetTable(-3);

        lua.PushString("damage");
        lua.PushNumber(enemy.enemyStats.damage);
        lua.SetTable(-3);

        lua.PushString("hp");
        lua.PushNumber(enemy.enemyStats.hp);
        lua.SetTable(-3);

        lua.PushString("maxHP");
        lua.PushNumber(enemy.enemyStats.maxHP);
        lua.SetTable(-3);

        lua.PushString("runSpeed");
        lua.PushNumber(enemy.enemyStats.runSpeed);
        lua.SetTable(-3);

        lua.PushString("speed");
        lua.PushNumber(enemy.enemyStats.speed);
        lua.SetTable(-3);

        lua.PushString("speedFactor");
        lua.PushNumber(enemy.enemyStats.speedFactor);
        lua.SetTable(-3);

        return 1;
    }

    private int SetStats(ILuaState lua)
    {
        Enemy enemy = GetComponent<Enemy>();
        if (!enemy)
            return 0;

        Enemy.EnemyStats enemyStats = enemy.enemyStats;

        if(!lua.IsNoneOrNil(1) && lua.IsTable(1))
        {
            lua.PushValue(1);

            lua.PushString("attackSpeed");
            lua.GetTable(-2);
            if(!lua.IsNoneOrNil(-1))
            {
                enemyStats.attackSpeed = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("damage");
            lua.GetTable(-2);
            if(!lua.IsNoneOrNil(-1))
            {
                enemyStats.damage = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("hp");
            lua.GetTable(-2);
            if (!lua.IsNoneOrNil(-1))
            {
                enemyStats.hp = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("maxHP");
            lua.GetTable(-2);
            if (!lua.IsNoneOrNil(-1))
            {
                enemyStats.maxHP = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("runSpeed");
            lua.GetTable(-2);
            if (!lua.IsNoneOrNil(-1))
            {
                enemyStats.runSpeed = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("speed");
            lua.GetTable(-2);
            if (!lua.IsNoneOrNil(-1))
            {
                enemyStats.speed = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.PushString("speedFactor");
            lua.GetTable(-2);
            if (!lua.IsNoneOrNil(-1))
            {
                enemyStats.speedFactor = (float)lua.L_CheckNumber(-1);
            }
            lua.Pop(1);

            lua.Pop(1);
        }

        enemy.enemyStats = enemyStats;

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

	private void CallMethod( int funcRef, int numArguments = 0, int numResults = 0, Action customTypesCallback = null, params object[] arguments)
	{
        if (funcRef == -1) return;

        Lua.RawGetI( LuaDef.LUA_REGISTRYINDEX, funcRef );

        // insert `traceback' function
        var b = Lua.GetTop();
        Lua.PushCSharpFunction(Traceback);
        Lua.Insert(b);

        for (int i = 0; i < numArguments && arguments.Length > 0; ++i)
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
            else if (type == typeof(uint))
            {
                Lua.PushUnsigned((uint)argument);
            }
        }

        if (customTypesCallback != null)
            customTypesCallback.Invoke();

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

            foreach (string file in elements)
            {
                string path = basePath + "/" + file;

                if (File.Exists(path))
                {
                    string name = file.Substring(file.LastIndexOf('/') + 1);
                    name = name.Remove(name.IndexOf('.'));

                    GameObject element = new GameObject();
                    element.name = name;
                    element.SetActive(false);

                    LuaScriptController controller = element.AddComponent<LuaScriptController>();
                    controller.enabled = true;
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
                            aItem.InitFromScript();
                            ItemManager.Instance.AddItem(aItem);
                            break;
                        case 2:
                            element.transform.SetParent(monsters.transform);
                            Enemy newEnemy = element.AddComponent<Enemy>();
                            newEnemy.luaScript = controller;
                            MonsterManager.Instance.AddEnemy(newEnemy);
                            break;
                    }               
                }
            }
        }    
    }

    public int GetNumCharges()
    {
        int ret = 1;

        Lua.GetGlobal("numCharges");

        if (Lua.IsNoneOrNil(-1))
            Debug.Log("NO CHARGES SPECIFIED ON ACTIVE ITEM");
        else
            ret = Lua.L_CheckInteger(-1);

        Lua.Pop(1);

        return ret;
    }

    private void ExtraTextures()
    {
        Lua.GetGlobal("extraTextures");
        if(!Lua.IsNoneOrNil(-1))
        {
            for (int i = 1; true; ++i)
            {
                Lua.PushNumber(i);
                Lua.GetTable(-2);

                if(Lua.IsNoneOrNil(-1))
                {
                    Lua.Pop(2);
                    return;
                }

                string texturePath = Lua.L_CheckString(-1);

                Texture2D texture = ImportTexture(basePath + "/" + texturePath);
                extraTextures.Add(texture);

                Lua.Pop(1);
            }             
        }
    }

    public static Texture2D ImportTexture(string path)
    {
        byte[] textureBytes = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;
        texture.LoadImage(textureBytes);

        return texture;
    } 
}