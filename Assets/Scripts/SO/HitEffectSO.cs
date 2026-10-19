using UnityEngine;

public abstract class HitEffectSO : ScriptableObject
{
    public abstract void Apply(EnemyController target, Vector3 hitPos, float baseDamage);
}