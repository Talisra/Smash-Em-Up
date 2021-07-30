using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProfile : MonoBehaviour
{
    public static GameProfile Instance { get; private set; }

    [HideInInspector]
    public bool isLevel;

    private Profile profile;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void SetLevelAndCheckProfile()
    {
        isLevel = true;
        if (profile == null)
        {
            profile = new Profile
            {
                profileName = "demo",
                level = 1
            };
        }
    }

    public Profile GetProfile()
    {
        return profile;
    }

    public void SetProfile(Profile prof)
    {
        profile = prof;
    }

}
