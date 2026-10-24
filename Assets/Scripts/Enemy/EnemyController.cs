using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    public static event Action<int> OnEnemyKilled;
    public static event Action<Vector3, int> OnEnemyKilledAtPosition;
    public static event Action<float> OnEnemyReachedEnd;

    [Header("waddle animation")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float swaySpeed = 12f;
    [SerializeField] private float swayAngle = 10f;

    public EnemyDataSO Data { get; private set; }
    public EnemyHealth Health { get; private set; }
    public float PathProgress { get; private set; }

    private CubicPath path;
    private float stunTimer;
    private float slowTimer;
    private float slowMultiplier;
    private float swayTimer;
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

        PathProgress = 0f;
        stunTimer = 0f;
        slowTimer = 0f;
        slowMultiplier = 0f;
        swayTimer = 0f;
        returnToPool = onDespawn;
        isRunning = true;

        transform.rotation = Quaternion.identity;
        if (visualTransform != null)
        {
            visualTransform.localRotation = Quaternion.identity;
        }

        if (path != null && path.IsValid())
        {
            transform.position = path.GetPosition(0f);
        }

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

        float stepDistance = Data.moveSpeed * speedFactor * Time.deltaTime;
        float tangentMag = path.GetTangent(PathProgress).magnitude;

        // divide by tangent magnitude so speed stays constant through tight curves
        if (tangentMag > 0.0001f)
        {
            PathProgress += stepDistance / tangentMag;
        }

        PathProgress = Mathf.Clamp01(PathProgress);
        transform.position = path.GetPosition(PathProgress);

        // tilt only the child sprite so the hp bar stays flat
        if (visualTransform != null)
        {
            swayTimer += Time.deltaTime * speedFactor * swaySpeed;
            float angle = Mathf.Sin(swayTimer) * swayAngle;
            visualTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        if (PathProgress >= 1f)
        {
            OnEnemyReachedEnd?.Invoke(Data != null ? Data.playerDamage : 1f);
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
        OnEnemyKilledAtPosition?.Invoke(transform.position, Data.goldDrop);
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
