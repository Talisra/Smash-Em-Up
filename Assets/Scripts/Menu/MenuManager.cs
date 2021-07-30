using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [HideInInspector]
    public bool isBusy = false;

    public Profile profile;

    public MainMenuMenu menu;

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
        transitionAnimator.Play("FadeOut");
        SoundtrackManager.Instance.Reset();
        SoundtrackManager.Instance.StartPlaying();
        AudioManager.Instance.Reset();
        AudioManager.Instance.muteSound = false;
        GameProfile.Instance.isLevel = false;
        shakeTime = SoundtrackManager.Instance.GetBaseTrackForShake()/4;
        shakeCount = shakeTime - shakeOffset;
    }

    public void RefreshProfile(Profile newProfile)
    {
        GameProfile.Instance.SetProfile(newProfile);
        profile = newProfile;
        menu.SetProfile(newProfile);
        menu.UpdateUI();
    }

    public void PrepareLoadLevel()
    {
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
                    SoundtrackManager.Instance.isLevel = true;
                    GameProfile.Instance.SetProfile(profile);
                    StartCoroutine(LoadAsynchronousely(1));
                    break;
            case 1:
                    Application.Quit();
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
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
                CameraEffects.Shake(0.2f, 0.1f);
                numOfShakeCounter++;
            }
            pressSmoke.Play();
        }

    }
}
