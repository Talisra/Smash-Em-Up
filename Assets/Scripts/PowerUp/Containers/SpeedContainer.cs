using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedContainer : MonoBehaviour
{
    public GameObject[] cogs;
    public int[] cogsRotSpeed;
    public GameObject button;

    private Rigidbody rb;
    private Renderer bodyRenderer;
    private bool isTriggered = false;
    private bool isFading = false;
    private float bodyCutoff = 0;

    public float minVelocity;
    public float maxVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bodyRenderer = GetComponent<Renderer>();
        bodyRenderer.sharedMaterial.SetFloat("_Cutoff", bodyCutoff);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // hit by player: activation
        if (collision.contacts[0].thisCollider.gameObject.name == "Button")
        {
            if (!isTriggered)
            {
                collision.contacts[0].thisCollider.gameObject.transform.position += 
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
        button.SetActive(false);
        rb.isKinematic = false;
        rb.AddForce(new Vector3(-10,0,0), ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered)
        {
            for (int i = 0; i < cogs.Length; i++)
            {
                cogs[i].transform.Rotate(0, cogsRotSpeed[i] * Time.deltaTime, 0, Space.Self);
            }
        }
        if (isFading)
        {
            bodyRenderer.sharedMaterial.SetFloat("_Cutoff", bodyCutoff);
            bodyCutoff += Time.deltaTime / 5;
        }
    }
}
