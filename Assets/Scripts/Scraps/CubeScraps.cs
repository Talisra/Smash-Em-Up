using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScraps : Scraps
{
    public float FadeRate = 1000f;
    private bool hasCollided = false;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            audioManager.Play("ScrapMetalHit");
        }
    }

    void Update()
    {
        Color oldCol = rend.material.color;
        Color newCol = new Color(oldCol.r, oldCol.g, oldCol.b, oldCol.a - (FadeRate * Time.deltaTime));
        rend.material.color = newCol;
        if (oldCol.a <0)
        {
            Dismiss();
        }
        if (transform.position.y < -15)
            Dismiss();
    }

    void Dismiss()
    {
        hasCollided = false;
        BackToPool();
    }
}
