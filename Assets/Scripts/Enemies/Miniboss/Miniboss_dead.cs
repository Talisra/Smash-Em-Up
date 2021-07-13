using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miniboss_dead : MonoBehaviour
{
    public GameObject aimBody;

    private float rotation;
    private float rotationThreshold = 400f;
    private float rotationAcc = 0.25f;
    private int direction;

    private void Awake()
    {
        rotation = rotationThreshold;
        direction = Random.Range(0, 100) > 50 ? 1 : -1;
    }

    private void OnEnable()
    {
        rotation = rotationThreshold;
        direction *= -1;
    }

    private void Update()
    {
        aimBody.transform.Rotate(new Vector3(0, 0, 1), rotation * direction * Time.deltaTime);
        if (rotation > 0)
            rotation -= rotationAcc;
    }
}
