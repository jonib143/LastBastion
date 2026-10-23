using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public static ProjectileFactory Instance { get; private set; }

    [SerializeField] private ProjectilePool projectilePool;

    void Awake()
    {
        Instance = this;
    }

    public Projectile Spawn(
        ProjectileDataSO data,
        Vector3 spawnPos,
        Vector3 direction,
        float towerDamage,
        List<HitEffectSO> effects,
        EnemyController target)
    {
        Projectile bullet = projectilePool.Get(data);
        if (bullet == null) return null;

        bullet.transform.position = spawnPos;
        bullet.Setup(data, direction, towerDamage, effects, target, EnemyPool.Instance, projectilePool.Release);
        return bullet;
    }
}