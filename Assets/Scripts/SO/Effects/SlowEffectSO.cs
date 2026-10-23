using UnityEngine;

[CreateAssetMenu(menuName = "td/effects/slow")]
public class SlowEffectSO : HitEffectSO
{
    [Range(0.1f, 0.9f)] public float slowPercent = 0.4f;
    public float duration = 2.5f;

    public override void Apply(EnemyController target, Vector3 hitPos, float baseDamage)
    {
        if (target != null)
        {
            target.ApplySlow(slowPercent, duration);
        }
    }
}
