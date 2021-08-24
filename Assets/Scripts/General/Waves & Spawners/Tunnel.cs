using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : Spawner
{
    private GameManager gm;
    public TunnelDoor[] doors;
    public BoxCollider trigger;

    private List<Enemy> enemiesInTunnel = new List<Enemy>();

    private Vector3 spawnPoint;

    [HideInInspector]
    public bool isBusy = false;
    [HideInInspector]
    public bool isBlocked = false;
    private int sign; // -1 if left, 1 if right. helps handle the enemy spawn point

    private void Awake()
    {
        trigger = GetComponent<BoxCollider>();
        gm = FindObjectOfType<GameManager>();
        sign = transform.position.x - 0 > 0 ? 1 : -1;
        spawnPoint = new Vector3(
                transform.position.x - (trigger.bounds.size.x / 4 * sign),
                transform.position.y + (trigger.bounds.size.y / 2), 0);
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

    public IEnumerator RespawnEnemy(Enemy enemy)
    {
        isBusy = true;
        yield return new WaitForSeconds(0.1f);
        Open();
        enemy.transform.position = spawnPoint;
        enemiesInTunnel.Add(enemy);
        enemy.GetRigidbody().AddForce(new Vector3(-sign * Random.Range(10, 30), Random.Range(-10, 10), 0), ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.5f);
        while (enemiesInTunnel.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (Enemy enemyInTunnel in enemiesInTunnel)
            {
                enemyInTunnel.GetRigidbody().AddForce(new Vector3(-sign * Random.Range(10, 30), Random.Range(-10, 10), 0), ForceMode.VelocityChange);
            }
        }
        Close();
        yield return new WaitForSeconds(0.3f);
        isBusy = false;
    }

    public IEnumerator Spawn(int enemyIdx)
    {
        isBusy = true;
        yield return new WaitForSeconds(0.2f);
        Open();
        Enemy enemy = GetEnemyFromPool(enemyIdx, spawnPoint);
        enemiesInTunnel.Add(enemy);
        enemy.GetRigidbody().AddForce(new Vector3(-sign * Random.Range(10,30),Random.Range(-10,10),0), ForceMode.VelocityChange);
        yield return new WaitForSeconds(0.5f);
        while (enemiesInTunnel.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            foreach(Enemy enemyInTunnel in enemiesInTunnel)
            {
                enemyInTunnel.GetRigidbody().AddForce(new Vector3(-sign * Random.Range(10, 30), Random.Range(-10, 10), 0), ForceMode.VelocityChange);
            }
        }
        Close();
        yield return new WaitForSeconds(0.3f);
        isBusy = false;
    }

    public void Open()
    {
        foreach(TunnelDoor door in doors)
        {
            door.OpenDoors();
        }
    }

    public void Close()
    {
        foreach (TunnelDoor door in doors)
        {
            door.CloseDoors();
        }
    }
}
