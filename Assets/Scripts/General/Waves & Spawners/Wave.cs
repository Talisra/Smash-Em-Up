using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public List<Subwave> subwaves = new List<Subwave>();
    public List<int> powerUps = new List<int>();
    public List<int> mapBonuses = new List<int>();

    public int CountEnemies()
    {
        int total = 0;
        foreach(Subwave sub in subwaves)
        {
            total += sub.CountEnemies();
        }
        return total;
    }
}
