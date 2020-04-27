using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager Instance;

    private List<Enemy> enemies = new List<Enemy>();

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

    public GameObject[] GetEnemiesWith(float difficulty)
    {
        List<GameObject> ret = new List<GameObject>();
        float achievedDifficulty = 0f;

        List<Enemy> spawnable;
        do
        {
            spawnable = enemies.FindAll(enemy => enemy.difficulty <= difficulty - achievedDifficulty);
            if(spawnable.Count > 0)
            {
                int rand = UnityEngine.Random.Range(0, spawnable.Count);
                ret.Add(Instantiate(enemies[rand].gameObject));
                achievedDifficulty += enemies[rand].difficulty;
            }          
        } while (spawnable.Count > 0);
       
        return ret.ToArray();
    }
}
