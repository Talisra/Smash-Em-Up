using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private ProfileLoader profileLoader;
    public EXPwindow expWindow;
    public ProfileSelector profileSelector;
    public WarningDialog warningDialog;
    public PerksDialog perksDialog;
    public PerksObserver perksObserver;

    // disable when starting a game
    public Camera logoCamera;
    public Button creditsButton;

    public TutorialCanvas tutorialCanvas;

    [HideInInspector]
    public bool isBusy = false;

    [HideInInspector]
    public List<Profile> profiles = new List<Profile>();
    [HideInInspector]
    public Profile selectedProfile;
    [HideInInspector]
    public int selectedProfileIdx  = -1;
    [HideInInspector]
    public bool muteSound = false;
    [HideInInspector]
    public bool muteMusic = false;

    public MainMenuMenu menu;

    public Image fade;
    public Animator transitionAnimator;

    public Animator hydraulicPressAnimator;
    public Animator cameraAnimator;
    public GameLogo gamelogo;
    public ParticleSystem pressSmoke;
    public TV_Screen_Menu tv_screen;

    public Image loadingBar;

    private float shakeTime;
    private float shakeCount = 0;
    private float shakeOffset = 0.03f; // to be synchronized with the soundtrack
    private int timesToShake = 4;
    private int numOfShakeCounter = 0;
    private bool pressPlayed = false;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        // Force Resolution
        Screen.SetResolution(1920, 1080, true);
        // Load profiles and set selected profile
        profileLoader = GetComponent<ProfileLoader>();
        LoadProfiles();
        LocalPreferences prefs = profileLoader.LoadPreferences();
        selectedProfileIdx = prefs.SelectedProfile;
        if (selectedProfileIdx != -1)
        {
            selectedProfile = profiles[selectedProfileIdx];
            if (selectedProfile.level == 0)
            {
                selectedProfileIdx = -1;
                selectedProfile = null;
                menu.SetPreferences(false, false);
            }
            else
            {
                muteSound = prefs.muteSound;
                muteMusic = prefs.muteMusic;
                menu.SetPreferences(muteSound, muteMusic);
                menu.profiler.SetProfile(selectedProfile);
                menu.profiler.UpdateUI();
            }
            if (selectedProfile != null)
            {
                Perk left = selectedProfile.activeSkillMouseLeft == -1 ? null : PerksManager.Instance.totalSkills[selectedProfile.activeSkillMouseLeft];
                Perk right = selectedProfile.activeSkillMouseRight == -1 ? null : PerksManager.Instance.totalSkills[selectedProfile.activeSkillMouseRight];
                UpdateProfileSkills(left, right);
            }
        }
        else
        {
            menu.SetPreferences(false, false);
        }
        fade.gameObject.SetActive(true);
        transitionAnimator.Play("FadeOut");

        // save local prefs from level 
        if (GameProfile.Instance.isLevel)
        {
            muteSound = GameProfile.Instance.muteSound;
            muteMusic = GameProfile.Instance.muteMusic;
        }
        // Show Exp Pool if there is exp in the GameProfile pool
        GameProfile.Instance.isLevel = false;
        if (GameProfile.Instance.GetProfile() == null)
        {
            GameProfile.Instance.SetProfile(selectedProfile);
        }
        if (selectedProfileIdx == -1)
        {
            GameProfile.Instance.ClearExpPool();
        }
        if (GameProfile.Instance.GetExpPool() > 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Invoke("ShowExpPool", .75f);
        }
        // Init audio
        SoundtrackManager.Instance.Reset();
        SoundtrackManager.Instance.StartPlaying();
        AudioManager.Instance.Reset();
        AudioManager.Instance.muteSound = false;
        shakeTime = SoundtrackManager.Instance.GetBaseTrackForShake()/4;
        shakeCount = shakeTime - shakeOffset;
    }

    public void ShowExpPool()
    {
        expWindow.gameObject.SetActive(true);
        expWindow.Init(GameProfile.Instance.GetExpPool(), GameProfile.Instance.GetProfile());
    }

    public void CheckPerks()
    {
        if (PerksManager.Instance.perksToShow.Count > 0)
        {
            perksDialog.Call();
            perksDialog.AddPerksToShow(PerksManager.Instance.perksToShow);
        }
    }

    public void ResetPerksObserver()
    {
        perksObserver.Reset();
    }

    public void UpdateProfileSkills(Perk leftSkill, Perk rightSkill)
    {
        GameProfile.Instance.SetSkills(leftSkill == null ? null : leftSkill.skill, rightSkill == null? null:  rightSkill.skill);
        selectedProfile.activeSkillMouseLeft = leftSkill == null ? -1 : leftSkill.skillIndexAtPerksManager;
        selectedProfile.activeSkillMouseRight = rightSkill == null ? -1 : rightSkill.skillIndexAtPerksManager;
        SaveProfiles();
    }

    public void LoadProfiles()
    {
        for (int i = 0; i < 3; i++)
        {
            if (profileLoader.CheckLoad(i))
            {
                profiles[i] = profileLoader.LoadProfile(i);
            }

        }
    }

    public void SaveProfiles()
    {
        for (int i = 0; i < 3; i++)
        {
            if (profiles[i].level != 0)
            {
                profileLoader.SaveProfile(profiles[i]);
            }
        }
    }

    public void DeleteProfile(int slot)
    {
        profiles[slot].level = 0;
        profileLoader.DeleteProfile(slot);
        ResetPerksObserver();
    }

    public void RefreshProfile(Profile newProfile)
    {
        if (newProfile != null)
        {
            if (newProfile.level == 0) // fake user
            {
                newProfile = null;
                selectedProfileIdx = -1;
            }
            else
            {
                selectedProfile = newProfile;
                selectedProfileIdx = newProfile.slot;
                profiles[newProfile.slot] = newProfile;
            }
            GameProfile.Instance.SetProfile(newProfile);
            perksObserver.SetProfile(newProfile);
        }
        menu.SetProfile(newProfile);
        SaveProfiles();
        menu.UpdateUI();
    }

    public void PrepareLoadLevel()
    {
        if (selectedProfileIdx == -1 || GameProfile.Instance.GetProfile() == null)
        {
            warningDialog.Call("No profile has been selected!\nSelect a profile\nor create a new one!");
            return;
        }
        numOfShakeCounter = timesToShake;
        logoCamera.gameObject.SetActive(false);
        creditsButton.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isBusy = true;
        cameraAnimator.Play("camera");
    }

    public void ActionAfterCamera()
    {
        tv_screen.turnOn();
        PerformAction(0);
    }

    IEnumerator LoadAsynchronousely(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;
            yield return null;
        }
    }

    public void PerformAction(int choice)
    {
        switch (choice)
        {
            case 0:
                {
                    numOfShakeCounter = timesToShake;
                    LocalPreferences lp = new LocalPreferences(selectedProfileIdx, muteSound, muteMusic);
                    profileLoader.SavePreferences(lp);
                    SoundtrackManager.Instance.isLevel = true;
                    GameProfile.Instance.muteSound = muteSound;
                    GameProfile.Instance.muteMusic = muteMusic;
                    GameProfile.Instance.SetProfile(selectedProfile); // double check that there is a profile
                    StartCoroutine(LoadAsynchronousely(1));
                    break;
                }
            case 1:
                {
                    SaveProfiles();
                    profileLoader.SavePreferences(new LocalPreferences(selectedProfileIdx, muteSound, muteMusic));
                    Application.Quit();
                    break;
                }
            default:
                break;
        }
    }


    public void ShowTutorialScreen(int tutorialIdx)
    {
        AudioListener.pause = true;
        Time.timeScale = 0;
        tutorialCanvas.gameObject.SetActive(true);
        tutorialCanvas.ShowTutorialScreen(tutorialIdx);
    }

    public void DismissCurrentTutorial()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        tutorialCanvas.gameObject.SetActive(false);
    }


    void Update()
    {
        shakeCount += Time.deltaTime;
        if (shakeCount >= shakeTime - shakeOffset && !pressPlayed)
        {
            hydraulicPressAnimator.Play("Hydraulic Press");
            pressPlayed = true;
        }
        if (shakeCount >= shakeTime)
        {
            shakeCount = 0;
            pressPlayed = false;
            if (numOfShakeCounter < timesToShake)
            {
                CameraEffects.Shake(0.15f, 0.1f);
            }
            pressSmoke.Play();
        }

    }
}
