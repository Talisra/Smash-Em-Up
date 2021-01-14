using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject CeilingDoor;
    public int spawnDelay = 5;
    public int spawnCounter = 30000;

    private Animator doorsAnimator;

    List<float> gameArea;

    private void Awake()
    {
        doorsAnimator = CeilingDoor.GetComponent<Animator>();
    }

    void Start()
    {
        List<float> area = FindObjectOfType<GameManager>().GetGameArea();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            OpenDoors();
            Vector3 spawnPos = new Vector3(0, transform.position.y, 0);
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity) as GameObject;
            enemy.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-400,400), -500, 0));
            yield return new WaitForSeconds(1.11f);
            CloseDoors();
        }

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
