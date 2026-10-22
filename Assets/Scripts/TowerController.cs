using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("data")]
    [SerializeField] private TowerDataSO towerData;

    [Header("scene references")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private ProjectileFactory projectileFactory;

    private float cooldownTimer;

    public TowerDataSO Data => towerData;

    public void Initialize(TowerDataSO data, EnemyPool pool, ProjectileFactory factory)
    {
        towerData = data;
        enemyPool = pool;
        projectileFactory = factory;
        cooldownTimer = 0f;
    }

    void Update()
    {
        if (towerData == null || towerData.attackPattern == null || enemyPool == null) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

        EnemyController target = FindTargetClosestToBase();
        if (target != null)
        {
            cooldownTimer = towerData.attackSpeed > 0f ? 1f / towerData.attackSpeed : 1f;
            towerData.attackPattern.Execute(this, target, enemyPool, projectileFactory);
        }
    }

    private EnemyController FindTargetClosestToBase()
    {
        EnemyController bestTarget = null;
        float highestProgress = -1f;

        var active = enemyPool.ActiveEnemies;
        for (int i = 0; i < active.Count; i++)
        {
            EnemyController enemy = active[i];
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            float dist = (enemy.transform.position - transform.position).magnitude;
            if (dist <= towerData.range && enemy.PathProgress > highestProgress)
            {
                highestProgress = enemy.PathProgress;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }
}