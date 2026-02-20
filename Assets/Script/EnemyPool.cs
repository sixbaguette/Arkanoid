using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    private EnemyData enemyData;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        for (int i = 0; i < enemyData.enemyTypes.Count; i++)
        {
            var enemyType = enemyData.enemyTypes[i];

            if (poolDictionary.ContainsKey(enemyType.prefab))
            {
                Debug.Log($"Le prefab {enemyType.prefab.name} pour le type {enemyType.name}" + $"est déjà dans le pool");
                continue;
            }

            int poolSize = GetPoolSizeForEnemyType(i);

            Queue<GameObject> enemyQueue = new Queue<GameObject>();

            for (int j = 0; j < poolSize; j++)
            {
                GameObject enemy = Instantiate(enemyType.prefab);
                enemy.SetActive(false);
                enemyQueue.Enqueue(enemy);
            }

            poolDictionary.Add(enemyType.prefab, enemyQueue);
        }
    }

    public GameObject GetEnemy(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> enemyQueue) && enemyQueue.Count > 0)
        {
            GameObject enemy = enemyQueue.Dequeue();
            enemy.SetActive(true);

            return enemy;
        }

        return null;
    }

    public void ReturnToPool(GameObject enemy, GameObject prefab)
    {
        enemy.SetActive(false);

        if (poolDictionary.TryGetValue(prefab, out var enemyQueue))
        {
            enemyQueue.Enqueue(enemy);
        }
        else
        {
            Debug.Log("Tentative de retourner un ennemi à un pool inexistant !");
        }
    }

    public List<EnemyData.EnemyType> GetEnemyTypes()
    {
        return enemyData.enemyTypes;
    }

    private int GetPoolSizeForEnemyType(int index)
    {
        switch (index)
        {
            case 0: //type A
                return 30; //22
            case 1: //type B
                return 30; //22
            case 2: //type C
                return 30; //11
            default:
                return 0;
        }
    }
}
