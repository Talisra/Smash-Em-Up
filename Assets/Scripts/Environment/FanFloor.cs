using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanFloor : MonoBehaviour
{
    public List<GameObject> AffectedObjects;
    public Vector3 ForceVector;

    void OnTriggerEnter(Collider collidee)
    {
        if (collidee.gameObject.tag == "Enemy")
            AffectedObjects.Add(collidee.gameObject);
    }

    void OnTriggerExit(Collider collidee)
    {
        AffectedObjects.Remove(collidee.gameObject);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < AffectedObjects.Count; i++)
        {
            GameObject go = AffectedObjects[i];
            if (go)
            {
                go.GetComponent<Rigidbody>().AddForce(ForceVector);
            }
            else
                AffectedObjects.Remove(go);
        }
    }
}
