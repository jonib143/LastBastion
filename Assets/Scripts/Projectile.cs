using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileDataSO Data { get; private set; }

    private Vector3 moveDir;
    private float baseDamage;
    private float lifeTimer;
    private List<HitEffectSO> hitEffects;
    private EnemyController currentTarget;
    private EnemyPool enemyPool;
    private Action<Projectile> returnToPool;
    private bool isRunning;

    public void Setup(
        ProjectileDataSO data,
        Vector3 dir,
        float towerDamage,
        List<HitEffectSO> effects,
        EnemyController target,
        EnemyPool pool,
        Action<Projectile> onDespawn)
    {
        Data = data;
        moveDir = dir.normalized;
        baseDamage = towerDamage;
        hitEffects = effects;
        currentTarget = target;
        enemyPool = pool;
        returnToPool = onDespawn;
        lifeTimer = 0f;
        isRunning = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!isRunning || Data == null) return;

        transform.position += moveDir * Data.speed * Time.deltaTime;

        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
        {
            float dist = (currentTarget.transform.position - transform.position).magnitude;
            if (dist <= Data.hitRadius)
            {
                TriggerHit(transform.position);
                Despawn();
                return;
            }
        }

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= Data.lifetime)
        {
            Despawn();
        }
    }

    private void TriggerHit(Vector3 hitPos)
    {
        if (hitEffects == null || hitEffects.Count == 0) return;

        if (Data.splashRadius > 0f && enemyPool != null)
        {
            var active = enemyPool.ActiveEnemies;
            // loop backwards in case an enemy dies 
            for (int i = active.Count - 1; i >= 0; i--)
            {
                EnemyController enemy = active[i];
                if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

                float dist = (enemy.transform.position - hitPos).magnitude;
                if (dist <= Data.splashRadius)
                {
                    ApplyEffectsToTarget(enemy, hitPos);
                }
            }
        }
        else
        {
            ApplyEffectsToTarget(currentTarget, hitPos);
        }
    }

    private void ApplyEffectsToTarget(EnemyController target, Vector3 hitPos)
    {
        for (int i = 0; i < hitEffects.Count; i++)
        {
            if (hitEffects[i] != null)
            {
                hitEffects[i].Apply(target, hitPos, baseDamage);
            }
        }
    }

    public void Despawn()
    {
        if (!isRunning) return;
        isRunning = false;
        gameObject.SetActive(false);
        returnToPool?.Invoke(this);
    }
}