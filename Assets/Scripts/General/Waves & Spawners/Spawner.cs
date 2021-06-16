using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Enemy GetEnemyFromPool(int enemyIdx, Vector3 spawnPoint)
    {
        Enemy enemy;
        switch (enemyIdx)
        {
            case 0: // Boule
                enemy = BoulePool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            case 1: // Cuball
                enemy = CuballPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            case 2: // Cryser
                enemy = CryserPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            default: return null;
        }
        return enemy;
    }


}
