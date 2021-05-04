using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public float fadeRate = 0.0005f;
    private bool isFadingOut = false;
    private bool isFadingIn = false;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private BoxCollider partCollider;
    // Start is called before the first frame update
    void Awake()
    {
        partCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.materials[0].color;
    }

    public void OnEnable()
    {
        meshRenderer.materials[0].color = originalColor;
        isFadingOut = false;
        isFadingIn = false;
    }

    public void StartFadeOut()
    {
        isFadingOut = true;
        partCollider.enabled = true;
    }
    public void StartFadeIn()
    {
        Color color = meshRenderer.materials[0].color;
        color.a = 0;
        meshRenderer.materials[0].color = color;
        isFadingIn = true;
        partCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFadingOut)
        {
            Color color = meshRenderer.materials[0].color;
            if (color.a > 0)
            {
                color.a -= fadeRate;
                meshRenderer.materials[0].color = color;
            }
            else
            {
                isFadingOut = false;
                gameObject.SetActive(false);
                partCollider.enabled = false;
            }    
        }
        if (isFadingIn)
        {
            Color color = meshRenderer.materials[0].color;
            if (color.a < 1)
            {
                color.a += fadeRate;
                meshRenderer.materials[0].color = color;
            }
            else
            {
                isFadingIn = false;
                partCollider.enabled = true;
            }
        }
    }
}
