using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProfileSelector : MonoBehaviour
{
    public YesNoDialogDeleteProfile deleteDialog;
    public WarningDialog warningDialog;
    public Profiler_Clickable profileInBack;
    public List<HoloProfileButton> profileButtons;
    public InputField input;
    [HideInInspector]
    public Profile activeProfile;
    private HoloProfileButton newProfilePlace;
    private bool isClosing = false;

    private void Start()
    {
        foreach(HoloProfileButton button in profileButtons)
        {
            button.SetProfile(null);
        }
        // READ HERE THE PROFILES FROM A FILE
        SelectFirstProfile();
    }

    private void OnEnable()
    {
        isClosing = false;
        foreach (HoloProfileButton button in profileButtons)
        {
            button.gameObject.SetActive(true);
        }
    }

    public void SelectProfile(HoloProfileButton holoButton)
    {
        if (holoButton.GetProfile() == null)
        {
            newProfilePlace = holoButton;
            CreateProfile();
            return;
        }
        foreach(HoloProfileButton button in profileButtons)
        {
            if (button == holoButton)
            {
                button.ChangeState(2);
                button.isSelected = true;
                Profile selecteProfile = holoButton.GetProfile();
                activeProfile = selecteProfile;
                profileInBack.SetProfile(selecteProfile);
                profileInBack.UpdateUI();
            }
            else
            {
                button.ChangeState(0);
                button.isSelected = false;
            }
        }
    }

    private void SelectFirstProfile() // selects the first profile available, and put no profile if there are no profiles available.
    {
        for(int i=0; i < profileButtons.Count; i++)
        {
            if (profileButtons[i].GetProfile() != null)
            {
                SelectProfile(profileButtons[i]);
                break;
            }
        }
        profileInBack.SetProfile(null);
        profileInBack.UpdateUI();
    }    

    public void DeleteProfile(HoloProfileButton holoButton)
    {
        holoButton.AskDeleteProfile();
        holoButton.deleteButton.ChangeState(3);
        SelectFirstProfile();
    }

    public void CreateProfile()
    {
        input.gameObject.SetActive(true);
        foreach(HoloProfileButton button in profileButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void AcceptProfile()
    {
        this.gameObject.SetActive(true);
        foreach (HoloProfileButton button in profileButtons)
        {
            button.gameObject.SetActive(true);
        }
        if (isClosing)
        {
            return;
        }
        input.gameObject.SetActive(false);
        if (input.text.Length == 0)
        {
            return;
        }
        if (!CheckNameAvailable(input.text))
        {
            warningDialog.Call("Profile name is already in use!");
            this.gameObject.SetActive(false);
            return;
        }
        Profile newprofile = new Profile
        {
            profileName = input.text,
            level = 1
        };
        newProfilePlace.SetProfile(newprofile);
        newProfilePlace.deleteButton.SetDeleteDisable(false);
        SelectProfile(newProfilePlace);
        activeProfile = newprofile;
    }

    private bool CheckNameAvailable(string name)
    {
        foreach(HoloProfileButton profButton in profileButtons)
        {
            if (profButton.GetProfile() != null)
            {
                if (profButton.GetProfile().profileName.Equals(name))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Close()
    {
        isClosing = true;
        MenuManager.Instance.RefreshProfile(activeProfile);
        this.gameObject.SetActive(false);
        if (activeProfile == null)
        {
            warningDialog.Call("No profile has been selected!\nSelect a profile\nor create a new one!");
        }
        this.gameObject.SetActive(false);
        input.text = "";
        input.gameObject.SetActive(false);
    }

}
