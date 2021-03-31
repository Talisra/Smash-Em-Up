using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBox : MonoBehaviour, IPoolableObject
{
    public Dictionary<string, int> enemies;
    public BoxCollider boxCollider;

    private List<Transform> boxParts;
    private List<Transform> supportParts;

    // helps with reconstruction of the box
    private Vector3 startPos;
    private List<Vector3> boxStartPos;
    private List<Vector3> supportStartPos;

    private Quaternion startQart;
    private List<Quaternion> boxStartQart;
    private List<Quaternion> supportStartQart;

    public GameObject kernel;
    public GameObject box;
    public GameObject supports;

    private AudioManager audioManager;
    private GameManager gm;
    private float minYpower = 250;
    private float maxYpower = 550;
    private float minXpower = 50;
    private float maxXpower = 350;

    // Start is called before the first frame update
    void Start()
    {
        SaveStartPosition();
        audioManager = FindObjectOfType<AudioManager>();
        boxParts = new List<Transform>();
        supportParts = new List<Transform>();
        Dictionary<string, int> asd = new Dictionary<string, int>();
        gm = FindObjectOfType<GameManager>();
    }

    // record the position and quarternion of the box 
    private void SaveStartPosition()
    {
        boxStartPos = new List<Vector3>();
        supportStartPos = new List<Vector3>();
        boxStartQart = new List<Quaternion>();
        supportStartQart = new List<Quaternion>();
        startPos = transform.position;
        startQart = transform.rotation;
        foreach (Transform child in box.transform)
        {
            boxStartPos.Add(child.position);
            boxStartQart.Add(child.rotation);
        }
        foreach (Transform child in supports.transform)
        {
            supportStartPos.Add(child.position);
            supportStartQart.Add(child.rotation);
        }
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
        RemoveChildren(box.transform, boxParts);
        RemoveChildren(supports.transform, supportParts);
        boxCollider.enabled = false;
        box.transform.SetParent(null);
        supports.transform.SetParent(null);
        kernel.SetActive(true);
        kernel.SetActive(false);
        Invoke("ResetBox", 5);
    }

    private void RemoveChildren(Transform parent, List<Transform> storage)
    {
        List<Transform> childrenList = new List<Transform>();
        foreach(Transform child in parent)
        {
            childrenList.Add(child);
        }
        foreach(Transform child in childrenList)
        {
            BoxPart boxPart = child.gameObject.GetComponent<BoxPart>();
            child.SetParent(null);
            child.gameObject.AddComponent<Rigidbody>();
            boxPart.StartFade();
            storage.Add(child);
        }
    }

    private void ReconstructPart(Transform parent, List<Transform> storage, List<Vector3> positions, List<Quaternion> rotations)
    {
        List<Transform> partList = new List<Transform>();
        foreach(Transform part in storage)
        {
            partList.Add(part);
        }
        int counter = 0;
        foreach(Transform child in partList)
        {
            BoxPart boxPart = child.gameObject.GetComponent<BoxPart>();
            boxPart.gameObject.SetActive(true);
            child.SetParent(parent);
            Destroy(child.gameObject.GetComponent<Rigidbody>());
            child.transform.position = positions[counter];
            child.rotation = rotations[counter++];
        }
        storage.Clear();
    }

    private void ResetBox()
    {
        transform.position = startPos;
        transform.rotation = startQart;
        ReconstructPart(box.transform, boxParts, boxStartPos, boxStartQart);
        ReconstructPart(supports.transform, supportParts, supportStartPos, supportStartQart);
        box.transform.SetParent(this.transform);
        supports.transform.SetParent(this.transform);
        boxCollider.enabled = true;
        BackToPool();
    }

    private Vector3 CalculateForceVector()
    {
        List<float> gameArea = gm.GetGameArea();
        float x = -(transform.position.x - (gameArea[0]+gameArea[2])/2) * Random.Range(minXpower, maxXpower);
        return new Vector3(x, Random.Range(minYpower,maxYpower), 0);
    }

    public void BackToPool()
    {
        SpawnBoxPool.Instance.ReturnToPool(this);
    }
}
