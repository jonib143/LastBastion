using UnityEngine;

[CreateAssetMenu(menuName = "td/cards/tower card")]
public class TowerCardSO : CardDataSO
{
    [Header("tower payload")]
    public TowerDataSO towerData;
}