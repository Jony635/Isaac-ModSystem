using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance;

    private List<Enemy> enemies = new List<Enemy>();

    public Dictionary<uint, Enemy> enemiesRef = new Dictionary<uint, Enemy>();

    private void Awake()
    {
        Instance = this;

        ImportEnemies();

    }

    private void ImportEnemies()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsSubclassOf(typeof(Enemy)))
            {              
                GameObject newGO = new GameObject();
                newGO.name = type.ToString();
                newGO.transform.SetParent(transform);

                Enemy enemy = (Enemy)newGO.AddComponent(type);
                enemy.gameObject.SetActive(false);
                enemies.Add(enemy);            
            }
        }
    }

    public Enemy[] GetEnemiesWith(float difficulty)
    {
        List<Enemy> ret = new List<Enemy>();
        float achievedDifficulty = 0f;

        List<Enemy> spawnable;
        do
        {
            spawnable = enemies.FindAll(enemy => enemy.difficulty <= (difficulty - achievedDifficulty));
            if(spawnable.Count > 0)
            {
                int rand = UnityEngine.Random.Range(0, spawnable.Count);
                ret.Add(Instantiate(enemies[rand].gameObject).GetComponent<Enemy>());
                achievedDifficulty += enemies[rand].difficulty;
            }          
        } while (spawnable.Count > 0);
       
        return ret.ToArray();
    }

    public uint RefEnemy(Enemy enemy)
    {
        if (enemiesRef.ContainsValue(enemy))
        {
            foreach(uint _key in enemiesRef.Keys)
            {
                if (enemiesRef[_key] == enemy)
                    return _key;
            }
        }

        uint key = 0u;
        do
        {
            key = (uint)UnityEngine.Random.Range(1, int.MaxValue);

        } while (enemiesRef.ContainsKey(key));

        enemiesRef.Add(key, enemy);

        return key;
    }

    public Enemy GetRefEnemy(uint key)
    {
        if (!enemiesRef.ContainsKey(key))
            return null;

        return enemiesRef[key];
    }

    public void ClearEnemiesRef()
    {
        enemiesRef.Clear();
    }
}
