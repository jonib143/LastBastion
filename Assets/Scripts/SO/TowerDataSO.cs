using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "td/tower data")]
public class TowerDataSO : ScriptableObject
{
    [Header("prefab & economy")]
    public TowerController towerPrefab;
    public int goldCost = 10;

    [Header("combat stats")]
    public float damage = 2f;
    public float attackSpeed = 1f;
    public float range = 5f;

    [Header("attack & effects")]
    public AttackPatternSO attackPattern;
    public ProjectileDataSO projectileData;
    public List<HitEffectSO> onHitEffects;
}