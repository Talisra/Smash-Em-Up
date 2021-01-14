using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scraps : MonoBehaviour
{
    public int minAmount;
    public int MaxAmount;

    public float minSize;
    public float maxSize;

    public int GenerateAmount()
    {
        return Random.Range(minAmount, MaxAmount);
    }

    public float GenerateSize()
    {
        return Random.Range(minSize, maxSize);
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
