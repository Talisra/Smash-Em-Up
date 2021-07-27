using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profile : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToNext;
    public string profileName;

    private int storedExp;

    public Skill[] skills;

    public void GainExp(int amount)
    {
        storedExp += amount;
    }
}
