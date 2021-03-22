using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scraps : MonoBehaviour, IPoolableObject
{
    protected AudioManager audioManager;

    public int minAmount;
    public int MaxAmount;

    public float minSize;
    public float maxSize;

    public int GenerateAmount()
    {
        return Random.Range(minAmount, MaxAmount);
    }

    public virtual Vector3 GenerateSize()
    {
        float size = Random.Range(minSize, maxSize);
        return new Vector3(size, size, size);
    }

    // Start is called before the first frame update
    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public virtual void BackToPool()
    {
        ScrapsPooler.Instance.ReturnToPool(gameObject);
    }
}
