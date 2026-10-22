using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    public static event Action<int> OnEnemyKilled;

    public EnemyDataSO Data { get; private set; }
    public EnemyHealth Health { get; private set; }
    public float PathProgress { get; private set; }

    private CubicPath path;
    private float duration;
    private float elapsed;
    private float stunTimer;
    private float slowTimer;
    private float slowMultiplier;
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
        PathProgress = 0f;
        stunTimer = 0f;
        slowTimer = 0f;
        slowMultiplier = 0f;
        returnToPool = onDespawn;
        isRunning = true;

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

        float speedFactor = 1f;
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            speedFactor = 1f - slowMultiplier;
        }

        // scale delta time instead of changing duration so enemy doesn't jump along the curve
        elapsed += Time.deltaTime * speedFactor;
        PathProgress = Mathf.Clamp01(elapsed / duration);

        transform.position = path.GetPosition(PathProgress);

        Vector3 dir = path.GetTangent(PathProgress);
        if (dir != Vector3.zero)
        {
            transform.up = dir;
        }

        if (PathProgress >= 1f)
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

    public void ApplySlow(float percent, float duration)
    {
        slowMultiplier = Mathf.Clamp(percent, 0f, 0.9f);
        if (duration > slowTimer)
        {
            slowTimer = duration;
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