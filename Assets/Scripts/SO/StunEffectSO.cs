using UnityEngine;

[CreateAssetMenu(menuName = "td/effects/stun")]
public class StunEffectSO : HitEffectSO
{
    public float stunDuration = 1.2f;

    public override void Apply(EnemyController target, Vector3 hitPos, float baseDamage)
    {
        if (target != null)
        {
            target.ApplyStun(stunDuration);
        }
    }
}