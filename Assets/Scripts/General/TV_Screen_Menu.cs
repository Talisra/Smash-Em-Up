using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TV_Screen_Menu : MonoBehaviour
{
    public GameObject off;
    public GameObject on;
    public GameObject videoEffect;


    void Start()
    {
        off.SetActive(true);
        on.SetActive(false);
        videoEffect.SetActive(false);
    }

    public void turnOn()
    {
        off.SetActive(false);
        on.SetActive(true);
        videoEffect.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
