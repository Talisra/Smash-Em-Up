using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInitRotation : MonoBehaviour
{
    Quaternion initRotation;
    // Start is called before the first frame update
    void Start()
    {
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Slerp(initRotation, initRotation, 0);
    }
}
