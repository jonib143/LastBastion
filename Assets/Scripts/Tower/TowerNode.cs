using UnityEngine;

public class TowerNode : MonoBehaviour
{
    public TowerCardSO CurrentCard { get; private set; }
    public TowerController CurrentTower { get; private set; }
    public bool IsEmpty => CurrentTower == null;

    public bool CanAcceptCard(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (draggedCard == null || draggedCard.towerData == null) return false;
        if (IsEmpty) return true;

        return mergeDb != null && mergeDb.GetMergeResult(CurrentCard, draggedCard) != null;
    }

    // for range preview
    public TowerDataSO GetResultingTowerData(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (draggedCard == null) return null;
        if (IsEmpty) return draggedCard.towerData;

        if (mergeDb != null)
        {
            TowerCardSO merged = mergeDb.GetMergeResult(CurrentCard, draggedCard);
            if (merged != null) return merged.towerData;
        }

        return null;
    }

    public bool TryApplyCard(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (!CanAcceptCard(draggedCard, mergeDb)) return false;

        TowerCardSO cardToSpawn = draggedCard;

        if (!IsEmpty)
        {
            cardToSpawn = mergeDb.GetMergeResult(CurrentCard, draggedCard);
            Destroy(CurrentTower.gameObject);
        }

        CurrentCard = cardToSpawn;
        CurrentTower = Instantiate(cardToSpawn.towerData.towerPrefab, transform.position, Quaternion.identity, transform);
        CurrentTower.Initialize(cardToSpawn.towerData);
        return true;
    }

    public bool TrySell(out int refundedGold)
    {
        refundedGold = 0;
        if (IsEmpty || CurrentCard == null) return false;

        refundedGold = CurrentCard.GoldCost;
        Destroy(CurrentTower.gameObject);
        CurrentTower = null;
        CurrentCard = null;
        return true;
    }
}