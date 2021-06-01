using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Enemy GetEnemyFromPool(string enemyName, Vector3 spawnPoint)
    {
        Enemy enemy;
        switch (enemyName)
        {
            case "Cuball":
                enemy = CuballPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            case "Cryser":
                enemy = CryserPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            case "Boule":
                enemy = BoulePool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            default: return null;
        }
        return enemy;
    }
}
