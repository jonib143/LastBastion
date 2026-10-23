using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "td/attack patterns/split shot")]
public class SplitShotPatternSO : AttackPatternSO
{
    public int maxTargets = 3;

    // cached 
    private readonly List<EnemyController> candidates = new();

    public override void Execute(
        TowerController tower,
        EnemyController target,
        EnemyPool enemyPool,
        ProjectileFactory projectileFactory)
    {
        if (tower.Data.projectileData == null || projectileFactory == null || enemyPool == null) return;

        candidates.Clear();
        var active = enemyPool.ActiveEnemies;

        for (int i = 0; i < active.Count; i++)
        {
            EnemyController enemy = active[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            float dist = (enemy.transform.position - tower.transform.position).magnitude;
            if (dist <= tower.Data.range)
            {
                candidates.Add(enemy);
            }
        }

        // sort descending so enemies closest to the base get targeted first
        candidates.Sort((a, b) => b.PathProgress.CompareTo(a.PathProgress));

        int shotCount = Mathf.Min(maxTargets, candidates.Count);
        for (int i = 0; i < shotCount; i++)
        {
            EnemyController lockedTarget = candidates[i];
            Vector3 dir = (lockedTarget.transform.position - tower.transform.position).normalized;

            projectileFactory.Spawn(
                tower.Data.projectileData,
                tower.transform.position,
                dir,
                tower.Data.damage,
                tower.Data.onHitEffects,
                lockedTarget);
        }
    }
}