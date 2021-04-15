using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapsPooler : MonoBehaviour
{
    public static ScrapsPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDict;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDict = new Dictionary<string, Queue<GameObject>>();

        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.name = pool.tag;
                objectPool.Enqueue(obj);
            }
            poolDict.Add(pool.tag, objectPool);
        }
        
    }

    private void IncreasePool(string tag)
    {
        Pool tagPool = null;
        foreach(Pool pool in pools)
        {
            if (pool.tag == tag)
                tagPool = pool;
        }
        if (tagPool == null)
        {
            Debug.LogWarning("Couldn't increase Pool with tag " + tag + " doesn't exist!");
            return;
        }
        GameObject obj = Instantiate(tagPool.prefab);
        obj.SetActive(false);
        poolDict[tag].Enqueue(obj);
        obj.name = tag;
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        poolDict[objectToReturn.name].Enqueue(objectToReturn);
    }

    public GameObject Get(string tag, Vector3 newPos, Quaternion newRot)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogWarning("Couldn't GET from Pool with tag " + tag + " doesn't exist!");
            return null;
        }
        else if (poolDict[tag].Count == 0)
            IncreasePool(tag);
        GameObject obj = poolDict[tag].Dequeue();
        obj.SetActive(true);
        obj.transform.position = newPos;
        obj.transform.rotation = newRot;

        return obj;
    }

}
