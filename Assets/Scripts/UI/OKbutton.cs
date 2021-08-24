﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class OKbutton : MonoBehaviour
{
    public string description;
    public GameObject buttonAnchor;
    public Text text;
    public Renderer rendScreen;
    public List<Renderer> rendSticklight;

    private WarningDialog parentDialog;
    private bool cursorEntered = false;
    private bool cursorClicked = false;

    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    private Color origScreenColor;
    private Color origEmitColor;
    public Color whiteEmitColor = new Color(1, 1, 1) * 2.7f;
    public Color mediumColor = new Color(0.9f, 0.7f, 0f);
    public Color baseColor = new Color(1, 0.4f, 0);
    public Color darkColor = new Color(1, 0.2f, 0);
    public Color lightColor = new Color(1, 0.8f, 0);

    private void Start()
    {
        parentDialog = GetComponentInParent<WarningDialog>();
        origScreenColor = rendScreen.material.GetColor(colorShaderID);
        origEmitColor = rendSticklight[0].material.GetColor(emitShaderID);
        ChangeState(0);
    }

    private void OnEnable()
    {
        ChangeState(0);
    }

    private void ChangeState(int state)
    {
        switch (state)
        {
            case 0: // none
                {
                    cursorEntered = false;
                    text.color = mediumColor;
                    rendScreen.material.SetColor(colorShaderID, darkColor);
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, origEmitColor);
                    }
                    buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.025f);
                    break;
                }
            case 1: // hover
                {
                    text.color = lightColor;
                    rendScreen.material.SetColor(colorShaderID, baseColor);
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, origEmitColor);
                    }
                    break;
                }
            case 2: // click
                text.color = Color.white;
                rendScreen.material.SetColor(colorShaderID, lightColor);
                foreach (Renderer stick in rendSticklight)
                {
                    stick.material.SetColor(emitShaderID, whiteEmitColor);
                }
                buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.025f);
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
            parentDialog.OK();
        }
    }

}