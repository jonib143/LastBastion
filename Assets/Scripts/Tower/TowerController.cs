using UnityEngine;

public class TowerController : MonoBehaviour
{
    [Header("data")]
    [SerializeField] private TowerDataSO towerData;

    private float cooldownTimer;

    public TowerDataSO Data => towerData;

    public void Initialize(TowerDataSO data)
    {
        towerData = data;
        cooldownTimer = 0f;
    }

    void Update()
    {
        if (towerData == null || towerData.attackPattern == null || EnemyPool.Instance == null) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return;

        EnemyController target = FindTargetClosestToBase();
        if (target != null)
        {
            cooldownTimer = towerData.attackSpeed > 0f ? 1f / towerData.attackSpeed : 1f;
            towerData.attackPattern.Execute(this, target, EnemyPool.Instance, ProjectileFactory.Instance);
        }
    }

    private EnemyController FindTargetClosestToBase()
    {
        EnemyController bestTarget = null;
        float highestProgress = -1f;

        var active = EnemyPool.Instance.ActiveEnemies;
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