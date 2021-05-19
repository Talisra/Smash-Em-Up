using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track
{
    public Subtrack[] subtracks;
    public float masterVolume;
    public bool mute = true;
}
