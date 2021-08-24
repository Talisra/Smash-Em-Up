using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cog : MonoBehaviour
{
    public float radialSpeed;


    private void Update()
    {

        transform.Rotate(0, 0, radialSpeed * Time.deltaTime, Space.World);
    }
}
