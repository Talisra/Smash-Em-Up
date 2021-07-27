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
    public Sound[] menuSounds;

    private List<bool> activeLayers;
    private List<int> layerOrder;

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
            s.source.ignoreListenerVolume = true;
        }
        foreach (Sound s in shutDown)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.ignoreListenerVolume = true;
        }
        foreach(Sound s in menuSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.ignoreListenerVolume = true;
            s.source.ignoreListenerPause = true;
        }
    }

    public void Mute()
    {
        muteSound = true;
    }

    public void Unmute()
    {
        muteSound = false;
    }

    public void Reset()
    {
        foreach (Sound sound in shutDown)
            sound.source.Stop();
        Stop("WhiteNoise");
    }

    public void PlayShutDown()
    {
        foreach (Sound sound in shutDown)
            sound.source.Play();
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

    public void MenuPlay(string name)
    {
        if (name == "")
            return;
        Sound s = Array.Find(menuSounds, sound => sound.name == name);
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

    public float GetPitch(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.pitch;
    }

    public float GetLength(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        return s.source.clip.length;
    }
}
