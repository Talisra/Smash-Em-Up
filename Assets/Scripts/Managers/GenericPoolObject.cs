using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPoolObject<T> : MonoBehaviour where T: Component
{
    [SerializeField]
    private T prefab;

    public static GenericPoolObject<T> Instance { get; private set; }

    private Queue<T> objects = new Queue<T>();

    private void Awake()
    {
        Instance = this;
    }

    public T Get(Vector3 newPos, Quaternion newRotation)
    {
        if (objects.Count == 0)
            AddObjects(1);
        T returnObj = objects.Dequeue();
        returnObj.transform.position = newPos;
        returnObj.transform.rotation = newRotation;
        returnObj.gameObject.SetActive(true);
        return returnObj;
    }
    public void ReturnToPool(T objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        objects.Enqueue(objectToReturn);
    }

    public void AddObjects(int count)
    {
        var newObject = GameObject.Instantiate(prefab);
        newObject.gameObject.SetActive(false);
        objects.Enqueue(newObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
