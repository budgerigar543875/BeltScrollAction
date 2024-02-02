using System.Collections;
using UnityEngine;

public class Stage2EnemySpawner : StageEnemySpawnerBase
{
    [SerializeField] GameObject enemy1;
    [SerializeField] GameObject enemy2;
    [SerializeField] GameObject enemy3;
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
        SpawnEnemy(enemy3);
        SpawnEnemy(enemy2);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy3);
        SpawnEnemy(enemy1);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy3);
        SpawnEnemy(enemy2);
        yield return new WaitWhile(() => Enemies.Count > 0);
        RaiseStartScrollEvent();
        yield return new WaitWhile(() => distance < MovableArea.WIDTH);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy3);
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
        SpawnEnemy(enemy3);
        SpawnEnemy(enemy3);
        SpawnEnemy(enemy3);
        yield return new WaitWhile(() => Enemies.Count > 0);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        yield return new WaitForSeconds(3f);
        Boss _boss = SpawnEnemy(boss).GetComponent<Boss>();
        int halfHp = _boss.MaxHp / 2;
        yield return new WaitWhile(() => _boss.RestHp > halfHp);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy3);
        int quarterHp = _boss.MaxHp / 4;
        yield return new WaitWhile(() => _boss.RestHp > quarterHp);
        SpawnEnemy(enemy1);
        SpawnEnemy(enemy2);
        SpawnEnemy(enemy3);
        yield return new WaitWhile(() => Enemies.Count > 0);
        RaiseStageClearEvent();
    }

    private IEnumerator StopScroll()
    {
        yield return new WaitWhile(() => distance < 2 * MovableArea.WIDTH);
        RaiseStopScrollEvent();
    }
}
