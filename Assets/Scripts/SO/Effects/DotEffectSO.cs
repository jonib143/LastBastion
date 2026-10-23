using UnityEngine;

[CreateAssetMenu(menuName = "td/effects/damage over time")]
public class DotEffectSO : HitEffectSO
{
    public float damagePerTick = 0.5f;
    public float tickInterval = 0.5f;
    public float duration = 3f;

    public override void Apply(EnemyController target, Vector3 hitPos, float baseDamage)
    {
        if (target != null && target.Health != null)
        {
            target.Health.ApplyDot(damagePerTick, tickInterval, duration);
        }
    }
}