using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryserBody : MonoBehaviour
{
    public float radialSpeed = 150;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, radialSpeed * Time.deltaTime, 0, Space.World);
    }
}
