using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [SerializeField] private EnemyPool pool;

    public EnemyController Spawn(EnemyDataSO data, CubicPath path)
    {
        if (path == null || !path.IsValid()) return null;

        EnemyController enemy = pool.Get(data);
        if (enemy == null) return null;

        enemy.transform.position = path.p0.position;
        enemy.Setup(data, path, pool.Release);
        return enemy;
    }
}