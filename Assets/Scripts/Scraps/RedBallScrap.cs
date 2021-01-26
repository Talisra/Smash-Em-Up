using UnityEngine;

public class RedBallScrap : Scraps
{
    public GameObject explosionPrefab;

    private float radialSpeed = 0f;
    private float accel = 1f;

    private void Start()
    {
        float exploDelay = Random.Range(1f, 2f);
        Invoke("Explode", exploDelay);
        Destroy(gameObject, exploDelay);
    }

    private void Explode()
    {
        audioManager.Play("Explosion_Evaporate");
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f,0.5f,0.5f), 1 * Time.deltaTime);
        radialSpeed += accel;
        transform.Rotate(0, radialSpeed * Time.deltaTime, 0, 0);
    }
}
