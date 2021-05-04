﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject CeilingDoor;
    private GameManager gm;
    private AudioManager audioManager;
    private Player player;
    public float spawnDelay = 1f;

    public float boxDelay = 1.5f;
    public float lastBonusAmount = 0;

    private Animator doorsAnimator;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        doorsAnimator = CeilingDoor.GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();
        player = FindObjectOfType<Player>();
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.1f);
        OpenDoors();
        audioManager.Play("Screech");
        List<Dictionary<string, int>> wave = GetWaveDict(gm.Wave);
        List<EnemyBox> boxes = new List<EnemyBox>();
        foreach(Dictionary<string, int> subWave in wave)
        {
            boxes.Add(SummonBox(subWave));
            yield return new WaitForSeconds(boxDelay);
        }
        yield return new WaitForSeconds(0.5f);
        CloseDoors();
        audioManager.Play("DoorClosed");
        yield return new WaitForSeconds(0.3f);
        foreach (EnemyBox box in boxes)
            box.SpawnEnemies();
    }

    public IEnumerator SpawnBonuses()
    {
        lastBonusAmount = 0;
        if (Random.Range(0,100) > 0)
        {
            lastBonusAmount++;
            SummonSpeedContainer();
        }
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator SpawnWaveBonuses()
    {
        if (Random.Range(0, 100) > 0)
        {
            SummonCannon();
        }
        yield return new WaitForSeconds(0.1f);
    }

    private EnemyBox SummonBox(Dictionary<string, int> subWave)
    {
        EnemyBox box = SpawnBoxPool.Instance.Get(transform.position, Quaternion.identity);
        box.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-9, 9), -10, 0), ForceMode.VelocityChange);
        box.SetEnemiesDictionary(subWave);
        return box;
    }

    private SpeedContainer SummonSpeedContainer()
    {
        List<float> area = gm.GetGameArea();
        Vector3 spawnTarget;
        do
        {
            float x = Random.Range(area[0] + 8, area[2] - 8);
            float y = Random.Range(area[1] + 5, area[3] - 5);
            spawnTarget = new Vector3(x, y, 0);
        } while (!CheckIfEmpty(spawnTarget));
        SpeedContainer container = SpeedContainerPool.Instance.Get(
            spawnTarget, 
            Quaternion.Euler(0, spawnTarget.x < 0 ? 180 : 0, 0));
        container.SpawnSelf();
        return container;
    }
    
    private Cannon SummonCannon()
    {
        List<float> area = gm.GetGameArea();
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
        cannon.SetCannonProperties(Random.Range(1,4), Random.Range(1,3));
        cannon.Spawn();
        return cannon;
    }

    private bool CheckIfEmpty(Vector3 target)
    {
        if (Vector3.Distance(player.transform.position, target) > 5)
            return true;
        return false;
    }

    private List<Dictionary<string, int>> GetWaveDict(int wave)
    {
        return gm.GetWave(wave);
    }

    public void OpenDoors()
    {
        doorsAnimator.SetBool("Open", true);
    }

    public void CloseDoors()
    {
        doorsAnimator.SetBool("Open", false);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        spawnCounter++;
        if (spawnCounter >= spawnDelay)
        {
            Spawn();
        }
        if (doorIsOpen)
        {
            doorCounter++;
            if (doorCounter >= doorTimer)
            {
                CloseDoors();
                doorCounter = 0;
                doorIsOpen = false;
            }
        }*/

    }
}
