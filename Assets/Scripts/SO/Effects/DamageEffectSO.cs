using UnityEngine;

[CreateAssetMenu(menuName = "td/effects/direct damage")]
public class DamageEffectSO : HitEffectSO
{
    public float damageMultiplier = 1f;

    public override void Apply(EnemyController target, Vector3 hitPos, float baseDamage)
    {
        if (target != null && target.Health != null)
        {
            target.Health.TakeDamage(baseDamage * damageMultiplier);
        }
    }
}