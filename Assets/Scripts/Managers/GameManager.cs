using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool devMode;

    public Profile profile;
    public Player player;
    public SideWall LeftWall;
    public SideWall RightWall;
    public GameObject Floor;
    public GameObject Ceiling;

    public HitFlash hitFlash;

    public CeilingSpawner spawner;



    public int Wave = 1;

    private int enemiesCounter = 0;

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
        spawner = FindObjectOfType<CeilingSpawner>();
        player = FindObjectOfType<Player>();
        player.AssignSkills(profile.skills);
        GameArea = new List<float>();
        CalculateGameArea();
        Cursor.visible = true;
        //StartCoroutine(SpawnWave());
    }
    /*
    public IEnumerator SpawnWave()
    {
        List<Dictionary<string, int>> wave = GetWave(Wave);
        foreach (Dictionary<string, int> subWave in wave)
        {
            foreach (string enemy in subWave.Keys)
            {
                for (int i=0; i<subWave[enemy]; i++)
                {
                    enemiesCounter++;
                }
            }
        }
        foreach (Dictionary<string, int> subWave in wave)
        {
            foreach (string enemy in subWave.Keys)
            {
                for (int i = 0; i < subWave[enemy]; i++)
                {
                    StartCoroutine(SelectRandomUnbusyTunnel().Spawn(enemy));
                    yield return new WaitForSeconds(Random.Range(1, 5));
                }
            }
        }
    }*/

    public void AddEnemy()
    {
        enemiesCounter++;
    }

    public void RemoveEnemy()
    {
        enemiesCounter--;
        if (enemiesCounter == 0)
            CompleteWave();
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

    public void AddScore()
    {
        player.AddPowerUp(1);
    }

    public void CompleteWave()
    {
        Wave++;
        //StartCoroutine(SpawnWave());
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
    public void EndGame()
    {
        gameOver = true;
        SoundtrackManager.Instance.StopMusic();
        AudioManager.Instance.PlayShutDown();
        StartCoroutine(EndNow());
        IEnumerator EndNow()
        {
            CameraEffects.EndGameEffectGlitch();
            yield return new WaitForSeconds(2.5f);
            CameraEffects.ShutDown();
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SoundtrackManager.Instance.StopMusic();
            Cursor.visible = true;
            SceneManager.LoadScene(0);
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
