using System;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] StageEnemySpawnerBase stageEnemySpawner;

    public event Action OnStartScroll;
    public event Action OnStopScroll;
    public event Action OnStageClear;

    private void Start()
    {
        stageEnemySpawner.OnStartScroll += StartScroll;
        stageEnemySpawner.OnStopScroll += StopScroll;
        stageEnemySpawner.OnStageClear += StageClear;
    }

    private void StartScroll()
    {
        if (OnStartScroll != null)
        {
            OnStartScroll();
        }
    }
    private void StopScroll()
    {
        if (OnStopScroll != null)
        {
            OnStopScroll();
        }
    }

    private void StageClear()
    {
        if (OnStageClear != null)
        {
            OnStageClear();
        }
    }

    public void Spawn(float moveDistance)
    {
        stageEnemySpawner.Spawn(moveDistance);
    }

    public void AdjustEnemyPosition(Vector3 diff)
    {
        stageEnemySpawner.AdjustEnemyPosition(diff);
    }

    public void GameOver()
    {
        stageEnemySpawner.EnemiesWin();
    }
}
