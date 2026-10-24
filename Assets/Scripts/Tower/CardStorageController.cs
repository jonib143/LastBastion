using System;
using UnityEngine;

public class CardStorageController : MonoBehaviour
{
    public const int MaxSlots = 5;

    public event Action OnStorageUpdated;

    [Header("references")]
    [SerializeField] private MergeDatabaseSO mergeDatabase;

    private readonly TowerCardSO[] slots = new TowerCardSO[MaxSlots];

    public TowerCardSO GetCard(int index)
    {
        if (index < 0 || index >= MaxSlots) return null;
        return slots[index];
    }

    [SerializeField] private TowerCardSO[] startingCards;

    void Start()
    {
        if (startingCards == null) return;
        for (int i = 0; i < startingCards.Length; i++)
        {
            TryAddCard(startingCards[i]);
        }
    }

    public bool HasEmptySlot()
    {
        for (int i = 0; i < MaxSlots; i++)
        {
            if (slots[i] == null) return true;
        }
        return false;
    }

    public bool TryAddCard(TowerCardSO card)
    {
        if (card == null) return false;

        for (int i = 0; i < MaxSlots; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = card;
                OnStorageUpdated?.Invoke();
                return true;
            }
        }

        return false;
    }

    public void RemoveCardAt(int index)
    {
        if (index < 0 || index >= MaxSlots) return;
        slots[index] = null;
        OnStorageUpdated?.Invoke();
    }

    public bool TryCombineOrMoveSlot(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return false;
        if (fromIndex < 0 || fromIndex >= MaxSlots || toIndex < 0 || toIndex >= MaxSlots) return false;

        TowerCardSO fromCard = slots[fromIndex];
        TowerCardSO toCard = slots[toIndex];

        if (fromCard == null) return false;

        // move into empty slot
        if (toCard == null)
        {
            slots[toIndex] = fromCard;
            slots[fromIndex] = null;
            OnStorageUpdated?.Invoke();
            return true;
        }

        // combining the two cards via recipe database
        if (mergeDatabase != null)
        {
            TowerCardSO merged = mergeDatabase.GetMergeResult(fromCard, toCard);
            if (merged != null)
            {
                slots[toIndex] = merged;
                slots[fromIndex] = null;
                OnStorageUpdated?.Invoke();
                return true;
            }
        }

        return false;
    }
}