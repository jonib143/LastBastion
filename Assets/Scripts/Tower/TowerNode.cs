using UnityEngine;

public class TowerNode : MonoBehaviour
{
    public TowerCardSO CurrentCard { get; private set; }
    public TowerController CurrentTower { get; private set; }
    public bool IsEmpty => CurrentTower == null;

    private TowerController previewTower;
    private TowerCardSO previewCard;

    public bool CanAcceptCard(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (draggedCard == null || draggedCard.towerData == null) return false;
        if (IsEmpty) return true;

        return mergeDb != null && mergeDb.GetMergeResult(CurrentCard, draggedCard) != null;
    }

    public void ShowPreview(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (!CanAcceptCard(draggedCard, mergeDb)) return;

        TowerCardSO cardToPreview = IsEmpty ? draggedCard : mergeDb.GetMergeResult(CurrentCard, draggedCard);
        if (cardToPreview == null || cardToPreview.towerData == null || cardToPreview.towerData.towerPrefab == null) return;
        
        if (previewTower != null && previewCard == cardToPreview) return;

        ClearPreview();

        // hide existing tower while previewing its upgraded form
        if (CurrentTower != null)
        {
            CurrentTower.gameObject.SetActive(false);
        }

        previewCard = cardToPreview;
        previewTower = Instantiate(cardToPreview.towerData.towerPrefab, transform.position, Quaternion.identity);
        
        previewTower.enabled = false;
    }

    public void ClearPreview()
    {
        if (previewTower != null)
        {
            Destroy(previewTower.gameObject);
            previewTower = null;
            previewCard = null;
        }

        if (CurrentTower != null)
        {
            CurrentTower.gameObject.SetActive(true);
        }
    }

    public bool TryConfirmDrop(TowerCardSO draggedCard, MergeDatabaseSO mergeDb)
    {
        if (!CanAcceptCard(draggedCard, mergeDb))
        {
            ClearPreview();
            return false;
        }

        if (previewTower == null)
        {
            ShowPreview(draggedCard, mergeDb);
        }

        if (previewTower == null) return false;

        if (CurrentTower != null)
        {
            Destroy(CurrentTower.gameObject);
        }

        // make the preview tower permanent and turn its update loop on
        CurrentCard = previewCard;
        CurrentTower = previewTower;
        CurrentTower.Initialize(CurrentCard.towerData);
        CurrentTower.enabled = true;

        previewTower = null;
        previewCard = null;
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