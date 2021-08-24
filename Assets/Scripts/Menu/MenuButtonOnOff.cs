using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MenuButtonOnOff : MonoBehaviour
{
    public int actionOff; // the action is handled by the menu
    public int actionOn; // the action is handled by the menu
    public GameObject buttonAnchor;
    public Image icon;
    public Sprite iconOnSprite;
    public Sprite iconOffSprite;

    private bool cursorEntered = false;
    private bool cursorClicked = false;
    [HideInInspector]
    public bool isOn;

    public Renderer rendScreen;
    public List<Renderer> rendSticklight;

    private InGameMenu menu;

    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    private Color origScreenColor;
    private Color origEmitColor;
    private Color icon_off_color = new Color(0.5f, 0, 0);
    private Color whiteEmitColor = new Color(1, 1, 1) * 2.7f;
    private Color offScreenColor = new Color(0.15f, 0, 0);
    private Color mediumRedColor = new Color(1, 0.2f, 0.2f);
    private Color lightRedEmitColor = new Color(1, 0.2f, 0.2f) * 2.7f;
    private Color darkRedEmitColor = new Color(0.3f, 0, 0);

    private void Awake()
    {
        menu = GetComponentInParent<InGameMenu>();
        origScreenColor = rendScreen.material.GetColor(colorShaderID);
        origEmitColor = rendSticklight[0].material.GetColor(emitShaderID);
    }

    private void ChangeState(int state)
    {
        switch (state)
        {
            case 0: // on
                {
                    icon.color = origEmitColor;
                    icon.sprite = iconOnSprite;
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, origEmitColor);
                    }
                    rendScreen.material.SetColor(colorShaderID, origScreenColor);
                    break;
                }
            case 1: // off
                {
                    icon.color = icon_off_color;
                    icon.sprite = iconOffSprite;
                    foreach (Renderer stick in rendSticklight)
                    {
                        stick.material.SetColor(emitShaderID, darkRedEmitColor);
                        stick.material.SetColor(colorShaderID, Color.black);
                    }
                    rendScreen.material.SetColor(colorShaderID, offScreenColor);
                    break;
                }
            case 2: // hover
                foreach (Renderer stick in rendSticklight)
                {
                    stick.material.SetColor(emitShaderID, lightRedEmitColor);
                }
                break;
        }
    }

    public void TurnOn()
    {
        isOn = true;
        ChangeState(0);
        if (menu)
            menu.Action(actionOn);
    }

    public void TurnOff()
    {
        isOn = false;
        ChangeState(1);
        if (menu)
            menu.Action(actionOff);
    }

    private void OnMouseUp()
    {
        cursorClicked = false;
        if (cursorEntered)
        {
            buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.05f);
            if (isOn)
            {
                TurnOff();
            }
            else
            {
                TurnOn();
            }
            ChangeState(2);
        }
    }

    private void OnMouseDown()
    {
        cursorClicked = true;
        buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.05f);
    }

    private void OnMouseEnter()
    {
        cursorEntered = true;
        ChangeState(2);
    }

    private void OnMouseExit()
    {
        if (isOn)
            ChangeState(0);
        else
            ChangeState(1);
        cursorEntered = false;
        if (cursorClicked)
        {
            buttonAnchor.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.05f);
        }
    }
}
