using System;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance { get; private set; }

    public event Action<float, float> OnHealthChanged;
    public event Action OnPlayerDefeated;

    [Header("health config")]
    [SerializeField] private float maxHp = 20f;

    public float CurrentHp { get; private set; }
    public float MaxHp => maxHp;

    void Awake()
    {
        Instance = this;
        CurrentHp = maxHp;
    }

    void OnEnable()
    {
        EnemyController.OnEnemyReachedEnd += TakeDamage;
    }

    void OnDisable()
    {
        EnemyController.OnEnemyReachedEnd -= TakeDamage;
    }

    void Start()
    {
        OnHealthChanged?.Invoke(CurrentHp, maxHp);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || CurrentHp <= 0f) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        OnHealthChanged?.Invoke(CurrentHp, maxHp);

        if (CurrentHp <= 0f)
        {
            OnPlayerDefeated?.Invoke();
        }
    }
}
