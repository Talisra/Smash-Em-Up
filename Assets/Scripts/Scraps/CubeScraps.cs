using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScraps : Scraps
{
    public float FadeRate = 100f;
    private bool hasCollided = false;

    private float alpha = 1;
    private Color origColor;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        origColor = rend.material.color;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCollided)
        {
            hasCollided = true;
            AudioManager.Instance.Play("ScrapMetalHit");
        }
    }

    private void OnEnable()
    {
        rend.material.color = origColor;
        alpha = 1;
    }

    void Update()
    {
        alpha -= FadeRate * Time.deltaTime;
        Color newCol = new Color(origColor.r, origColor.g, origColor.b, alpha);
        rend.material.color = newCol;
        if (alpha < 0)
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
