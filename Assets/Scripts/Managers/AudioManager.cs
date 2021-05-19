using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;
    public bool muteSound;
    public Sound[] shutDown;
    public Session[] sessions;
    private float maxDuration = 0;
    private int currentSession = 1;

    private List<bool> activeLayers;
    private List<int> layerOrder;
    private int currentLayerOrder = 0;

    public bool isLevel = false;

    // Start is called before the first frame update
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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach (Sound s in shutDown)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach (Session session in sessions)
        {
            foreach(Track track in session.tracks)
            {
                track.mute = true;
                foreach(Subtrack sub in track.subtracks)
                {
                    sub.source = gameObject.AddComponent<AudioSource>();
                    sub.source.clip = sub.clip;
                    sub.source.volume = track.masterVolume;
                    sub.source.loop = true;
                }
            }
        }
    }

    public void Reset()
    {
        foreach (Sound sound in shutDown)
            sound.source.Stop();
        OrderTracks();
        isLevel = false;
        sessions[0].trackCounter = 0;
        currentSession = 1;
        currentLayerOrder = 0;
        StartCoroutine(PlaySessionLoop(sessions[0], 1));
    }

    public void StopMusic()
    {
        StopAllCoroutines();
        foreach(Session session in sessions)
        {
            foreach(Track track in session.tracks)
            {
                track.mute = true;
                foreach(Subtrack sub in track.subtracks)
                {
                    sub.source.Stop();
                }
            }
        }
    }

    public void PlayShutDown()
    {
        foreach (Sound sound in shutDown)
            sound.source.Play();
    }

    public void PlayLevelMusic()
    {
        StartCoroutine(PlaySession(sessions[currentSession], 1));
    }

    private void ManageTrackLayers(Session session)
    {
        if (currentLayerOrder >= activeLayers.Count)
            return;
        activeLayers[layerOrder[currentLayerOrder++]] = true;
        for(int i=0; i<activeLayers.Count; i++)
        {
            if (activeLayers[i])
            {
                session.tracks[i].mute = false;
                //Debug.Log("track " + i);
            }
        }
    }

    private void AddTrackLayer(Session session)
    {
        if (session.trackCounter >= session.tracks.Length)
            return;
        session.tracks[session.trackCounter++].mute = false;
    }

    private void NextSession(Session current)
    {
        foreach(Track track in current.tracks)
        {
            foreach(Subtrack sub in track.subtracks)
            {
                sub.source.Stop();
            }
        }
    }

    private IEnumerator PlaySession(Session session, int loop)
    {
        int i = 0;
        ManageTrackLayers(session);
        while (i < loop)
        {
            foreach (Track track in session.tracks)
            {
                if (!track.mute)
                    StartCoroutine(PlayTrack(track, session));
            }
            yield return new WaitForSeconds(maxDuration);
            i++;
        }
        currentSession++;
        if (currentSession >= sessions.Length)
            currentSession = 1;
        NextSession(session);
        StartCoroutine(PlaySession(sessions[currentSession], loop));
    }

    private IEnumerator PlaySessionLoop(Session session, int loopTime)
    {
        AddTrackLayer(session);
        bool levelStarted = false;
        bool audioStarted = false;
        while (true)
        {
            if (isLevel) // Level Music starts playing
            {
                if (levelStarted)
                    AddTrackLayer(session);
                if (!audioStarted)
                {
                    PlayLevelMusic();
                    foreach (Track track in session.tracks)
                    {
                        foreach(Subtrack sub in track.subtracks)
                        {
                            sub.source.Stop();
                        }
                    }
                    audioStarted = true;
                }
                levelStarted = true;
            }
            foreach (Track track in session.tracks)
            {
                if (!track.mute)
                    StartCoroutine(PlayTrack(track, session));
            }
            if (isLevel)
                yield return new WaitForSeconds(maxDuration * loopTime);
            else
                yield return new WaitForSeconds(session.tracks[0].subtracks[0].clip.length);
        }
    }

    private IEnumerator PlayTrack(Track track, Session parentSession)
    {
        foreach(Subtrack sub in track.subtracks)
        {
            StartCoroutine(PlaySubTrack(sub, parentSession));
        }
        yield return new WaitForSeconds(parentSession.maxDuration);
    }

    private IEnumerator PlaySubTrack(Subtrack subtrack, Session parentSession)
    {
        for (int i = 0 ; i< parentSession.maxLength/subtrack.length; i++)
        {
            subtrack.source.Play();
            yield return new WaitForSeconds(subtrack.source.clip.length);
        }
    }

    /*
    public IEnumerator PlaySubTrack(Subtrack subtrack, Session parentSession)
    {
        subtrack.source.Play();
        yield return new WaitForSeconds(parentSession.maxDuration);
    }
    */

    private void OrderTracks()
    {
        int maxLayers = 0;
        foreach (Session session in sessions)
        {
            session.OrderTracks();
            if (session.maxDuration > maxDuration)
            {
                maxDuration = session.maxDuration;
            }
            if (session.tracks.Length > maxLayers)
            {
                maxLayers = session.tracks.Length;
            }
        }
        activeLayers = new List<bool>();
        layerOrder = new List<int>();
        for (int i = 0; i < maxLayers; i++)
        {
            activeLayers.Add(false);
            layerOrder.Add(i);
        }
        Auxiliary.Shuffle<int>(layerOrder);
        /*
        Debug.Log("after shuffle");
        foreach (int num in layerOrder)
        {
            Debug.Log(num);
        }*/
    }

    public void Play(string name) // plays a sound with the given name
    {
        if (muteSound)
            return;
        if (name == "")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound with name " + name + " was not found");
            return;
        } 
        s.source.Play();
    }

    public void Stop(string name) // stop a sound that is in play, will do nothing if the sound isnt playing
    {
        if (muteSound)
            return;
        if (name == "")
            return;
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound with name " + name + " was not found");
            return;
        }
        s.source.Stop();
    }
}
