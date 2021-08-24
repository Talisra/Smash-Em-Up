using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuMenu : InGameMenu
{

    public override void Action(int action)
    {
        if (MenuManager.Instance)
            if (MenuManager.Instance.isBusy)
                return;
        switch (action)
        {
            case 0:
                MenuManager.Instance.PrepareLoadLevel();
                break;
            case 1:
                MenuManager.Instance.PerformAction(1);
                break;
            case 2:
                AudioManager.Instance.Mute();
                MenuManager.Instance.muteSound = true;
                break;
            case 3:
                AudioManager.Instance.Unmute();
                MenuManager.Instance.muteSound = false;
                break;
            case 4:
                AudioListener.volume = 0;
                MenuManager.Instance.muteMusic = true;
                break;
            case 5:
                AudioListener.volume = 1;
                MenuManager.Instance.muteMusic = false;
                break;
            default:
                break;
        }
    }
}
