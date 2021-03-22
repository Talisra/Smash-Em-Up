using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPart : MonoBehaviour
{
    private bool isFading = false;
    private MeshRenderer meshRenderer;
    private Color originalColor;
    private float fadeRate = 0.0005f;
    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.materials[0].color;
    }

    public void OnEnable()
    {
        meshRenderer.materials[0].color = originalColor;
    }

    public void StartFade()
    {
        isFading = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFading)
        {
            Color color = meshRenderer.materials[0].color;
            if (color.a > 0)
            {
                color.a -= fadeRate;
                meshRenderer.materials[0].color = color;
            }
            else
            {
                isFading = false;
            }    
        }
    }
}
