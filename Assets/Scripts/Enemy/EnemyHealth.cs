using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float currentHp;
    private float maxHp;
    private Action onDeath;

    private float dotTimer;
    private float dotTickTimer;
    private float dotTickInterval;
    private float dotDamage;

    public void Initialize(float maxHealth, Action deathCallback)
    {
        maxHp = maxHealth;
        currentHp = maxHp;
        onDeath = deathCallback;
        dotTimer = 0f;
    }

    void Update()
    {
        if (dotTimer <= 0f) return;

        dotTimer -= Time.deltaTime;
        dotTickTimer -= Time.deltaTime;

        if (dotTickTimer <= 0f)
        {
            dotTickTimer = dotTickInterval;
            TakeDamage(dotDamage);
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHp <= 0f) return;

        currentHp -= amount;
        if (currentHp <= 0f)
        {
            onDeath?.Invoke();
        }
    }

    public void ApplyDot(float damagePerTick, float interval, float duration)
    {
        dotDamage = damagePerTick;
        dotTickInterval = interval;
        dotTimer = duration;
        dotTickTimer = interval;
    }
}