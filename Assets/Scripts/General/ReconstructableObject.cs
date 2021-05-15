using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReconstructableObject : MonoBehaviour, IPoolableObject
{
    public Part[] parts;
    public GameObject kernel;
    // helps with reconstruction
    private Vector3 startPos;
    private Quaternion startQart;
    private List<Vector3> ObjsStartPos;
    private List<Quaternion> ObjsStartQart;
    private List<Part> partStoarge;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        SaveStartPosition();
        partStoarge = new List<Part>();
    }

    private void SaveStartPosition()
    {
        ObjsStartPos = new List<Vector3>();
        ObjsStartQart = new List<Quaternion>();
        startPos = transform.position;
        startQart = transform.rotation;
        foreach (Part child in parts)
        {
            ObjsStartPos.Add(child.transform.position);
            ObjsStartQart.Add(child.transform.rotation);
        }
    }

    protected virtual void Break()
    {
        RemoveChildren();
        kernel.SetActive(true);
        kernel.SetActive(false);
    }

    protected virtual void FadeIn() // must be after reset!
    {
        List<Part> childrenList = new List<Part>();
        foreach (Part child in parts)
        {
            childrenList.Add(child);
        }
        foreach (Part child in childrenList)
        {
            child.StartFadeIn();
        }
    }

    private void RemoveChildren()
    {
        List<Part> childrenList = new List<Part>();
        foreach (Part child in parts)
        {
            childrenList.Add(child);
        }
        foreach (Part child in childrenList)
        {
            child.transform.SetParent(null);
            child.gameObject.AddComponent<Rigidbody>();
            child.StartFadeOut();
            partStoarge.Add(child);
        }
    }

    private void Reconstruct()
    {
        List<Part> partList = new List<Part>();
        foreach (Part part in partStoarge)
        {
            partList.Add(part);
        }
        int counter = 0;
        foreach (Part child in partList)
        {
            child.gameObject.SetActive(true);
            child.transform.SetParent(this.transform);
            Destroy(child.gameObject.GetComponent<Rigidbody>());
            child.transform.position = ObjsStartPos[counter];
            child.transform.rotation = ObjsStartQart[counter++];
        }
        partStoarge.Clear();
    }
    protected virtual void Reset()
    {
        transform.position = startPos;
        transform.rotation = startQart;
        Reconstruct();
        BackToPool();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void BackToPool()
    {
        // Implement at child
    }
}
