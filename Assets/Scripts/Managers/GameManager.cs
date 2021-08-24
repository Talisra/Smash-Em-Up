﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Camera menuCamera;

    public TutorialCanvas tutorialCanvas;

    public bool devMode;
    [HideInInspector]
    public bool isPaused = false;
    private bool inTutorial = false;

    private InGameMenu menu;

    public Player player;
    public SideWall LeftWall;
    public SideWall RightWall;
    public GameObject Floor;
    public GameObject Ceiling;

    public HitFlash hitFlash;

    public CeilingSpawner spawner;

    private PostProcessVolume ppv;
    private ColorGrading colorGrading;
    private Vignette vignette;
    private float BaseVignetteIntensity = 0.684f;

    private bool gameOver = false;
    private int score = 0;

    private float mapX;
    private float mapY;
    private float mapW;
    private float mapH;



    private List<float> GameArea;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        ppv.profile.TryGetSettings(out colorGrading);
        ppv.profile.TryGetSettings(out vignette);
        vignette.intensity.value = 0;
        spawner = FindObjectOfType<CeilingSpawner>();
        GameProfile.Instance.SetLevelAndCheckProfile();
        player = FindObjectOfType<Player>();
        player.InitWithProfile(GameProfile.Instance.GetProfile());
        player.AssignSkills(GameProfile.Instance.GetSkills());
        GameArea = new List<float>();
        CalculateGameArea();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menu = FindObjectOfType<InGameMenu>();
        menu.SetProfile(GameProfile.Instance.GetProfile());
        menu.SetPreferences(GameProfile.Instance.muteSound, GameProfile.Instance.muteMusic);
        StartGameEffect();
    }

    public void StartGameEffect()
    {
        CameraEffects.WhiteNoiseEffect(1.5f);
        menuCamera.gameObject.SetActive(false);
    }

    public List<Dictionary<string, int>> GetWave(int wave)
    {
        return WaveDictionary.Waves[wave];
    }

    void CalculateGameArea()
    {
        float mapX = LeftWall.transform.position.x + LeftWall.GetWidth() / 2;
        float mapY = Floor.transform.position.y + Floor.GetComponent<Collider>().bounds.size.y/2;
        float mapW = RightWall.transform.position.x - RightWall.GetWidth() / 2;
        float mapH = Ceiling.transform.position.y - Ceiling.GetComponent<Collider>().bounds.size.y/2;
        GameArea.Add(mapX);
        GameArea.Add(mapY);
        GameArea.Add(mapW);
        GameArea.Add(mapH);
    }

    public List<float> GetGameArea()
    {
        return GameArea;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddPowerup(int amount)
    {
        player.AddPowerUp(1);
    }

    public IEnumerator StartWave()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(spawner.SpawnBonuses());
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(
            spawner.lastBonusAmount > 0? 5f + spawner.lastBonusAmount*0.5f : 0.1f);
        StartCoroutine(spawner.Spawn());
        yield return new WaitForSeconds(Random.Range(3, 7));
        StartCoroutine(spawner.SpawnWaveBonuses());
    }

    public void FlashRedScreen()
    {
        hitFlash.FlashDamage();
    }

    public void SavePrefs()
    {
        GameProfile.Instance.muteSound = !menu.sound.isOn;
        GameProfile.Instance.muteMusic = !menu.music.isOn;
    }

    public void EndGame()
    {
        SavePrefs();
        gameOver = true;
        SoundtrackManager.Instance.StopMusic();
        AudioManager.Instance.PlayShutDown();
        StartCoroutine(EndNow());

        IEnumerator EndNow()
        {
            CameraEffects.EndGameEffectGlitch();
            yield return new WaitForSeconds(2.5f);
            CameraEffects.ShutDown(0);
            yield return new WaitForSeconds(0.9f);
            Cursor.visible = true;
            CameraEffects.glitchEffect.enabled = false;
            CameraEffects.glitch_digital.enabled = false;
            CameraEffects.glitch_analog.enabled = false;
            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene(0);
        }
    }

    public bool CheckGameOver()
    {
        return gameOver;
    }

    public void EndGameFromMenu()
    {
        SavePrefs();
        Time.timeScale = 1;
        AudioListener.pause = false;
        AudioManager.Instance.MenuStop("WhiteNoiseMenu");
        SoundtrackManager.Instance.StopMusic();
        menuCamera.gameObject.SetActive(false);
        StartCoroutine(EndFromMenu());
        IEnumerator EndFromMenu()
        {
            CameraEffects.ShutDown(1.45f);
            yield return new WaitForSeconds(0.45f);
            CameraEffects.glitchEffect.enabled = false;
            CameraEffects.glitch_digital.enabled = false;
            CameraEffects.glitch_analog.enabled = false;
            SceneManager.LoadScene(0);
        }
    }

    public void PauseGame()
    {
        AudioListener.pause = true;
        isPaused = true;
        Time.timeScale = 0;
        colorGrading.saturation.value = -35;
        colorGrading.contrast.value = 35;
        menuCamera.gameObject.SetActive(true);
        menu.UpdateUI();
        menu.MoveMenu(-1); // move menu down
    }

    public void UnpauseGameInit()
    {
        menu.MoveMenu(1); // move menu up
        // ^ disabling the camera mplemented at InGameMenu because there is a delay until the menu goes up
    }


    public void UnpauseGame()
    {
        menuCamera.gameObject.SetActive(false);
        AudioListener.pause = false;
        AudioManager.Instance.MenuStop("WhiteNoiseMenu");
        isPaused = false;
        Time.timeScale = 1;
        colorGrading.saturation.value = 5;
        colorGrading.contrast.value = 15;
    }

    public void ShowTutorialScreen(int tutorialIdx)
    {
        AudioListener.pause = true;
        isPaused = true;
        inTutorial = true;
        Time.timeScale = 0;
        tutorialCanvas.gameObject.SetActive(true);
        tutorialCanvas.ShowTutorialScreen(tutorialIdx);
    }

    public void DismissCurrentTutorial()
    {
        AudioListener.pause = false;
        isPaused = false;
        inTutorial = false;
        Time.timeScale = 1;
        tutorialCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (vignette.intensity.value < BaseVignetteIntensity)
        {
            vignette.intensity.value += Time.deltaTime;
            if (vignette.intensity.value >= BaseVignetteIntensity)
            {
                vignette.intensity.value = BaseVignetteIntensity;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gameOver && !inTutorial)
            {
                if (isPaused)
                {
                    UnpauseGameInit();
                }
                else
                {
                    PauseGame();
                }
            }

        }
        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                player.TakeDamage(player.GetCurrentHp(), false, false, Vector3.zero);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                spawner.SummonPowerup(0);
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                player.AddPowerUp(1);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                spawner.SummonPowerup(1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                spawner.SummonCannon();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                WaveManager.Instance.SkipWave();
            }
        }
    }
}
