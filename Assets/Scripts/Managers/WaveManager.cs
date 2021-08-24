using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    private FanFloor fanFloor;

    public Tunnel[] tunnels;
    private Player player;
    private float waveDelay = 0.5f;
    private float delayBetweenSubwaves = 5f;
    public int Wave = 0;
    private bool isBossWave = false;
    private int bossWaveNumber = 10;

    private List<int> availableEnemies = new List<int>();
    private List<int> availablePowerups = new List<int>();
    private List<int> availableMapBonuses = new List<int>();

    private List<Enemy> activeEnemies = new List<Enemy>();
    private int totalEnemiesAtWave;
    private int baseEnemiesAmount = 1;
    private int subwaves = 1;


    // obstavles wave
    private int minObstacles = 1;
    private int maxObstacles = 2;
    private int obstacleChains = 1;
    private int obstacleWaveCounter = 0;
    private int obstacleWaveDuration = 3;
    private bool isObstacleWave = false;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        tunnels = FindObjectsOfType<Tunnel>();
        player = FindObjectOfType<Player>();
        fanFloor = FindObjectOfType<FanFloor>();
        //fanFloor.ChangePlatforms();
        Invoke("CompleteWave", 2.5f);
    }

    private void UpdateAvailableEnemies()
    {
        switch (Wave)
        {
            case 1:
                availableEnemies.Add(0);
                break;
            case 2:
                availablePowerups.Add(0);
                break;
            case 4:
                availableEnemies.Add(1);
                subwaves++;
                break;
            case 6:
                availablePowerups.Add(1);
                break;
            case 11:
                availableEnemies.Add(2);
                subwaves++;
                break;
            case 3: //14
                availableMapBonuses.Add(0);
                break;
            case 19:
                availableEnemies.Add(3);
                subwaves++;
                break;
            case 25:
                availableEnemies.Add(4);
                break;
        }
    }

    private void DeveloperModeAddEnemies()
    {
        switch (Wave)
        {
            case 1:
                availableEnemies.Add(3);
                break;
        }
    }

    private Wave CustomWave1()
    {
        Wave newWave = new Wave();
        Subwave subwave = new Subwave
        {
            minDelay = 1f,
            maxDelay = 2f,
            enemiesPerSpawn = 2
        };
        subwave.enemiesIndex.Add(1);
        newWave.subwaves.Add(subwave);
        return newWave;
    }

    private Wave GenerateWave(int subwavesToGenerate, int wave)
    {
        Wave newWave = new Wave();
        for (int i = 0; i < subwavesToGenerate; i++)
        {
            if (i==0)
            {
                newWave.subwaves.Add(GenerateBouleSubwave(wave > 5? wave/5 : wave));
            }
            else
            {
                newWave.subwaves.Add(GenerateSubwave(Wave%10, baseEnemiesAmount + Random.Range(0,2)));
            }
        }    
        if (availablePowerups.Count > 0)
        {
            for (int i = 0; i < Wave / 10 + 1; i++)
            {
                if (Random.Range(0, 100) > (75 - Wave / 5))
                {
                    newWave.powerUps.Add(Random.Range(0, availablePowerups.Count));
                }
            }
        }
        if (availableMapBonuses.Count > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Random.Range(0, 100) > (75 - Wave / 5))
                {
                    newWave.mapBonuses.Add(Random.Range(0, availableMapBonuses.Count));
                }
            }
        }

        return newWave;
    }

    private Subwave GenerateBouleSubwave(int amount)
    {
        Subwave subwave = new Subwave
        {
            minDelay = 1f + ((float)Wave) / 10,
            maxDelay = 2f + ((float)Wave) / 10,
            enemiesPerSpawn = 1
        };
        List<int> subWaveEnemies = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            subWaveEnemies.Add(0);
        }
        subwave.enemiesIndex = subWaveEnemies;
        return subwave;
    }

    private Subwave GenerateSubwave(int subwaveStrength, int amount) // strenth is a number between 1 to 10
    {
        Subwave subwave = new Subwave
        {
            minDelay = 3f,
            maxDelay = 7f + amount / 2,
            enemiesPerSpawn = Random.Range(1, amount/2)
        };
        List<int> subWaveEnemies = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            subWaveEnemies.Add(Random.Range((int)(availableEnemies.Count*((float)subwaveStrength/10)), availableEnemies.Count));
        }
        subwave.enemiesIndex = subWaveEnemies;
        return subwave;
    }

    public void CompleteMiniboss()
    {
        fanFloor.ChangePlatforms();
        isBossWave = false;
        Invoke("CompleteWave", 3f);
    }

    public void RespawnEnemy(Enemy enemy)
    {
        Tunnel tunnel = SelectRandomUnbusyTunnel();
        StartCoroutine(tunnel.RespawnEnemy(enemy));
    }

    public IEnumerator SpawnSubwave(Subwave subwave)
    {
        int arrayPointer = 0;
        for (int i=0; i<subwave.enemiesIndex.Count; i++)
        {
            for(int j=0; j<subwave.enemiesPerSpawn; j++)
            {
                if (arrayPointer < subwave.enemiesIndex.Count)
                {
                    StartCoroutine(SelectRandomUnbusyTunnel().Spawn(subwave.enemiesIndex[arrayPointer]));
                    arrayPointer++;
                }
                yield return new WaitForSeconds(0.25f);
            }
            float delay = Random.Range(subwave.minDelay, subwave.maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator SpawnMapBonuses(Wave wave)
    {
        foreach (int powerupType in wave.mapBonuses)
        {
            yield return new WaitForSeconds(Random.Range(3, 12));
            SummonCannon();
        }
    }

    public IEnumerator SpawnPowerups(Wave wave)
    {
        foreach(int powerupType in wave.powerUps)
        {
            yield return new WaitForSeconds(Random.Range(5, 15));
            SummonPowerup(powerupType);
        }
    }

    public IEnumerator SpawnWave(Wave wave)
    {
        yield return new WaitForSeconds(waveDelay);
        totalEnemiesAtWave = wave.CountEnemies();
        StartCoroutine(SpawnPowerups(wave));
        StartCoroutine(SpawnMapBonuses(wave));
        foreach (Subwave subwave in wave.subwaves)
        {
            StartCoroutine(SpawnSubwave(subwave));
            yield return new WaitForSeconds(delayBetweenSubwaves);
        }
    }


    private Tunnel SelectRandomUnbusyTunnel()
    {
        Tunnel tunnelChosen = tunnels[Random.Range(0, tunnels.Length)];
        while (tunnelChosen.isBusy || tunnelChosen.isBlocked)
            tunnelChosen = tunnels[Random.Range(0, tunnels.Length)];
        return tunnelChosen;
    }
    public void SpawnBonuses()
    {
        if (Random.Range(0, 100) > 66)
        {
            //lastBonusAmount++;
            //SummonSpeedContainer();
            SummonPowerup(Random.Range(0, 2));
        }
    }

    public void SummonPowerup(int type) //0: heal, 1: shield
    {
        List<float> area = GameManager.Instance.GetGameArea();
        Vector3 spawnTarget;
        do
        {
            float x = Random.Range(area[0] + 8, area[2] - 8);
            float y = Random.Range(area[1] + 5, area[3] - 5);
            spawnTarget = new Vector3(x, y, 0);
        } while (!CheckIfEmpty(spawnTarget));
        switch (type)
        {
            case 0:
                {
                    HealthUp healp = HealthUpPool.Instance.Get(
                        spawnTarget,
                        Quaternion.identity);
                    break;
                }
            case 1:
                {
                    ShieldUp shieldp = ShieldUpPool.Instance.Get(
                        spawnTarget,
                        Quaternion.identity);
                    shieldp.SetDuration(10);
                    break;
                }
        }
    }

    public Cannon SummonCannon()
    {
        if (GameManager.Instance.CheckGameOver())
            return null;
        List<float> area = GameManager.Instance.GetGameArea();
        Vector3 spawnTarget;
        do
        {
            float x = Random.Range(area[0] + 5, area[2] - 5);
            float y = Random.Range(area[1] + 5, area[3] - 5);
            spawnTarget = new Vector3(x, y, 0);

        } while (!CheckIfEmpty(spawnTarget));
        Cannon cannon = CannonPool.Instance.Get(
            spawnTarget,
            Quaternion.identity);
        cannon.SetCannonProperties(Random.Range(1, 4), Random.Range(1, 3));
        cannon.Spawn();
        return cannon;
    }

    private bool CheckIfEmpty(Vector3 target)
    {
        if (!player)
        {
            return true;
        }
        if (Vector3.Distance(player.transform.position, target) > 5)
            return true;
        return false;
    }

    public void AddEnemy(Enemy enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        totalEnemiesAtWave--;
        //Debug.Log("Enemies left: " + activeEnemies.Count);
        if (totalEnemiesAtWave == 0)
        {
            CompleteWave();
        }
    }

    public void SkipWave()
    {
        if (!isBossWave)
        {
            //StopAllCoroutines();
            totalEnemiesAtWave = 0;
            //Debug.Log("total enemies " + activeEnemies.Count);
            for (int i=activeEnemies.Count-1; i>=0 ; i--)
            {
                //Debug.Log("Destroying " + activeEnemies[i].name + "...");
                activeEnemies[i].InstantKill();
            }
            activeEnemies.Clear();
            CompleteWave();
        }
    }
    public void MiniBossWave()
    {
        isBossWave = true;
        AIObstacleManager.Instance.ClearAll(); // clear all AIO that are in the level so there are no aio in boss wave
        fanFloor.ChangePlatforms();
    }

    public void ManageObstacleWave()
    {
        if (Wave == 26 || Wave == 35)
        {
            obstacleChains++;
        }
        if (Wave == 13)
        {
            maxObstacles++;
        }
        if (Wave == 21)
        {
            minObstacles++;
        }
        obstacleWaveCounter++;
        if (obstacleWaveCounter > obstacleWaveDuration)
        {
            isObstacleWave = true;
            obstacleWaveCounter = 0;
            StartCoroutine(NewObstacleWave());
            PushPlayerDown();
        }
    }

    public void PushPlayerDown()
    {
        player.TakeControl();
        player.PullDown();
    }

    public IEnumerator NewObstacleWave()
    {
        AIObstacleManager.Instance.ClearAll();
        yield return new WaitForSeconds(1); // wait for the obstacles to disappear
        AIObstacleManager.Instance.ObstacleWave(obstacleChains, minObstacles, maxObstacles);
    }
    public void CompleteObstacleWave()
    {
        isObstacleWave = false;
        CompleteWave();
        player.GiveControl();
    }

    private void CompleteWave()
    {
        if (Wave == 0)
        {
            GameProfile.Instance.ShowTutorialForPlayer(0);
        }
        if (GameProfile.Instance.GetProfile().maxCombo > 0)
        {
            GameProfile.Instance.ShowTutorialForPlayer(2);
        }
        if (Wave > 10 && Wave % bossWaveNumber != 0) // obstacle wave
        {
            ManageObstacleWave();
            if (isObstacleWave)
                return;
        }

        Wave++;
        UpdateAvailableEnemies();
        if (Wave % bossWaveNumber != 0)
        {
            StartCoroutine(SpawnWave(GenerateWave(subwaves, Wave)));
            //DeveloperModeAddEnemies();
            //StartCoroutine(SpawnWave(CustomWave1()));
        }
        else
            MiniBossWave();

    }
}
