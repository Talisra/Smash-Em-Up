using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    public static SoundtrackManager Instance { get; private set; }

    private int loops = 1;
    // The number of "loops" depends on the longest track. The number of loops can be changed
    // when adding more tracks.

    public bool isMute;

    public Session baseSession;
    public Session[] sessions;
    public List<int> trackOrder;
    private float maxDuration = 0;
    private int currentSession = 0;
    private int baseSessionCounter = 0;

    private List<bool> activeLayers;
    private List<int> layerOrder;
    private int currentLayerOrder = 0;

    public bool isLevel = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        int sessionLength = sessions[0].tracks.Length;
        foreach (Session session in sessions)
        {
            if (session.tracks.Length != sessionLength)
                Debug.LogWarning("Soundtrack Warning: Not all sessions have the same number of tracks!");
            foreach (Track track in session.tracks)
            {
                track.mute = true;
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        sub.source = gameObject.AddComponent<AudioSource>();
                        sub.source.clip = sub.clip;
                        sub.source.volume = track.masterVolume * sub.localVolume;
                        sub.source.loop = true;
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            track.mute = true;
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    sub.source = gameObject.AddComponent<AudioSource>();
                    sub.source.clip = sub.clip;
                    sub.source.volume = track.masterVolume * sub.localVolume;
                    sub.source.loop = true;
                }
            }
        }
        // get the longest duration for each session
        foreach (Session session in sessions)
        {
            session.CalculateDurations();
            if (session.maxDuration > maxDuration)
            {
                maxDuration = session.maxDuration;
            }
        }
        GenerateTrackOrder();
    }

    public void PauseSoundtrack()
    {
        foreach (Session session in sessions)
        {
            foreach (Track track in session.tracks)
            {
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        if (!track.mute)
                            sub.source.Pause();
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    if (!track.mute)
                        sub.source.Pause();
                }
            }
        }
    }

    public void ResumeSoundtrack()
    {
        foreach (Session session in sessions)
        {
            foreach (Track track in session.tracks)
            {
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        if (!track.mute)
                            sub.source.Play();
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    if (!track.mute)
                        sub.source.Play();
                }
            }
        }
    }

    public void Mute()
    {
        isMute = true;
        foreach (Session session in sessions)
        {
            foreach (Track track in session.tracks)
            {
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        if (!track.mute)
                            sub.source.volume = 0;
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    if (!track.mute)
                        sub.source.volume = 0;
                }
            }
        }
    }

    public void Unmute()
    {
        isMute = false;
        foreach (Session session in sessions)
        {
            foreach (Track track in session.tracks)
            {
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        if (!track.mute)
                            sub.source.volume = track.masterVolume * sub.localVolume;
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    if (!track.mute)
                        sub.source.volume = track.masterVolume * sub.localVolume;
                }
            }
        }
    }

    public void Reset()
    {
        GenerateTrackOrder();
        isLevel = false;
        baseSessionCounter = 0;
        currentSession = 0;
        currentLayerOrder = 0;
    }

    public void StartPlaying()
    {
        StartCoroutine(PlayBaseSession());
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        foreach (Session session in sessions)
        {
            foreach (Track track in session.tracks)
            {
                track.mute = true;
                foreach (TrackLayer layer in track.trackLayers)
                {
                    foreach (Subtrack sub in layer.subtracks)
                    {
                        sub.source.Stop();
                    }
                }
            }
        }
        foreach (Track track in baseSession.tracks)
        {
            track.mute = true;
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    sub.source.Stop();
                }
            }
        }
    }

    public void PlayLevelMusic()
    {
        StartCoroutine(PlaySession(sessions[currentSession], loops));
    }

    private void ManageTrackLayers(Session session)
    {
        if (currentLayerOrder < activeLayers.Count && currentSession == 0)
        {
            activeLayers[layerOrder[currentLayerOrder]] = true;
            currentLayerOrder++;
        }
        for (int i = 0; i < activeLayers.Count; i++)
        {
            if (activeLayers[i])
            {
                session.tracks[i].mute = false;
            }
        }
    }

    private void AddTrackLayer(Session session)
    {
        if (baseSessionCounter >= session.tracks.Length)
            return;
        session.tracks[baseSessionCounter].mute = false;
        baseSessionCounter++;
    }

    private void StopSession(Session sessionToStop)
    {
        foreach (Track track in sessionToStop.tracks)
        {
            foreach (TrackLayer layer in track.trackLayers)
            {
                foreach (Subtrack sub in layer.subtracks)
                {
                    sub.source.Stop();
                }
            }
        }
    }

    private IEnumerator PlaySession(Session session, int loop)
    {
        int i = 0;
        ManageTrackLayers(session);
        //LogSessionState(session);
        while (i < loop)
        {
            for (int j=0; j<session.tracks.Length; j++)
            {
                if (!session.tracks[j].mute)
                {
                    PlayTrack(session.tracks[j], trackOrder[j]);
                }
            }
            yield return new WaitForSeconds(maxDuration);
            i++;
        }
        //LogSessionState(session);
        currentSession++;
        if (currentSession >= sessions.Length)
            currentSession = 0;
        StopSession(session);
        StartCoroutine(PlaySession(sessions[currentSession], loop));
    }

    public IEnumerator PlayBaseSession()
    {
        AddTrackLayer(baseSession);
        bool audioStarted = false;
        while (true)
        {
            if (isLevel) // Level Music starts playing
            {
                if (currentSession == 1)
                    AddTrackLayer(baseSession);
                if (!audioStarted)
                {
                    PlayLevelMusic();
                    foreach (Track track in baseSession.tracks)
                    {
                        foreach (Subtrack sub in track.trackLayers[0].subtracks)
                        {
                            sub.source.Stop();
                        }
                    }
                    audioStarted = true;
                }
            }
            foreach (Track track in baseSession.tracks)
            {
                if (!track.mute)
                    PlayTrack(track, 0);
            }
            if (isLevel)
                yield return new WaitForSeconds(maxDuration * loops);
            else // Assuming all base session's track are the same length/duration
                yield return new WaitForSeconds(baseSession.tracks[0].trackLayers[0].subtracks[0].clip.length);
        }
    }

    private void PlayTrack(Track track, int index)
    {
        foreach (Subtrack sub in track.trackLayers[index].subtracks)
        {
            PlaySubTrack(sub);
        }
    }

    private void PlaySubTrack(Subtrack subtrack)
    {
        subtrack.source.Play();
    }


    private void GenerateTrackOrder()
    {
        Session session1 = sessions[0];
        List<int> order = new List<int>();
        foreach(Track track in session1.tracks)
        {
            if (track.trackLayers.Count > 1)
            {
                order.Add(Random.Range(0, track.trackLayers.Count));
            }
            else
                order.Add(0);
        }
        foreach (Session session in sessions)
            session.SetTracksToPlay(order);
        trackOrder = order;
        activeLayers = new List<bool>();
        layerOrder = new List<int>();
        for (int i = 0; i < sessions[0].tracks.Length ; i++)
        {
            activeLayers.Add(false);
            layerOrder.Add(i);
        }
        Auxiliary.Shuffle<int>(layerOrder);
        
        for (int i = 0; i < layerOrder.Count; i++)
        {
            if (sessions[0].tracks[i].isLast)
            {
                layerOrder.Remove(i);
                layerOrder.Add(i);
            }
        }
        /*
        for(int i=0; i<layerOrder.Count; i++)
        {
            Debug.Log(layerOrder[i] + " " +activeLayers[i]);
        }*/
    }
    /*
    public void LogSessionState(Session session)
    {
        Debug.Log("=======================");
        for (int j = 0; j < activeLayers.Count; j++)
        {
            string log = session.tracks[j].subtracks[0].source.clip.name + ": " + (activeLayers[j] == true ? " V Active" : " X Mute") + "|| isPlaying: " + session.tracks[j].subtracks[0].source.isPlaying;
            Debug.Log(log);
        }
        Debug.Log("=======================");
    }
    */
}
