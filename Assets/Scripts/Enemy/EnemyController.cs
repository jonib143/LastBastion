using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    // broadcasts gold reward when killed by towers
    public static event Action<int> OnEnemyKilled;

    public EnemyDataSO Data { get; private set; }
    public EnemyHealth Health { get; private set; }

    private CubicPath path;
    private float duration;
    private float elapsed;
    private float stunTimer;
    private Action<EnemyController> returnToPool;
    private bool isRunning;

    void Awake()
    {
        Health = GetComponent<EnemyHealth>();
    }

    public void Setup(EnemyDataSO data, CubicPath assignedPath, Action<EnemyController> onDespawn)
    {
        Data = data;
        path = assignedPath;

        float pathLength = path.GetLength();
        duration = data.moveSpeed > 0f ? pathLength / data.moveSpeed : 1f;

        elapsed = 0f;
        stunTimer = 0f;
        returnToPool = onDespawn;
        isRunning = true;

        // pass handlekilled instead of despawn so leaked enemies don't give gold
        Health.Initialize(data.maxHp, HandleKilled);
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isRunning || path == null || !path.IsValid()) return;

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        transform.position = path.GetPosition(t);

        Vector3 dir = path.GetTangent(t);
        if (dir != Vector3.zero)
        {
            transform.up = dir;
        }

        if (t >= 1f)
        {
            //var hpBar = FindFirstObjectByType<PlayerHPBar>();
            //if (hpBar != null) hpBar.TakeDamage(1f);

            Despawn();
        }
    }

    public void ApplyStun(float stunTime)
    {
        if (stunTime > stunTimer)
        {
            stunTimer = stunTime;
        }
    }

    private void HandleKilled()
    {
        if (!isRunning) return;
        OnEnemyKilled?.Invoke(Data.goldDrop);
        Despawn();
    }

    public void Despawn()
    {
        if (!isRunning) return;
        isRunning = false;
        gameObject.SetActive(false);
        returnToPool?.Invoke(this);
    }
}