using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    public GameObject creditsCanvas;

    public void ShowCredits()
    {
        creditsCanvas.SetActive(true);
    }

    public void DisposeCredits()
    {
        creditsCanvas.SetActive(false);
    }
}
