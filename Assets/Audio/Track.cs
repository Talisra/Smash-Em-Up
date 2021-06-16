using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track
{
    [SerializeField]
    public List<TrackLayer> trackLayers;
    public float masterVolume;
    public bool isLast = false;
    [HideInInspector]
    public bool mute;
}