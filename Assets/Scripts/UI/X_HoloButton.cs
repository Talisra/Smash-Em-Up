using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class X_HoloButton : MonoBehaviour
{
    private ProfileSelector selector;
    public Text text;
    public int action;

    private bool cursorEntered = false;
    private bool cursorClicked = false;

    public List<Renderer> border;
    public Renderer background;
    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    private Color red = Color.red;
    private Color lightRed = new Color(1f, 0.5f, 0.5f);
    private Color darkRed = new Color(0.5f, 0.0f, 0.0f);
    private void Start()
    {
        selector = GetComponentInParent<ProfileSelector>();
        ChangeState(0);
    }

    private void OnEnable()
    {
        ChangeState(0);
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case 0: // none
                {
                    text.color = darkRed;
                    background.material.SetColor(colorShaderID, darkRed);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, lightRed);
                    }
                    break;
                }
            case 1: // hover
                {
                    text.color = lightRed;
                    background.material.SetColor(colorShaderID, darkRed);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, lightRed);
                    }
                    break;
                }
            case 2: // clicked
                {
                    text.color = Color.white;
                    background.material.SetColor(colorShaderID, red);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, Color.white);
                    }
                }
                break;
            default:
                break;
        }
    }


    private void OnMouseEnter()
    {
        cursorEntered = true;
        if (cursorClicked)
        {
            ChangeState(2);
        }
        else
        {
            ChangeState(1);
        }
    }

    private void OnMouseExit()
    {
        ChangeState(0);
    }

    private void OnMouseDown()
    {
        cursorClicked = true;
        ChangeState(2);
    }

    private void OnMouseUp()
    {
        cursorClicked = false;
        if (cursorEntered)
        {
            selector.Close();
        }
    }

}
