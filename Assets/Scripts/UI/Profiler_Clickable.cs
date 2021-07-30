using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Profiler_Clickable : Profiler
{
    public Renderer background;
    public GameObject effect;
    public GameObject profilerSelector;
    private Color orig;

    protected override void Start()
    {
        base.Start();
        orig = background.material.color;
    }

    private void OnMouseEnter()
    {
        effect.SetActive(true);
        background.material.SetColor("_Color", Color.blue);
    }

    private void OnMouseExit()
    {
        effect.SetActive(false);
        background.material.SetColor("_Color", orig);
    }

    private void OnMouseDown()
    {
        profilerSelector.SetActive(true);
    }

}
