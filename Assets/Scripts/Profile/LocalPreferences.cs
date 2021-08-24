[System.Serializable]
public class LocalPreferences
{
    public int SelectedProfile { get; set; } // a number mean the index selected (between 1 to 3), while -1 means there are no
                                             // profiles and the user must create one
    public bool muteSound = false;
    public bool muteMusic = false;

    public LocalPreferences()
    {
        SelectedProfile = -1;
    }

    public LocalPreferences(int idx, bool muteSound, bool muteMusic)
    {
        SelectedProfile = idx;
        this.muteSound = muteSound;
        this.muteMusic = muteMusic;
    }
}
