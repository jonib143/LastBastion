using UnityEngine;

[CreateAssetMenu(menuName = "td/tower data")]
public class TowerDataSO : ScriptableObject
{
    [Header("prefab")]
    public GameObject towerPrefab;

    [Header("stats")]
    public float damage = 2f;
    public float attackSpeed = 1f;
    public float range = 5f;

    [Header("ammo")]
    public ProjectileDataSO projectileData;
}