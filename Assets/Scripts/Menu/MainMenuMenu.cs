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
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                MenuManager.Instance.PrepareLoadLevel();
                break;
            default:
                break;
        }
    }
}
