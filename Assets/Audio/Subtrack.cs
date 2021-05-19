using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Subtrack
{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource source;
    public float volume;
    public int length;
}
