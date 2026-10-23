using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public event Action<Vector3, bool, float, Sprite> OnCardPreviewUpdated;
    public event Action<Vector3, bool> OnSellPreviewUpdated;
    public event Action OnPreviewHidden;
    public event Action<int> OnTowerSold; // broadcasts refunded gold

    [Header("data")]
    [SerializeField] private MergeDatabaseSO mergeDatabase;

    [Header("scene nodes")]
    [SerializeField] private List<TowerNode> nodes;
    [SerializeField] private float nodeSnapRadius = 1.2f;

    public MergeDatabaseSO MergeDatabase => mergeDatabase;

    public void UpdateCardDrag(TowerCardSO draggedCard, Vector3 mouseWorldPos)
    {
        if (draggedCard == null || draggedCard.towerData == null)
        {
            OnPreviewHidden?.Invoke();
            return;
        }

        TowerNode closestNode = FindClosestNode(mouseWorldPos);
        if (closestNode != null)
        {
            bool canAccept = closestNode.CanAcceptCard(draggedCard, mergeDatabase);
            TowerDataSO previewData = closestNode.GetResultingTowerData(draggedCard, mergeDatabase);

            float previewRange = previewData != null ? previewData.range : draggedCard.towerData.range;
            Sprite previewSprite = GetTowerSprite(previewData != null ? previewData : draggedCard.towerData);

            OnCardPreviewUpdated?.Invoke(closestNode.transform.position, canAccept, previewRange, previewSprite);
        }
        else
        {
            Sprite previewSprite = GetTowerSprite(draggedCard.towerData);
            OnCardPreviewUpdated?.Invoke(mouseWorldPos, false, draggedCard.towerData.range, previewSprite);
        }
    }

    public bool TryDropCard(TowerCardSO draggedCard, Vector3 mouseWorldPos)
    {
        OnPreviewHidden?.Invoke();

        TowerNode closestNode = FindClosestNode(mouseWorldPos);
        if (closestNode == null) return false;

        return closestNode.TryApplyCard(draggedCard, mergeDatabase);
    }

    public void UpdateSellDrag(Vector3 mouseWorldPos)
    {
        TowerNode closestNode = FindClosestNode(mouseWorldPos);
        if (closestNode != null && !closestNode.IsEmpty)
        {
            OnSellPreviewUpdated?.Invoke(closestNode.transform.position, true);
        }
        else
        {
            OnSellPreviewUpdated?.Invoke(mouseWorldPos, false);
        }
    }

    public bool TryDropSell(Vector3 mouseWorldPos)
    {
        OnPreviewHidden?.Invoke();

        TowerNode closestNode = FindClosestNode(mouseWorldPos);
        if (closestNode == null) return false;

        if (closestNode.TrySell(out int refundedGold))
        {
            OnTowerSold?.Invoke(refundedGold);
            return true;
        }

        return false;
    }

    public void CancelDrag()
    {
        OnPreviewHidden?.Invoke();
    }

    private TowerNode FindClosestNode(Vector3 worldPos)
    {
        worldPos.z = 0f;
        TowerNode bestNode = null;
        float bestDist = nodeSnapRadius;

        for (int i = 0; i < nodes.Count; i++)
        {
            TowerNode node = nodes[i];
            if (node == null) continue;

            Vector3 nodePos = node.transform.position;
            nodePos.z = 0f;

            float dist = (nodePos - worldPos).magnitude;
            if (dist <= bestDist)
            {
                bestDist = dist;
                bestNode = node;
            }
        }

        return bestNode;
    }

    private Sprite GetTowerSprite(TowerDataSO data)
    {
        if (data == null || data.towerPrefab == null) return null;
        SpriteRenderer sr = data.towerPrefab.GetComponentInChildren<SpriteRenderer>();
        return sr != null ? sr.sprite : null;
    }
}