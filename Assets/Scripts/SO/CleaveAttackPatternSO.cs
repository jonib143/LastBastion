using UnityEngine;

[CreateAssetMenu(menuName = "td/attack patterns/melee cleave")]
public class CleaveAttackPatternSO : AttackPatternSO
{
    // 180 = warrior half-circle cleave, 360 = knight full-circle spin
    [Range(10f, 360f)] public float cleaveAngle = 180f;

    public override void Execute(
        TowerController tower,
        EnemyController target,
        EnemyPool enemyPool,
        ProjectileFactory projectileFactory)
    {
        if (enemyPool == null || tower.Data.onHitEffects == null) return;
        
        Vector2 aimDir = target.transform.position - tower.transform.position;
        float centerAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        var active = enemyPool.ActiveEnemies;
        for (int i = active.Count - 1; i >= 0; i--)
        {
            EnemyController enemy = active[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            Vector2 toEnemy = enemy.transform.position - tower.transform.position;
            if (toEnemy.magnitude > tower.Data.range) continue;

            float enemyAngle = Mathf.Atan2(toEnemy.y, toEnemy.x) * Mathf.Rad2Deg;
            float delta = Mathf.Abs(Mathf.DeltaAngle(centerAngle, enemyAngle));

            if (delta <= cleaveAngle / 2f)
            {
                for (int e = 0; e < tower.Data.onHitEffects.Count; e++)
                {
                    if (tower.Data.onHitEffects[e] != null)
                    {
                        tower.Data.onHitEffects[e].Apply(enemy, enemy.transform.position, tower.Data.damage);
                    }
                }
            }
        }
    }
}