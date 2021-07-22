using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{

    public void Action(int action)
    {
        switch (action)
        {
            case 0:
                GameManager.Instance.UnpauseGame();
                break;
            case 1:
                GameManager.Instance.EndGameFromMenu();
                break;
            case 2:
                AudioManager.Instance.ToggleMuteTrue();
                break;
            case 3:
                AudioManager.Instance.ToggleMuteFalse();
                break;
            case 4:
                SoundtrackManager.Instance.Mute();
                break;
            case 5:
                SoundtrackManager.Instance.Unmute();
                break;
            default:
                break;
        }
    }
}
