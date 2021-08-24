using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HoloProfileButton : MonoBehaviour
{
    [HideInInspector]
    public ProfileSelector selector;
    private Profile profile;
    public Text text;

    public DeleteProfile deleteButton;

    private int Slot;

    public List<Renderer> border;
    public Renderer background;
    private int colorShaderID = Shader.PropertyToID("_Color");
    private int emitShaderID = Shader.PropertyToID("_EmissionColor");

    [HideInInspector]
    public bool isSelected = false;

    private Color selectedColor = Color.white;
    private Color hoverColor = Color.blue;
    private Color noneColor = new Color(0, 0, 0.4f);
    private Color lightEmittingBlue = Color.blue * 1.9f;

    private void Start()
    {
        selector = GetComponentInParent<ProfileSelector>();
    }

    public void SetSlot(int slot)
    {
        this.Slot = slot;
    }

    public int GetSlot()
    {
        return Slot;
    }

    private void WriteProfile()
    {
        text.text = profile.profileName + " Lv." + profile.level;
    }

    private void WriteNewProfile()
    {
        text.text = "New Profile";
        ChangeState(0);
    }

    public void AskDeleteProfile()
    {
        selector.deleteDialog.gameObject.SetActive(true);
        selector.deleteDialog.button = this;
        selector.gameObject.SetActive(false);
    }

    public void DeleteProfile()
    {
        selector.gameObject.SetActive(true);
        if (selector.activeProfile == profile)
        {
            selector.activeProfile = null;
            GameProfile.Instance.SetProfile(null);
        }
        ChangeState(0);
        isSelected = false;
        deleteButton.SetDeleteDisable(true);
        profile = null;
        WriteNewProfile();
        selector.SelectFirstProfile();
        MenuManager.Instance.DeleteProfile(Slot);
    }

    public Profile GetProfile()
    {
        return profile;
    }

    public void SetProfile(Profile newProfile)
    {
        if (newProfile == null)
        {
            WriteNewProfile();
            deleteButton.SetDeleteDisable(true);
        }
        else
        {
            profile = newProfile;
            WriteProfile();
            deleteButton.SetDeleteDisable(false);
        }
    }

    public void ChangeState(int state)
    {
        switch (state)
        {
            case 0: // none
                {
                    text.color = noneColor;
                    background.material.SetColor(colorShaderID, noneColor);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, noneColor);
                    }
                    break;
                }
            case 1: // hover
                {
                    text.color = hoverColor;
                    background.material.SetColor(colorShaderID, new Color(0, 0, 0.5f));
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, lightEmittingBlue);
                    }
                    break;
                }
            case 2: // selected
                {
                    text.color = selectedColor;
                    background.material.SetColor(colorShaderID, Color.blue);
                    foreach (Renderer stick in border)
                    {
                        stick.material.SetColor(emitShaderID, selectedColor);
                    }
                }
                break;
            default:
                break;
        }
    }

    private void OnMouseEnter()
    {
        if (!isSelected)
            ChangeState(1);
    }

    private void OnMouseExit()
    {
        if (isSelected)
            ChangeState(2);
        else
            ChangeState(0);
    }

    private void OnMouseDown()
    {
        selector.SelectProfile(this);
    }

}
