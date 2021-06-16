using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public Tunnel[] tunnels;
    private Player player;
    private float waveDelay = 0.1f;
    private float delayBetweenSubwaves = 3f;
    private int Wave = 0;

    private List<int> availableEnemies = new List<int>();
    private List<int> availablePowerups = new List<int>();
    private List<int> availableMapBonuses = new List<int>();

    private int activeEnemies = 0;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        tunnels = FindObjectsOfType<Tunnel>();
        player = FindObjectOfType<Player>();
        CompleteWave();
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
            case 3:
                availableEnemies.Add(1);
                break;
            case 8:
                availableEnemies.Add(2);
                break;
            case 9:
                availablePowerups.Add(1);
                break;
            case 11:
                availableMapBonuses.Add(0);
                break;
        }
    }

    private Wave GenerateWave()
    {
        Wave newWave = new Wave();
        newWave.subwaves.Add(GenerateBouleSubwave());
        if (Wave > 2)
        {
            newWave.subwaves.Add(GenerateSubwave(Wave));
        }
        if (Wave > 30)
            newWave.subwaves.Add(GenerateSubwave(Wave/2));
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

    private Subwave GenerateBouleSubwave()
    {
        Subwave subwave = new Subwave
        {
            minDelay = 1f + ((float)Wave)/10,
            maxDelay = 2f + ((float)Wave)/10,
            enemiesPerSpawn = Wave < 3 ? 1 : Random.Range(1,4)
        };
        List<int> subWaveEnemies = new List<int>();
        for (int i = 0; i < Wave/7 + 1; i++)
        {
            subWaveEnemies.Add(0);
        }
        if (Wave < 3)
            subWaveEnemies.Add(0);
        subwave.enemiesIndex = subWaveEnemies;
        return subwave;
    }

    private Subwave GenerateSubwave(int waveStrength)
    {
        Subwave subwave = new Subwave
        {
            minDelay = 3f,
            maxDelay = 7f,
            enemiesPerSpawn = Wave / 10 + 1
        };
        List<int> subWaveEnemies = new List<int>();
        subWaveEnemies.Add(Random.Range(1, availableEnemies.Count));
        for (int i=0; i<waveStrength/9; i++)
        {
            if (Random.Range(0,100) > 50)
                subWaveEnemies.Add(Random.Range(1, availableEnemies.Count));
            else
                subWaveEnemies.Add(Random.Range(0, availableEnemies.Count));
        }
        subwave.enemiesIndex = subWaveEnemies;
        return subwave;
    }

    private void CompleteWave()
    {
        Debug.Log("Completed Wave #"+Wave);
        Wave++;
        UpdateAvailableEnemies();
        StartCoroutine(SpawnWave(GenerateWave()));
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
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(Random.Range(subwave.minDelay, subwave.maxDelay));
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
        AddEnemies(wave.CountEnemies());
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
        Tunnel tunnelChosen = tunnels[Random.Range(0, tunnels.Length - 1)];
        while (tunnelChosen.isBusy)
            tunnelChosen = tunnels[Random.Range(0, tunnels.Length - 1)];
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
            float x = Random.Range(area[0] + 8, area[2] - 8);
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
        if (Vector3.Distance(player.transform.position, target) > 3)
            return true;
        return false;
    }

    public void AddEnemies(int amount)
    {
        activeEnemies += amount;
    }

    public void RemoveEnemy()
    {
        activeEnemies--;
        //Debug.Log("Enemies left: " + activeEnemies);
        if (activeEnemies == 0)
            CompleteWave();
    }
}
