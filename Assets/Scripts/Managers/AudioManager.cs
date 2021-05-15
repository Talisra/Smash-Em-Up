using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public Sound[] sounds;
    public Session[] sessions;
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
            //Debug.Log(s);
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
        foreach (Session s in sessions)
        {
            foreach (Track track in s.tracks)
            {
                Sound sound = track.sound;
                sound.source = gameObject.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;
                sound.source.volume = track.volume; // volume is controlled by track
                sound.source.pitch = 1; // pitch must always be 1;
                sound.source.loop = true; // 
            }
        }
    }

    private void Start()
    {
        //sessions[0].tracks[0].sound.source.Play();
    }



    public void Play(string name)
    {
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
}
