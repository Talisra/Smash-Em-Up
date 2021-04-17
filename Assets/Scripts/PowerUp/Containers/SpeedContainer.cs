using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedContainer : MonoBehaviour
{
    public GameObject[] cogs;
    public int[] cogsRotSpeed;

    private Rigidbody rb;
    private bool isTriggered = false;

    public float minVelocity;
    public float maxVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // hit by player: activation
        if (collision.contacts[0].thisCollider.gameObject.name == "Button")
        {
            if (!isTriggered)
            {
                collision.contacts[0].thisCollider.gameObject.transform.position -= 
                    new Vector3(0.2f * transform.localScale.x, 0, 0);
                isTriggered = true;
                Activate();
            }

        }
        // Hit the wall: reward check
        if (collision.gameObject.tag == "Unpassable")
        {
            if (rb.velocity.x > maxVelocity) // Too fast
            {

            }
            else if (rb.velocity.x < minVelocity) // Too slow
            {

            }
            else // Get reward
            {

            }
        }
    }

    private void Break()
    {
        
    }

    private void Lock()
    {

    }

    private void Open()
    {

    }

    public void Activate()
    {
        rb.isKinematic = false;
        rb.AddForce(new Vector3(-100,0,0));
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<cogs.Length; i++)
        {
            cogs[i].transform.Rotate(0, cogsRotSpeed[i] * Time.deltaTime, 0, Space.Self);
        }
    }
}
