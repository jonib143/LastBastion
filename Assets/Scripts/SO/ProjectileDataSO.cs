using UnityEngine;

[CreateAssetMenu(menuName = "td/projectile data")]
public class ProjectileDataSO : ScriptableObject
{
    [Header("flight stats")]
    public Projectile prefab;
    public float speed = 8f;
    public float lifetime = 3f;
    public bool isHoming = false;

    [Header("hit detection")]
    public float hitRadius = 0.4f;
    public float splashRadius = 0f;
}