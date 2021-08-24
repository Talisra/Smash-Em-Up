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
            case 3: // Denymatt
                enemy = DenymattPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            case 4: // Trinati
                enemy = TrinatiPool.Instance.Get(spawnPoint, Quaternion.identity);
                break;
            default: return null;
        }
        WaveManager.Instance.AddEnemy(enemy);
        return enemy;
    }


}
