using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "td/projectile data")]
public class ProjectileDataSO : ScriptableObject
{
    [Header("stats")]
    public Projectile prefab;
    public float speed = 8f;
    public float lifetime = 3f;

    [Header("hit detection")]
    public float hitRadius = 0.4f;
    public List<HitEffectSO> onHitEffects;
}