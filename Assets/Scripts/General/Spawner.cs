using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject CeilingDoor;
    private GameManager gm;
    public float spawnDelay = 1f;

    public float boxDelay = 1.5f;

    private Animator doorsAnimator;

    List<float> gameArea;

    private void Awake()
    {
        doorsAnimator = CeilingDoor.GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();
        List<float> area = gm.GetGameArea();
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSeconds(0.1f);
        OpenDoors();
        List<Dictionary<string, int>> wave = GetWaveDict(gm.Wave);
        List<EnemyBox> boxes = new List<EnemyBox>();
        foreach(Dictionary<string, int> subWave in wave)
        {
            boxes.Add(SummonBox(subWave));
            yield return new WaitForSeconds(boxDelay);
        }
        yield return new WaitForSeconds(0.5f);
        CloseDoors();
        foreach (EnemyBox box in boxes)
            box.SpawnEnemies();
    }

    private EnemyBox SummonBox(Dictionary<string, int> subWave)
    {
        EnemyBox box = SpawnBoxPool.Instance.Get(transform.position, Quaternion.identity);
        box.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-9, 9), -10, 0), ForceMode.VelocityChange);
        box.SetEnemiesDictionary(subWave);
        return box;
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
