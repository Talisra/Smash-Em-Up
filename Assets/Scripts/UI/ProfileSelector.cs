using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ProfileSelector : MonoBehaviour
{
    public ProfileLoader profileLoader;
    public YesNoDialogDeleteProfile deleteDialog;
    public WarningDialog warningDialog;
    public Profiler_Clickable profileInBack;
    public List<HoloProfileButton> profileButtons;
    public InputField input;
    [HideInInspector]
    public Profile activeProfile = null;
    private HoloProfileButton newProfilePlace;
    private bool isClosing = false;

    private bool isInited = false;

    private void Awake()
    {
        int slotCounter = 0;
        foreach(HoloProfileButton button in profileButtons)
        {
            button.SetSlot(slotCounter++);
        }
    }

    private void OnEnable()
    {
        if (!isInited)
        {
            InitProfiles();
        }
        foreach(HoloProfileButton holoButton in profileButtons)
        {
            holoButton.gameObject.SetActive(true);
        }
        isClosing = false;
    }

    public void InitProfiles()
    {
        for (int i = 0; i < MenuManager.Instance.profiles.Count; i++)
        {
            Profile tempProfile = MenuManager.Instance.profiles[i];
            if (tempProfile.level != 0) // Initing the primitive array at MenuManager somehow creates
                                        // empty profiles with level 0, so here we know if the profile
                                        // is real (level !=0) or not
            {
                profileButtons[i].SetProfile(tempProfile);
            }
            else
                profileButtons[i].SetProfile(null);
        }
        if (MenuManager.Instance.selectedProfileIdx != -1)
        {
            SelectProfile(profileButtons[MenuManager.Instance.selectedProfileIdx]);
        }
        isInited = true;
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

    public void SelectFirstProfile() // selects the first profile available, and put no profile if there are no profiles available.
    {
        for(int i=0; i < profileButtons.Count; i++)
        {
            if (profileButtons[i].GetProfile() != null)
            {
                SelectProfile(profileButtons[i]);
                break;
            }
        }
        activeProfile = null;
        profileInBack.SetProfile(null);
        profileInBack.UpdateUI();
    }    

    public void DeleteProfile(HoloProfileButton holoButton)
    {
        holoButton.AskDeleteProfile();
        holoButton.deleteButton.ChangeState(3);
    }

    public void CreateProfile()
    {
        input.gameObject.SetActive(true);
        foreach(HoloProfileButton button in profileButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void AcceptProfile() // creates a new profile if possible
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
        Profile newprofile = new Profile(input.text, newProfilePlace.GetSlot());
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
        input.text = "";
        input.gameObject.SetActive(false);
    }

}
