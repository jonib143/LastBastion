using UnityEngine;

public abstract class AttackPatternSO : ScriptableObject
{
    public abstract void Execute(
        TowerController tower,
        EnemyController target,
        EnemyPool enemyPool,
        ProjectileFactory projectileFactory);
}