using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustEffect : MonoBehaviour
{
    public float effectTime = 0.8f;
    private Rigidbody rb;
    private float scaleXYZ = 1;
    private float timeCounter = 0;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetEffect(Vector3 enemyLocation, Vector3 target)
    {
        transform.position = enemyLocation;
        //rb.AddForce((target - enemyLocation) * -1, ForceMode.VelocityChange);
        timeCounter = 0;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (timeCounter < effectTime)
        {
            timeCounter += Time.deltaTime;
            scaleXYZ += Time.deltaTime * 5;
            transform.localScale = new Vector3(scaleXYZ, scaleXYZ, scaleXYZ);
        }
        else
            gameObject.SetActive(false);
    }
}
