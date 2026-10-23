using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [Serializable]
    public struct PoolEntry
    {
        public EnemyDataSO data;
        public int prewarmCount;
    }

    [Header("prewarm config")]
    [SerializeField] private List<PoolEntry> entries;

    private readonly Dictionary<EnemyDataSO, Queue<EnemyController>> pools = new();
    public List<EnemyController> ActiveEnemies { get; } = new();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            if (entry.data == null || entry.data.prefab == null) continue;

            if (!pools.ContainsKey(entry.data))
            {
                pools[entry.data] = new Queue<EnemyController>();
            }

            for (int j = 0; j < entry.prewarmCount; j++)
            {
                pools[entry.data].Enqueue(CreateInstance(entry.data));
            }
        }
    }

    private EnemyController CreateInstance(EnemyDataSO data)
    {
        EnemyController enemy = Instantiate(data.prefab, transform);
        enemy.gameObject.SetActive(false);
        return enemy;
    }

    public EnemyController Get(EnemyDataSO data)
    {
        if (data == null || data.prefab == null) return null;

        if (!pools.TryGetValue(data, out Queue<EnemyController> queue))
        {
            queue = new Queue<EnemyController>();
            pools[data] = queue;
        }

        EnemyController enemy = queue.Count > 0 ? queue.Dequeue() : CreateInstance(data);
        ActiveEnemies.Add(enemy);
        return enemy;
    }

    public void Release(EnemyController enemy)
    {
        if (enemy == null || enemy.Data == null) return;

        ActiveEnemies.Remove(enemy);
        pools[enemy.Data].Enqueue(enemy);
    }
}