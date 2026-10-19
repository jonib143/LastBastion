using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileDataSO Data { get; private set; }

    private Vector3 moveDir;
    private float baseDamage;
    private float lifeTimer;
    private EnemyController currentTarget;
    private Action<Projectile> returnToPool;
    private bool isRunning;

    public void Setup(ProjectileDataSO data, Vector3 dir, float towerDamage, EnemyController target, Action<Projectile> onDespawn)
    {
        Data = data;
        moveDir = dir.normalized;
        baseDamage = towerDamage;
        currentTarget = target;
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
                if (Data.onHitEffects != null)
                {
                    foreach (var effect in Data.onHitEffects)
                    {
                        if (effect != null)
                        {
                            effect.Apply(currentTarget, transform.position, baseDamage);
                        }
                    }
                }

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

    public void Despawn()
    {
        if (!isRunning) return;
        isRunning = false;
        gameObject.SetActive(false);
        returnToPool?.Invoke(this);
    }
}