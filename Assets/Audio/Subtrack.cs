using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Subtrack
{
    public AudioClip clip;
    public float localVolume = 1; //leave at 1 normally
    [HideInInspector]
    public AudioSource source;
}
