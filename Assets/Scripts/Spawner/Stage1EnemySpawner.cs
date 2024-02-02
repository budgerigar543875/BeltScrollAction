using System.Collections;
using UnityEngine;

public class Stage1EnemySpawner : StageEnemySpawnerBase
{
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject boss;

    private float distance;
    Coroutine coroutine = null;

    protected override void SpawnImpl(float moveDistance)
    {
        distance = moveDistance;
        if (coroutine == null)
        {
            coroutine = StartCoroutine(Spawn());
            StartCoroutine(StopScroll());
        }
    }

    private IEnumerator Spawn()
    {
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy2);
        yield return new WaitWhile(() => Enemies.Count > 0);
        RaiseStartScrollEvent();
        yield return new WaitWhile(() => distance < MovableArea.WIDTH);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => distance < 2 * MovableArea.WIDTH);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy2);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        yield return new WaitForSeconds(3f);
        SpawnEnemy(boss);
        yield return new WaitWhile(() => Enemies.Count > 0);
        RaiseStageClearEvent();
    }

    private IEnumerator StopScroll()
    {
        yield return new WaitWhile(() => distance < 2 * MovableArea.WIDTH);
        RaiseStopScrollEvent();
    }
}
