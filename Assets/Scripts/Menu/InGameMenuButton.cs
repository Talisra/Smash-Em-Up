using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameMenuButton : MonoBehaviour
{
    public string description;
    public int action; // the action is handled by the menu
    public GameObject buttonAnchor;
    public Text text;
    public Renderer rendScreen;
    public List<Renderer> rendSticklight;

    private InGameMenu menu;

    private bool cursorEntered = false;
    private bool cursorClicked = false;

    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    private Color origScreenColor;
    private Color origEmitColor;
    private Color whiteEmitColor = new Color(1,1,1) * 2.7f;
    private Color mediumScreenColor = new Color(0.6f, 0, 0);
    private Color mediumRedColor = new Color(1, 0.2f, 0.2f);
    private Color lightRedEmitColor = new Color(1, 0.2f, 0.2f) * 2.7f;

    private void Start()
    {
        menu = GetComponentInParent<InGameMenu>();
        text.text = description;
        origScreenColor = rendScreen.material.GetColor(colorShaderID);
        origEmitColor = rendSticklight[0].material.GetColor(emitShaderID);
    }

    private void ChangeState(int state)
    {
        switch (state)
        {
            case 0: // none
                {
                    cursorEntered = false;
                    text.color = Color.red;
                    rendScreen.material.SetColor(colorShaderID, origScreenColor);
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, origEmitColor);
                    }
                    buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.025f);
                    break;
                }
            case 1: // hover
                {
                    text.color = mediumRedColor;
                    rendScreen.material.SetColor(colorShaderID, mediumScreenColor);
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, lightRedEmitColor);
                    }
                    break;
                }
            case 2: // click
                text.color = Color.white;
                rendScreen.material.SetColor(colorShaderID, Color.red);
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
            menu.Action(action);
        }
    }
}
