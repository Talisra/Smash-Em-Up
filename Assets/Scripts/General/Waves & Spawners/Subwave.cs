using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subwave
{
    public List<int> enemiesIndex = new List<int>();
    public float minDelay;
    public float maxDelay;

    public int enemiesPerSpawn;

    public int CountEnemies()
    {
        return enemiesIndex.Count;
    }
}
