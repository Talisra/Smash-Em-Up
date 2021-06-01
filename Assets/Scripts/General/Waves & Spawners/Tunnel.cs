using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : Spawner
{
    private GameManager gm;
    public TunnelDoor[] doors;
    public BoxCollider boxCollider;

    private List<Enemy> enemiesInTunnel = new List<Enemy>();

    private Vector3 spawnPoint;

    private int sign; // -1 if left, 1 if right. helps handle the enemy spawn point
    private bool doorOpen = false;

    private float pulseDelay = 0.5f;
    private float pulseCounter;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        gm = FindObjectOfType<GameManager>();
        sign = transform.position.x - 0 > 0 ? 1 : -1;
        spawnPoint = new Vector3(
                transform.position.x - (boxCollider.bounds.size.x / 4 * sign),
                transform.position.y + (boxCollider.bounds.size.y / 2), 0);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemyComponent = other.gameObject.GetComponent<Enemy>();
            enemyComponent.SetInGame();
            enemiesInTunnel.Remove(enemyComponent);
        }
    }

    public IEnumerator Spawn(string enemyString)
    {
        doorOpen = true;
        yield return new WaitForSeconds(0.1f);
        Open();
        Enemy enemy = GetEnemyFromPool(enemyString, spawnPoint);
        enemiesInTunnel.Add(enemy);
        yield return new WaitForSeconds(0.5f);
        Close();
        doorOpen = false;
        yield return new WaitForSeconds(0.3f);
    }

    public void Open()
    {
        foreach(TunnelDoor door in doors)
        {
            door.OpenDoors();
        }
        boxCollider.enabled = false;
    }

    public void Close()
    {
        foreach (TunnelDoor door in doors)
        {
            door.CloseDoors();
        }
        boxCollider.enabled = true;
    }

    private void FixedUpdate()
    {
        pulseCounter += Time.fixedDeltaTime;
        if (pulseCounter >= pulseDelay)
        {
            foreach (Enemy enemy in enemiesInTunnel)
            {
                enemy.GetComponent<Rigidbody>().AddForce(new Vector3(-sign, 0, 0), ForceMode.VelocityChange);
            }
            pulseCounter = 0;
        }

    }
}
