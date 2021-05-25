using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Animator transitionAnimator;
    // Start is called before the first frame update
    void Start()
    {
        transitionAnimator.Play("FadeOut");
        AudioManager.Instance.Reset();
        AudioManager.Instance.muteSound = false;
        AudioManager.Instance.StartPlaying();
    }

    public void PerformAction(int choice)
    {
        StartCoroutine(Wait(choice));

    }

    IEnumerator Wait(int choice)
    {
        yield return new WaitForSeconds(1.6f);
        switch (choice)
        {
            case 0:
                    AudioManager.Instance.isLevel = true;
                    SceneManager.LoadScene(1);
                    break;
            case 2:
                    Application.Quit();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
