using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideWall : MonoBehaviour
{
    public GameObject wideWall;
    public TunnelDoor[] activeDoors;

    private float width;

    private void Awake()
    {
        width = wideWall.GetComponent<BoxCollider>().bounds.size.x;
    }

    public float GetWidth()
    {
        return width;
    }
}
