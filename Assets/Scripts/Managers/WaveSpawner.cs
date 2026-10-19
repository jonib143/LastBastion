using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public event Action<int, int> OnWaveStarted; // current wave (1-based), total waves
    public event Action OnWaveCleared;
    public event Action OnAllWavesCleared;

    [Header("references")]
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private CubicPath path;

    [Header("waves")]
    [SerializeField] private List<WaveDataSO> waves;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private bool waitingForWaveClear = false;

    public int TotalWaves => waves.Count;

    public void StartNextWave()
    {
        if (isSpawning || waitingForWaveClear || currentWaveIndex >= waves.Count) return;

        OnWaveStarted?.Invoke(currentWaveIndex + 1, waves.Count);
        StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWaveRoutine(WaveDataSO wave)
    {
        isSpawning = true;
        waitingForWaveClear = true;

        foreach (var group in wave.groups)
        {
            if (group.enemyData == null) continue;

            for (int i = 0; i < group.count; i++)
            {
                enemyFactory.Spawn(group.enemyData, path);

                if (group.spawnInterval > 0f)
                {
                    yield return new WaitForSeconds(group.spawnInterval);
                }
            }

            if (group.delayAfterGroup > 0f)
            {
                yield return new WaitForSeconds(group.delayAfterGroup);
            }
        }

        isSpawning = false;
    }

    void Update()
    {
        // only check for clear after the coroutine finishes spawning the last enemy
        if (!waitingForWaveClear || isSpawning) return;

        if (enemyPool.ActiveEnemies.Count == 0)
        {
            waitingForWaveClear = false;
            currentWaveIndex++;

            if (currentWaveIndex >= waves.Count)
            {
                OnAllWavesCleared?.Invoke();
                enabled = false;
                return;
            }

            OnWaveCleared?.Invoke();
        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawGizmo(Color.cyan);
        }
    }
}