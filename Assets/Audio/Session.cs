using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Session
{
    public string name;
    public Track[] tracks;
    public Session next;
}
