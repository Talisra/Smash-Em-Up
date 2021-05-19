using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Session
{
    public string name;
    public Track[] tracks;
    [HideInInspector]
    public float maxLength = 0;
    [HideInInspector]
    public float maxDuration;
    [HideInInspector]
    public int trackCounter = 1;


    public void OrderTracks()
    {
        foreach(Track track in tracks)
        {
            foreach(Subtrack sub in track.subtracks)
            {
                if (sub.length > maxLength)
                {
                    maxLength = sub.length;
                }
                if (sub.source.clip.length > maxDuration)
                {
                    maxDuration = sub.source.clip.length;
                }
            }
        }
    }

}
