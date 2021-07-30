using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogo : MonoBehaviour
{
    private bool isShaking = false;
    float speed = 50.0f; //how fast it shakes
    float amount = 0.1f; //how much it shakes

    float shakeCounter = 0;
    float shakeDuration;

    private Vector3 startingPos;

    private void Start()
    {
        startingPos = transform.position;
    }

    public void Shake(float duration)
    {
        isShaking = true;
        shakeDuration = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            transform.position = new Vector3(
                transform.position.x + Mathf.Sin(Time.time * speed) * amount * 0.5f,
                transform.position.y,
                transform.position.z);
            shakeCounter += Time.deltaTime;
            if (shakeCounter >= shakeDuration)
            {
                transform.position = startingPos;
                isShaking = false;
                shakeCounter = 0;
            }
        }
    }
}
