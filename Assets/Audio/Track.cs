using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track
{
    public Sound sound;
    public bool mute;
    public int bars;
    public int layer;
    [Range(0, 1)]
    public float volume;
}
