using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ProfileLoader : MonoBehaviour
{
    int level = 0;
    string currentName = "Asd";

    void Start()
    {
        SaveFile();
        LoadFile();
    }

    public void SaveFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        Profile data = new Profile
        {
            profileName = "TAL",
            level = 0
        };
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadFile()
    {
        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        Profile data = (Profile)bf.Deserialize(file);
        file.Close();

        level = data.level;
        currentName = data.profileName;

        Debug.Log(data.level);
        Debug.Log(data.profileName);
    }

}
