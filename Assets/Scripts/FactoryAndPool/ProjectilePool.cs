using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [Serializable]
    public struct PoolEntry
    {
        public ProjectileDataSO data;
        public int prewarmCount;
    }

    [Header("prewarm config")]
    [SerializeField] private List<PoolEntry> entries;

    private readonly Dictionary<ProjectileDataSO, Queue<Projectile>> pools = new();

    void Awake()
    {
        foreach (var entry in entries)
        {
            if (entry.data == null || entry.data.prefab == null) continue;

            if (!pools.ContainsKey(entry.data))
            {
                pools[entry.data] = new Queue<Projectile>();
            }

            for (int i = 0; i < entry.prewarmCount; i++)
            {
                pools[entry.data].Enqueue(CreateInstance(entry.data));
            }
        }
    }

    private Projectile CreateInstance(ProjectileDataSO data)
    {
        Projectile bullet = Instantiate(data.prefab, transform);
        bullet.gameObject.SetActive(false);
        return bullet;
    }

    public Projectile Get(ProjectileDataSO data)
    {
        if (data == null || data.prefab == null) return null;

        // fallback in case this so wasn't added to the inspector list
        if (!pools.TryGetValue(data, out Queue<Projectile> queue))
        {
            queue = new Queue<Projectile>();
            pools[data] = queue;
        }

        return queue.Count > 0 ? queue.Dequeue() : CreateInstance(data);
    }

    public void Release(Projectile bullet)
    {
        if (bullet == null || bullet.Data == null) return;

        pools[bullet.Data].Enqueue(bullet);
    }
}