using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDraftController : MonoBehaviour
{
    public const int DraftSlotCount = 3;

    public event Action OnDraftOpened;
    public event Action OnDraftUpdated;
    public event Action OnDraftClosed;

    [Header("references")]
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private CardStorageController storageController;

    [Header("draft pool")]
    [SerializeField] private List<TowerCardSO> draftPool;

    [Header("refresh cost config")]
    [SerializeField] private int baseRefreshCost = 5;
    [SerializeField] private int refreshCostStep = 5;

    private readonly TowerCardSO[] currentOffers = new TowerCardSO[DraftSlotCount];
    private int currentRefreshCount = 0;

    public int CurrentRefreshCost => baseRefreshCost + (currentRefreshCount * refreshCostStep);

    void OnEnable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveCleared += OpenNewDraftSession;
        }
    }

    void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveCleared -= OpenNewDraftSession;
        }
    }

    void Start()
    {
        // open draft before wave 1 starts
        OpenNewDraftSession();
    }

    public void OpenNewDraftSession()
    {
        // reset escalating refresh cost for the new wave
        currentRefreshCount = 0;
        RollCards();
        OnDraftOpened?.Invoke();
        OnDraftUpdated?.Invoke();
    }

    public TowerCardSO GetOffer(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= DraftSlotCount) return null;
        return currentOffers[slotIndex];
    }

    public bool TryBuyCard(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= DraftSlotCount) return false;

        TowerCardSO card = currentOffers[slotIndex];
        if (card == null || storageController == null || GoldManager.Instance == null) return false;

        if (!storageController.HasEmptySlot()) return false;
        if (!GoldManager.Instance.TrySpendGold(card.GoldCost)) return false;

        storageController.TryAddCard(card);
        currentOffers[slotIndex] = null; // mark slot as bought
        OnDraftUpdated?.Invoke();
        return true;
    }

    public bool TryRefreshDraft()
    {
        if (GoldManager.Instance == null) return false;

        int cost = CurrentRefreshCost;
        if (!GoldManager.Instance.TrySpendGold(cost)) return false;

        currentRefreshCount++;
        RollCards();
        OnDraftUpdated?.Invoke();
        return true;
    }

    public void CloseDraft()
    {
        OnDraftClosed?.Invoke();
    }

    private void RollCards()
    {
        if (draftPool == null || draftPool.Count == 0) return;

        for (int i = 0; i < DraftSlotCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, draftPool.Count);
            currentOffers[i] = draftPool[randomIndex];
        }
    }
}