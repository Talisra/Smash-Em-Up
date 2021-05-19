using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.Reset();
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
