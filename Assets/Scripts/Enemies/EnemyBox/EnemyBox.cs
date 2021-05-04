using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBox : ReconstructableObject
{
    public BoxCollider boxCollider;
    public Dictionary<string, int> enemies;

    private GameManager gm;
    private float minYpower = 250;
    private float maxYpower = 550;
    private float minXpower = 50;
    private float maxXpower = 350;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        Dictionary<string, int> asd = new Dictionary<string, int>();
        gm = FindObjectOfType<GameManager>();
        boxCollider = GetComponent<BoxCollider>();
    }  

    public void SetEnemiesDictionary(Dictionary<string, int> dict)
    {
        enemies = dict;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" || collision.gameObject.tag == "EnemyBox")
            audioManager.Play("BoxImpact");
        if (collision.gameObject.tag == "Player")
        {
            audioManager.Play("MetalCollision");
            Player player = collision.gameObject.GetComponent<Player>();
            if (player.collidingFloor)
                player.Squash(this.gameObject);
            else if(player.collidingBoxesID.Count > 1)
                player.Squash(this.gameObject);
        }
    }

    private void TakeEnemyFromPool(string enemyName)
    {
        Enemy enemy;
        switch (enemyName)
        {
            case "Cuball":
                enemy = CuballPool.Instance.Get(transform.position, Quaternion.identity);
                break;
            case "Cryser":
                enemy = CryserPool.Instance.Get(transform.position, Quaternion.identity);
                break;
            default: return;
        }
        Vector3 forceVector = CalculateForceVector();
        enemy.GetComponent<Rigidbody>().AddForce(forceVector);
        gm.AddEnemy();
    }

    public void SpawnEnemies()
    {
        audioManager.Play("BoxOpen");
        foreach (string enemyString in enemies.Keys)
        {
            for (int i = 0; i < enemies[enemyString]; i++)
            {
                TakeEnemyFromPool(enemyString);
            }
        }
        Break();
        boxCollider.enabled = false;
        Invoke("Reset", 5);
    }

    protected override void Reset()
    {
        base.Reset();
        boxCollider.enabled = true;
    }

    private Vector3 CalculateForceVector()
    {
        List<float> gameArea = gm.GetGameArea();
        float x = -(transform.position.x - (gameArea[0]+gameArea[2])/2) * Random.Range(minXpower, maxXpower);
        return new Vector3(x, Random.Range(minYpower,maxYpower), 0);
    }

    public override void BackToPool()
    {
        SpawnBoxPool.Instance.ReturnToPool(this);
    }
}
