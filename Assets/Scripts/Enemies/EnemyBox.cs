using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBox : MonoBehaviour
{
    public GameObject[] enemies;

    public GameObject[] sides;
    public GameObject boxTop;

    private GameManager gm;
    private float minYpower = 350;
    private float maxYpower = 650;
    private float xPowerModifier = 50;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    public void Spawn()
    {
        foreach (GameObject enemyGO in enemies)
        {
            GameObject actual = Instantiate(enemyGO, transform.position, Quaternion.identity) as GameObject;
            Vector3 forceVector = CalculateForceVector();
            actual.GetComponent<Rigidbody>().AddForce(forceVector);
        }
    }

    private void OpenBox()
    {

    }

    private Vector3 CalculateForceVector()
    {
        List<float> gameArea = gm.GetGameArea();
        float x = (transform.position.x - gameArea[0]) * xPowerModifier;
        float y = Random.Range(minYpower, maxYpower);
        return new Vector3(x, y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
