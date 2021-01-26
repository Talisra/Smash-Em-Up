using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeScraps : Scraps
{
    public float FadeRate = 1000f;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioManager.Play("Tap");
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
        Destroy(gameObject);
    }
}
