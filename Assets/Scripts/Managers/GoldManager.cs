using System;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    public event Action<int> OnGoldChanged;

    [Header("references")]
    [SerializeField] private TowerPlacementController placementController;

    [Header("economy")]
    [SerializeField] private int startingGold = 100;

    public int CurrentGold { get; private set; }

    void Awake()
    {
        Instance = this;
        CurrentGold = startingGold;
    }

    void OnEnable()
    {
        if (placementController != null)
        {
            placementController.OnTowerSold += AddGold;
        }
    }

    void OnDisable()
    {
        if (placementController != null)
        {
            placementController.OnTowerSold -= AddGold;
        }
    }

    void Start()
    {
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool CanAfford(int cost)
    {
        return CurrentGold >= cost;
    }

    public bool TrySpendGold(int cost)
    {
        if (cost < 0 || !CanAfford(cost)) return false;

        CurrentGold -= cost;
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        CurrentGold += amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }
}
