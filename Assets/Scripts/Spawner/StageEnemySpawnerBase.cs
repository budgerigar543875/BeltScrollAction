using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageEnemySpawnerBase : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] EnemyInformation enemyInformation;

    private float moveDistance = 0;
    private List<GameObject> enemies;

    public event Action OnStartScroll;
    public event Action OnStopScroll;
    public event Action OnStageClear;

    protected List<GameObject> Enemies => enemies;

    protected virtual void Awake()
    {
        moveDistance = 0;
        enemies = new List<GameObject>();
    }

    public void Spawn(float moveDistance)
    {
        this.moveDistance += moveDistance;
        SpawnImpl(this.moveDistance);
    }

    protected abstract void SpawnImpl(float moveDistance);

    protected void RaiseStartScrollEvent()
    {
        if (OnStartScroll != null)
        {
            OnStartScroll();
        }
    }

    protected void RaiseStopScrollEvent()
    {
        if (OnStopScroll != null)
        {
            OnStopScroll();
        }
    }

    protected void RaiseStageClearEvent()
    {
        if (OnStageClear != null)
        {
            OnStageClear();
        }
    }

    protected GameObject SpawnEnemy(GameObject prefab)
    {
        float spawnX = MovableArea.MAX_X + 2;
        int spawnDir = UnityEngine.Random.Range(0, 100) % 2 == 0 ? 1 : -1;
        float spawnY = UnityEngine.Random.Range(MovableArea.MIN_Y, MovableArea.MAX_Y);
        GameObject enemy = Instantiate(prefab, new Vector3(spawnDir * spawnX, spawnY, 0f), prefab.transform.rotation);
        enemies.Add(enemy);
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        enemyBase.Target = player;
        enemyBase.OnDie += EnemyDied;
        enemyBase.OnDamage += EnemyDamage;
        return enemy;
    }

    private void EnemyDied(GameObject enemy)
    {
        enemies.Remove(enemy);
    }

    private void EnemyDamage(EnemyBase enemy)
    {
        enemyInformation.gameObject.SetActive(true);
        enemyInformation.SetName(enemy.DisplayName);
        enemyInformation.SetHp(enemy.MaxHp, enemy.RestHp);
        enemyInformation.DisplayInformation();
    }

    public void EnemiesWin()
    {
        enemies.ForEach(obj => obj.GetComponent<EnemyBase>().Victory());
    }

    public void AdjustEnemyPosition(Vector3 diff)
    {
        diff.y = 0f;
        enemies.ForEach(obj =>
        {
            // Player‚ªˆÚ“®‚µ‚½•ª‚¾‚¯‘Š‘Î“I‚ÉˆÚ“®‚·‚é
            obj.transform.position -= diff;
        });
    }
}
