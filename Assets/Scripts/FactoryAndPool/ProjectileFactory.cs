using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    [SerializeField] private ProjectilePool pool;

    public Projectile Spawn(ProjectileDataSO data, Vector3 spawnPos, Vector3 direction, float towerDamage, EnemyController target)
    {
        Projectile bullet = pool.Get(data);
        if (bullet == null) return null;

        bullet.transform.position = spawnPos;
        bullet.Setup(data, direction, towerDamage, target, pool.Release);
        return bullet;
    }
}