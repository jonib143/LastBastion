using UnityEngine;

[CreateAssetMenu(menuName = "td/effects/aoe damage")]
public class AoeDamageEffectSO : HitEffectSO
{
    public float radius = 1.5f;
    public float splashDamageMultiplier = 0.5f;

    public override void Apply(EnemyController target, Vector3 hitPos, float baseDamage)
    {
        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeInHierarchy || enemy.Health == null) continue;

            float dist = (enemy.transform.position - hitPos).magnitude;
            if (dist <= radius)
            {
                enemy.Health.TakeDamage(baseDamage * splashDamageMultiplier);
            }
        }
    }
}