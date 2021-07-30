using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public YesNoDialogQuit dialog;
    public Profiler profiler;
    private int moveDir;
    private float moveSpeed = 50;
    private float startY;
    private float upYpos = 12;

    private void Start()
    {
        startY = transform.position.y;
    }

    public void SetProfile(Profile profile)
    {
        profiler.SetProfile(profile);
    }

    public virtual void UpdateUI()
    {
        profiler.UpdateUI();
    }

    public void MoveMenu(int direction) // 1: up, -1: down
    {
        moveDir = direction;
        if (direction == -1)
            transform.position = transform.position = new Vector3(transform.position.x, upYpos, transform.position.z);
    }

    private void Update()
    {
        if (moveDir != 0)
        {
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + moveSpeed * Time.unscaledDeltaTime * moveDir,
                transform.position.z);
            if (transform.position.y > upYpos && moveDir == 1)// moving up
            {
                transform.position = new Vector3(transform.position.x, startY, transform.position.z);
                moveDir = 0;
                GameManager.Instance.UnpauseGameEnd();
            } else if (transform.position.y <= startY && moveDir == -1)// moving down
            {
                transform.position = new Vector3(transform.position.x, startY, transform.position.z);
                moveDir = 0;
            }
        }
    }

    public virtual void Action(int action)
    {
        switch (action)
        {
            case 0:
                GameManager.Instance.UnpauseGameStart();
                break;
            case 1:
                dialog.gameObject.SetActive(true);
                break;
            case 2:
                AudioManager.Instance.Mute();
                break;
            case 3:
                AudioManager.Instance.Unmute();
                break;
            case 4:
                AudioListener.volume = 0;
                break;
            case 5:
                AudioListener.volume = 1;
                break;
            case 6:
                GameManager.Instance.EndGameFromMenu();
                break;
            default:
                break;
        }
    }
}
