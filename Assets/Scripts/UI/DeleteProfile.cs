using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DeleteProfile : MonoBehaviour
{
    private ProfileSelector selector;
    private HoloProfileButton parentButton;
    public Image icon;

    private bool isDisabled = false;
    private bool cursorEntered = false;
    private bool cursorClicked = false;

    public List<Renderer> border;
    public Renderer background;
    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    private Color red = Color.red;
    private Color lightRed = new Color(1f, 0.5f, 0.5f);
    private Color darkRed = new Color(0.5f, 0.2f, 0.2f);
    private Color veryDarkBlue = new Color(0, 0, 0.2f);
    private Color noneColor = new Color(0, 0, 0.4f);

    private void Start()
    {
        selector = GetComponentInParent<ProfileSelector>();
        parentButton = GetComponentInParent<HoloProfileButton>();
    }

    public void SetDeleteDisable(bool disabled)
    {
        if (disabled)
        {
            ChangeState(3);
        }
        else
        {
            ChangeState(0);
        }
        isDisabled = disabled;
    }

    public void ChangeState(int state)
    {
        if (isDisabled)
            return;
        switch (state)
        {
            case 0: // none
                {
                    icon.color = darkRed;
                    background.material.SetColor(colorShaderID, noneColor);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, darkRed);
                    }
                    break;
                }
            case 1: // hover
                {
                    icon.color = Color.red;
                    background.material.SetColor(colorShaderID, darkRed);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, Color.red);
                    }
                    break;
                }
            case 2: // clicked
                {
                    icon.color = Color.white;
                    background.material.SetColor(colorShaderID, red);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, Color.white);
                    }
                    break;
                }
            case 3: // disabled
                {
                    icon.color = noneColor;
                    background.material.SetColor(colorShaderID, veryDarkBlue);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, noneColor);
                    }
                    break;
                }
            default:
                break;
        }
    }


    private void OnMouseEnter()
    {
        if (isDisabled)
        {
            return;
        }
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
        if (isDisabled)
        {
            return;
        }
        ChangeState(0);
    }

    private void OnMouseDown()
    {
        if (isDisabled)
        {
            return;
        }
        cursorClicked = true;
        ChangeState(2);
    }

    private void OnMouseUp()
    {
        if (isDisabled)
        {
            return;
        }
        cursorClicked = false;
        if (cursorEntered)
        {
            selector.DeleteProfile(parentButton);
        }
    }
}
