using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Session
{
    public string name;
    public Track[] tracks;
    [HideInInspector]
    public List<int> tracksToPlay;
    [HideInInspector]
    public float maxDuration = 0;

    public void CalculateDurations()
    {
        foreach (Track track in tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
                foreach (Subtrack sub in layer.subtracks)
                {
                    if (sub.source.clip.length > maxDuration)
                    {
                        maxDuration = sub.source.clip.length;
                    }
                }
        }
    }

    public void SetTracksToPlay(List<int> newOrder)
    {
        tracksToPlay = newOrder;
    }

}
