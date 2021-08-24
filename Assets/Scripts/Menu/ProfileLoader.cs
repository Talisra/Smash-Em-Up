using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ProfileLoader : MonoBehaviour
{
    private string fileDest;
    private string prefFileName = "/preferences.dat";

    private void Awake()
    {
        fileDest = Application.persistentDataPath;
    }

    public void SaveProfile(Profile profile)
    {
        string profileFileEnd = fileDest + "/profile" + profile.slot + ".dat";
        FileStream file;
        if (File.Exists(profileFileEnd)) file = File.OpenWrite(profileFileEnd);
        else file = File.Create(profileFileEnd);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, profile);
        file.Close();
    }

    public bool CheckLoad(int slot)
    {
        string fileToCheck = fileDest + "/profile" + slot + ".dat";
        if (File.Exists(fileToCheck))
        {
            return true;
        }
        return false;
    }

    public Profile LoadProfile(int slot)
    {
        string fileToCheck = fileDest + "/profile" + slot + ".dat";
        FileStream file;

        if (File.Exists(fileToCheck)) file = File.OpenRead(fileToCheck);
        else
        {
            Debug.LogError("Profile file #" + slot + " not found!");
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        Profile profile = (Profile)bf.Deserialize(file);
        file.Close();
        return profile;
    }

    public void DeleteProfile(int slot)
    {
        string fileToCheck = fileDest + "/profile" + slot + ".dat";
        if (File.Exists(fileToCheck)) 
            File.Delete(fileToCheck);
    }

    public void SavePreferences(LocalPreferences prefs)
    {
        string profileFileEnd = fileDest + prefFileName;
        FileStream file;
        if (File.Exists(profileFileEnd)) file = File.OpenWrite(profileFileEnd);
        else file = File.Create(profileFileEnd);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, prefs);
        file.Close();
    }

    public LocalPreferences LoadPreferences()
    {
        string fileToCheck = fileDest + prefFileName;
        FileStream file;

        if (File.Exists(fileToCheck)) file = File.OpenRead(fileToCheck);
        else
        {
            // create new pref file
            return new LocalPreferences();
        }
        BinaryFormatter bf = new BinaryFormatter();
        LocalPreferences prefs = (LocalPreferences)bf.Deserialize(file);
        file.Close();
        return prefs;
    }

}
