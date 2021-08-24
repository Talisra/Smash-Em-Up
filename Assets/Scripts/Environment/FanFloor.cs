using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanFloor : MonoBehaviour
{
    public Animator animator;

    private bool isFanFloor = true;
    public List<GameObject> AffectedObjects;
    public Vector3 ForceVector;
    public List<Cog> cogs;


    private int rematchTimes = 0;
    private WaveManager waveManager;
    public Miniboss miniboss;
    public GameObject miniboss_dead;
    public Gyroscope_dead gyroscope_dead;

    public BoxCollider fanTrigger;

    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        miniboss.gameObject.SetActive(false);
        miniboss_dead.SetActive(false);
        gyroscope_dead.gameObject.SetActive(false);
    }

    public void KillMiniboss()
    {
        miniboss.gameObject.SetActive(false);
        miniboss_dead.SetActive(true);
        gyroscope_dead.gameObject.SetActive(true);
        gyroscope_dead.gameObject.transform.position = new Vector3(0, 0, 0);
        gyroscope_dead.Break();
        waveManager.Invoke("CompleteMiniboss", 2.5f);
        rematchTimes++;
    }

    public void ChangePlatforms()
    {
        if (isFanFloor)
        {
            fanTrigger.enabled = false;
            animator.Play("FanDown");
            miniboss.gameObject.SetActive(true);
            miniboss.SetDifficulty(rematchTimes);
            miniboss_dead.SetActive(false);
            gyroscope_dead.gameObject.SetActive(false);
        }
        else
        {
            fanTrigger.enabled = true;
            animator.Play("FanUp");
            gyroscope_dead.Reset();
        }

        isFanFloor = !isFanFloor;
    }

    void OnTriggerEnter(Collider collidee)
    {
        if (collidee.gameObject.tag == "Enemy")
            AffectedObjects.Add(collidee.gameObject);
    }

    void OnTriggerExit(Collider collidee)
    {
        AffectedObjects.Remove(collidee.gameObject);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < AffectedObjects.Count; i++)
        {
            GameObject go = AffectedObjects[i];
            if (go)
            {
                go.GetComponent<Rigidbody>().AddForce(ForceVector);
            }
            else
                AffectedObjects.Remove(go);
        }
    }
}
