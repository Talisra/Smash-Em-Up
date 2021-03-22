using UnityEngine;

public class RedBallScrap : Scraps
{
    public GameObject explosionPrefab;

    private float radialSpeed = 0f;
    private float accel = 1f;

    private void OnEnable()
    {
        float exploDelay = Random.Range(0.5f, 1.5f);
        Invoke("Explode", exploDelay);
    }

    private void Explode()
    {
        if (isActiveAndEnabled)
        {
            audioManager.Play("Explosion_Evaporate");
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity) as GameObject;
            Destroy(explosion, explosion.GetComponent<ParticleSystem>().main.duration);
            BackToPool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f,0.5f,0.5f), 1 * Time.deltaTime);
        radialSpeed += accel;
        transform.Rotate(0, radialSpeed * Time.deltaTime, 0, 0);
    }
}
