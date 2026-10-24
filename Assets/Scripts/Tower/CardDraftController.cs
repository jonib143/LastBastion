using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDraftController : MonoBehaviour
{
    public const int DraftCardCount = 3;

    public event Action OnDraftOpened;
    public event Action OnDraftStateChanged;
    public event Action OnDraftClosed;

    [Header("references")]
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private CardStorageController storageController;

    [Header("draft config")]
    [SerializeField] private List<TowerCardSO> draftPool;
    [SerializeField] private int baseRefreshCost = 10;
    [SerializeField] private int refreshCostStep = 5;

    private readonly TowerCardSO[] currentChoices = new TowerCardSO[DraftCardCount];
    private readonly bool[] purchasedSlots = new bool[DraftCardCount];

    public bool IsDraftOpen { get; private set; }
    public int CurrentRefreshCost { get; private set; }

    void OnEnable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveCleared += OpenNewDraft;
            waveSpawner.OnWaveStarted += HandleWaveStarted;
        }
    }

    void OnDisable()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveCleared -= OpenNewDraft;
            waveSpawner.OnWaveStarted -= HandleWaveStarted;
        }
    }

    void Start()
    {
        // open initial draft before wave 1 starts
        OpenNewDraft();
    }

    public TowerCardSO GetCardChoice(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= DraftCardCount) return null;
        return currentChoices[slotIndex];
    }

    public bool IsSlotPurchased(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= DraftCardCount) return true;
        return purchasedSlots[slotIndex];
    }

    public bool CanBuyCard(int slotIndex)
    {
        if (!IsDraftOpen) return false;
        if (slotIndex < 0 || slotIndex >= DraftCardCount) return false;
        if (purchasedSlots[slotIndex]) return false;

        TowerCardSO card = currentChoices[slotIndex];
        if (card == null) return false;
        if (storageController == null || !storageController.HasEmptySlot()) return false;
        if (GoldManager.Instance == null || !GoldManager.Instance.CanAfford(card.GoldCost)) return false;

        return true;
    }

    public bool CanRefreshDraft()
    {
        if (!IsDraftOpen) return false;
        if (draftPool == null || draftPool.Count == 0) return false;
        if (GoldManager.Instance == null || !GoldManager.Instance.CanAfford(CurrentRefreshCost)) return false;

        return true;
    }

    public void OpenNewDraft()
    {
        CurrentRefreshCost = baseRefreshCost;
        IsDraftOpen = true;

        RollDraftCards();

        OnDraftOpened?.Invoke();
        OnDraftStateChanged?.Invoke();
    }

    public bool TryBuyCard(int slotIndex)
    {
        if (!CanBuyCard(slotIndex)) return false;

        TowerCardSO card = currentChoices[slotIndex];
        if (!GoldManager.Instance.TrySpendGold(card.GoldCost)) return false;

        if (!storageController.TryAddCard(card))
        {
            // refund gold if storage failed unexpectedly
            GoldManager.Instance.AddGold(card.GoldCost);
            return false;
        }

        purchasedSlots[slotIndex] = true;
        OnDraftStateChanged?.Invoke();
        return true;
    }

    public bool TryRefreshDraft()
    {
        if (!CanRefreshDraft()) return false;
        if (!GoldManager.Instance.TrySpendGold(CurrentRefreshCost)) return false;

        CurrentRefreshCost += refreshCostStep;
        RollDraftCards();

        OnDraftStateChanged?.Invoke();
        return true;
    }

    public void CloseDraft()
    {
        if (!IsDraftOpen) return;

        IsDraftOpen = false;
        OnDraftClosed?.Invoke();
    }

    private void RollDraftCards()
    {
        bool hasCards = draftPool != null && draftPool.Count > 0;

        for (int i = 0; i < DraftCardCount; i++)
        {
            purchasedSlots[i] = false;

            if (hasCards)
            {
                int randomIndex = UnityEngine.Random.Range(0, draftPool.Count);
                currentChoices[i] = draftPool[randomIndex];
            }
            else
            {
                currentChoices[i] = null;
            }
        }
    }

    private void HandleWaveStarted(int currentWave, int totalWaves)
    {
        CloseDraft();
    }
}
