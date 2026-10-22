using UnityEngine;

[CreateAssetMenu(menuName = "td/attack patterns/single shot")]
public class SingleShotPatternSO : AttackPatternSO
{
    public override void Execute(
        TowerController tower,
        EnemyController target,
        EnemyPool enemyPool,
        ProjectileFactory projectileFactory)
    {
        if (tower.Data.projectileData == null || projectileFactory == null) return;

        Vector3 dir = (target.transform.position - tower.transform.position).normalized;
        projectileFactory.Spawn(
            tower.Data.projectileData,
            tower.transform.position,
            dir,
            tower.Data.damage,
            tower.Data.onHitEffects,
            target);
    }
}