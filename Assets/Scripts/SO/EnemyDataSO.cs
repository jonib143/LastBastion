using UnityEngine;

[CreateAssetMenu(menuName = "td/enemy data")]
public class EnemyDataSO : ScriptableObject
{
    public EnemyController prefab;
    public float maxHp = 5f;
    public float moveSpeed = 3f;
    public int goldDrop = 5;
    public float playerDamage = 1f;
}
