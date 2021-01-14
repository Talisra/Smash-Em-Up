using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public int menuChoice;
    public TextMesh buttonText;
    public GameObject explosionPrefab;
    public GameObject scrap;
    public GameObject hammerCursor;
    public float impactForce;
    private Renderer rend;
    private Rigidbody rb;
    private int numOfScraps = 20;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        buttonText.color = Color.red;
    }

    private void OnMouseEnter()
    {
        hammerCursor.GetComponent<MenuPlayer>().AlignY(transform.position.y);
        buttonText.color = Color.white;
        rend.material.SetFloat("_Metallic", 0);
    }

    private void OnMouseExit()
    {
        rend.material.SetFloat("_Metallic", 1);
        buttonText.color = Color.red;
    }

    private void OnMouseDown()
    {
        hammerCursor.GetComponent<MenuPlayer>().SmashMenuObject();
        rb.AddForce(new Vector3(-impactForce, 0, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unpassable")
        {
            for (int i = 0; i < numOfScraps; i++)
            {
                Instantiate(scrap, transform.position, Quaternion.identity);
            }
            GameObject explosion = Instantiate(
                explosionPrefab, transform.position, Quaternion.identity) as GameObject;

            FindObjectOfType<MenuManager>().PerformAction(menuChoice);

            FindObjectOfType<AudioManager>().Play("BrickBreak");
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
            Destroy(gameObject);

        }
    }
}
