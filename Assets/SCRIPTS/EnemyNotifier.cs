using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNotifier : MonoBehaviour
{
    private Spawn spawner;

    public void SetSpawner(Spawn enemySpawner)
    {
        spawner = enemySpawner;
    }

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.NotifyEnemyKilled();
        }
    }
}
