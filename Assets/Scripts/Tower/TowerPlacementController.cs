using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementController : MonoBehaviour
{
    public event Action<int> OnTowerSold;

    [Header("data")]
    [SerializeField] private MergeDatabaseSO mergeDatabase;

    [Header("scene nodes")]
    [SerializeField] private List<TowerNode> nodes;
    [SerializeField] private float nodeSnapRadius = 1.2f;

    private TowerNode hoveredNode;

    public void UpdateCardDrag(TowerCardSO draggedCard, Vector3 mouseWorldPos)
    {
        if (draggedCard == null || draggedCard.towerData == null)
        {
            CancelDrag();
            return;
        }

        TowerNode closestNode = FindClosestNode(mouseWorldPos);

        // clear previous node preview if cursor moved away to a different spot
        if (hoveredNode != null && hoveredNode != closestNode)
        {
            hoveredNode.ClearPreview();
        }

        hoveredNode = closestNode;

        if (hoveredNode != null)
        {
            hoveredNode.ShowPreview(draggedCard, mergeDatabase);
        }
    }

    public bool TryDropCard(TowerCardSO draggedCard, Vector3 mouseWorldPos)
    {
        TowerNode targetNode = hoveredNode != null ? hoveredNode : FindClosestNode(mouseWorldPos);
        hoveredNode = null;

        if (targetNode == null) return false;

        return targetNode.TryConfirmDrop(draggedCard, mergeDatabase);
    }

    public void UpdateSellDrag(Vector3 mouseWorldPos)
    {
        // no ghost needed for sell drag
    }

    public bool TryDropSell(Vector3 mouseWorldPos)
    {
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
        if (hoveredNode != null)
        {
            hoveredNode.ClearPreview();
            hoveredNode = null;
        }
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
}