using System.Collections.Generic;
using UnityEngine;

public class BricPool : MonoBehaviour
{
    [SerializeField]
    private BricData bricData;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Start()
    {
        for (int i = 0; i < bricData.bricTypes.Count; i++)
        {
            var bricType = bricData.bricTypes[i];

            if (poolDictionary.ContainsKey(bricType.prefab))
            {
                Debug.Log($"Le prefab {bricType.prefab.name} pour le type {bricType.name}" + $"est déjà dans le pool");
                continue;
            }

            int poolSize = GetPoolSizeForBricType(i);

            Queue<GameObject> bricQueue = new Queue<GameObject>();

            for (int j = 0; j < poolSize; j++)
            {
                GameObject bric = Instantiate(bricType.prefab);
                bric.SetActive(false);
                bricQueue.Enqueue(bric);
            }

            poolDictionary.Add(bricType.prefab, bricQueue);
        }
    }

    public GameObject GetBrics(GameObject prefab)
    {
        if (poolDictionary.TryGetValue(prefab, out Queue<GameObject> bricQueue) && bricQueue.Count > 0)
        {
            GameObject bric = bricQueue.Dequeue();
            bric.SetActive(true);

            return bric;
        }

        return null;
    }

    public void ReturnToPool(GameObject bric, GameObject prefab)
    {
        bric.SetActive(false);

        if (poolDictionary.TryGetValue(prefab, out var bricQueue))
        {
            bricQueue.Enqueue(bric);
        }
        else
        {
            Debug.Log("Tentative de retourner un ennemi à un pool inexistant !");
        }
    }

    public List<BricData.BricType> GetBricsTypes()
    {
        return bricData.bricTypes;
    }

    private int GetPoolSizeForBricType(int index)
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
